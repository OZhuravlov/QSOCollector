using Microsoft.Data.Sqlite;
using System.Data;
using System.Data.Common;

namespace QSOCollector
{

    public partial class qsoCollectorForm : Form
    {
        private readonly string connectionString;
        private readonly SettingsRepository settingsRepository;
        private CancellationTokenSource clientCancellationTokenSource = new();
        private TcpServer? tcpServer = null;
        private CancellationTokenSource serverCancellationTokenSource = new();

        public qsoCollectorForm(string connectionString)
        {
            this.connectionString = connectionString;
            this.settingsRepository = new SettingsRepository(connectionString);
            InitializeComponent();
            RestoreSavedFormValuesFromDB();
            handleServerCheckBoxChanged(enableServerCheckBox);
            HandleClientCheckBoxChanged(enableClientCheckBox);
        }


        private void qsoCollectorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (stopClientButton.Enabled)
            {
                DialogResult result = MessageBox.Show("The client is still running. Are you sure you want to exit?", "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }

            if (stopServerButton.Enabled)
            {
                DialogResult result = MessageBox.Show("The server is still running. Are you sure you want to exit?", "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }
            SaveFormValuesToDB();
        }

        private void enableServerCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            handleServerCheckBoxChanged(sender as CheckBox);
        }

        private void handleServerCheckBoxChanged(CheckBox? serverCheckBox)
        {
            if (serverCheckBox == null)
            {
                return;
            }
            HandleCheckBoxChanged(serverCheckBox);
            if (serverCheckBox.Checked)
            {
                StopServer();
            }
            else
            {
                serverLogTextBox.Clear();
            }

            if (serverCheckBox.Checked && string.IsNullOrEmpty(serverPortTextBox.Text))
            {
                UpdateButton(startServerButton, false);
                serverPortTextBox.Focus();
            }
        }

        private void HandleCheckBoxChanged(CheckBox checkbox)
        {
            Control? parent = checkbox.Parent;
            if (parent == null)
            {
                return;
            }
            handleCheckBoxForChildControls(checkbox, parent, checkbox.Checked);
        }

        private void handleCheckBoxForChildControls(CheckBox checkbox, Control parentControl, bool enabled)
        {
            foreach (Control control in parentControl.Controls)
            {
                if (control == checkbox) continue;

                if (control is Button button)
                {
                    UpdateButton(button, enabled);
                    continue;
                }

                control.Enabled = enabled;
                handleCheckBoxForChildControls(checkbox, control, enabled);
            }
        }

        private void stopServerButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to stop the server?", "Confirm Stop Server", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result != DialogResult.Yes)
            {
                return;
            }
            StopServer();
        }
        private void StopServer()
        {
            if (tcpServer != null)
            {
                tcpServer.Stop();
                tcpServer = null;
            }
            enableServerCheckBox.Enabled = true;
            serverPortTextBox.Enabled = true;
            startServerButton.Text = "Start Server";
            UpdateButton(startServerButton, true, Color.DarkSeaGreen);
            UpdateButton(stopServerButton, false);
        }

        private void startServerButton_Click(object sender, EventArgs e)
        {
            int port = Int32.Parse(serverPortTextBox.Text);
            StartServer(port);
            enableServerCheckBox.Enabled = false;
            serverPortTextBox.Enabled = false;
            serverLogTextBox.Clear();
            startServerButton.Text = "Executing...";
            UpdateButton(startServerButton, false, Color.Lavender);
            UpdateButton(stopServerButton, true, Color.RosyBrown);
            serverLogTextBox.AppendText("Server started...\r\n");
        }

        private async void StartServer(int port) {
            tcpServer = new(port);
            await tcpServer.Start(connectionString, serverLogTextBox);
        }

        private void UpdateButton(Button button, bool enabled)
        {
            UpdateButton(button, enabled, null);
        }

        private void UpdateButton(Button button, bool enabled, Color? backColor)
        {
            button.Enabled = enabled;
            Color color;
            FontStyle fontStyle;
            if (button.Enabled)
            {
                color = backColor == null ? Color.DarkCyan : backColor.Value;
                fontStyle = FontStyle.Bold;
            }
            else
            {
                color = backColor == null ? Color.Transparent : backColor.Value;
                fontStyle = FontStyle.Regular;
            }
            button.BackColor = color;
            button.Font = new Font(button.Font, fontStyle);
        }

        private void serverPortTextBox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(serverPortTextBox.Text))
            {
                UpdateButton(startServerButton, false);
            }
            else
            {
                UpdateButton(startServerButton, true, Color.DarkSeaGreen);
            }
        }

        private void portTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void enableClientCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox? checkBox = sender as CheckBox;
            if (checkBox == null)
            {
                return;
            }
            HandleClientCheckBoxChanged(checkBox);
        }

        private void HandleClientCheckBoxChanged(CheckBox? clientCheckBox)
        {
            if (clientCheckBox == null)
            {
                return;
            }
            HandleCheckBoxChanged(clientCheckBox);

            if (clientCheckBox.Checked)
            {
                StopClient();
            }
            else
            {
                clientLogTextBox.Clear();
            }

            if (clientCheckBox.Checked &&
                (string.IsNullOrEmpty(clientServerNameIpTextBox.Text) || string.IsNullOrEmpty(clientServerPortTextBox.Text)))
            {
                UpdateButton(startClientButton, false);
                clientServerNameIpTextBox.Focus();
            }
        }

        private void startClientButton_Click(object sender, EventArgs e)
        {
            List<ListenerConfig>? listeners = settingsRepository.GetListenerConfigs();
            if (listeners == null || listeners.Count == 0)
            {
                MessageBox.Show("No active listener configurations found. Please configure at least one active listener.", "No Active Listeners", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            enableClientCheckBox.Enabled = false;
            clientServerNameIpTextBox.Enabled = false;
            clientServerPortTextBox.Enabled = false;
            clientLogTextBox.Clear();
            startClientButton.Text = "Executing...";
            UpdateButton(startClientButton, false, Color.Lavender);
            UpdateButton(stopClientButton, true, Color.RosyBrown);

            string serverIp = clientServerNameIpTextBox.Text;
            int serverPort = Int32.Parse(clientServerPortTextBox.Text);

            foreach (var listener in listeners)
            {
                StartClientUdpListenerTask(listener.QsoPort, serverIp, serverPort, clientCancellationTokenSource.Token);
            }
        }

        private void StartClientUdpListenerTask(int localPort, string serverIp, int serverPort, CancellationToken cancellationToken)
        {
            var listener = new UdpClientListener(localPort, serverIp, serverPort, clientLogTextBox);
            Task.Run(() => listener.StartAsync(cancellationToken));
        }

        private void stopClientButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to stop the client?", "Confirm Stop Client", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result != DialogResult.Yes)
            {
                return;
            }
            StopClient();
        }

        private void StopClient()
        {
            clientCancellationTokenSource.Cancel(true);
            if (clientCancellationTokenSource.IsCancellationRequested)
            {
                clientCancellationTokenSource.Dispose();
                clientCancellationTokenSource = new CancellationTokenSource();
            }
            enableClientCheckBox.Enabled = true;
            clientServerNameIpTextBox.Enabled = true;
            clientServerPortTextBox.Enabled = true;
            startClientButton.Text = "Start Client";
            UpdateButton(startClientButton, true, Color.DarkSeaGreen);
            UpdateButton(stopClientButton, false);
        }

        private void clientServerNameIpTextBox_TextChanged(object sender, EventArgs e)
        {
            HandleClientServerChanged();
        }

        private void clientServerPortTextBox_TextChanged(object sender, EventArgs e)
        {
            HandleClientServerChanged();
        }

        private void HandleClientServerChanged()
        {
            if (string.IsNullOrEmpty(clientServerNameIpTextBox.Text)
                || string.IsNullOrEmpty(clientServerPortTextBox.Text))
            {
                UpdateButton(startClientButton, false);
                UpdateButton(stopClientButton, false);
            }
            else
            {
                UpdateButton(startClientButton, true, Color.DarkSeaGreen);
            }
        }

        private void RestoreSavedFormValuesFromDB()
        {
            var settings = settingsRepository.LoadSettings();
            if (settings.TryGetValue("ServerEnabled", out var serverEnabled))
                enableServerCheckBox.Checked = Convert.ToBoolean(serverEnabled);
            if (settings.TryGetValue("ServerPort", out var serverPort))
                serverPortTextBox.Text = serverPort;
            if (settings.TryGetValue("ClientEnabled", out var clientEnabled))
                enableClientCheckBox.Checked = Convert.ToBoolean(clientEnabled);
            if (settings.TryGetValue("ClientServerNameIp", out var clientServerNameIp))
                clientServerNameIpTextBox.Text = clientServerNameIp;
            if (settings.TryGetValue("ClientServerPort", out var clientServerPort))
                clientServerPortTextBox.Text = clientServerPort;
        }

        private void SaveFormValuesToDB()
        {
            settingsRepository.SaveSetting("ServerEnabled", enableServerCheckBox.Checked.ToString());
            settingsRepository.SaveSetting("ServerPort", serverPortTextBox.Text);
            settingsRepository.SaveSetting("ClientEnabled", enableClientCheckBox.Checked.ToString());
            settingsRepository.SaveSetting("ClientServerNameIp", clientServerNameIpTextBox.Text);
            settingsRepository.SaveSetting("ClientServerPort", clientServerPortTextBox.Text);
        }

        private void listenersConfigButton_Click(object sender, EventArgs e)
        {
            new ListenersForm(connectionString).ShowDialog(this);
        }
    }
}
