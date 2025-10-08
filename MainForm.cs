using System.Collections.Concurrent;
using System.Data;
using System.Data.SQLite;
using System.Globalization;

namespace QSOCollector
{
    public partial class QsoCollectorForm : Form
    {
        private readonly string connectionString;
        private readonly DbRepository dbRepository;
        private CancellationTokenSource? clientCancellationTokenSource = new();
        private ClientProgressUpdater? clientProgressUpdater;
        private ServerProgressUpdater? serverProgressUpdater;
        private DataTable? serverQsoAmountDataTable;
        private TcpServer? tcpServer = null;

        public QsoCollectorForm(string connectionString)
        {
            this.connectionString = connectionString;
            dbRepository = new DbRepository(connectionString);

            InitializeComponent();
            this.Text += $"        v.{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}";
            RestoreSavedFormValuesFromDB();
            HandleServerCheckBoxChanged(enableServerCheckBox);
            serverLogTextBox.Clear();
            HandleClientCheckBoxChanged(enableClientCheckBox);
            clientLogTextBox.Clear();
        }

        private void QsoCollectorForm_FormClosing(object sender, FormClosingEventArgs e)
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

        private void EnableServerCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            HandleServerCheckBoxChanged(sender as CheckBox);
        }

        private void HandleServerCheckBoxChanged(CheckBox? serverCheckBox)
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
            HandleCheckBoxForChildControls(checkbox, parent, checkbox.Checked);
        }

        private static void HandleCheckBoxForChildControls(CheckBox checkbox, Control parentControl, bool enabled)
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
                HandleCheckBoxForChildControls(checkbox, control, enabled);
            }
        }

        private void StopServerButton_Click(object sender, EventArgs e)
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
            serverLogTextBox.AppendText("Server stopped...\r\n");
            UpdateButton(startServerButton, true, Color.DarkSeaGreen);
            UpdateButton(stopServerButton, false);
        }

        private void StartServerButton_Click(object sender, EventArgs e)
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

        private async void StartServer(int port)
        {
            serverQsoAmountsDataGridView.DataSource = serverQsoAmountsBindingSource;
            GetDataForServerQsoAmountDataGridView(connectionString, "SELECT q.mode QsoAmountMode, COUNT(CASE WHEN q.qso_time >= current_date THEN 1 END) TodayQsoAmount, count(*) TotalQsoAmount, COUNT(e.id) ExportedQsoAmount, MAX(q.qso_time) LastQsoTime, MAX(e.end_time) LastExportedQsoTime FROM qsodata q LEFT JOIN adif_export e ON q.export_id = e.id AND e.is_confirmed = true WHERE q.is_temporary = false GROUP BY q.mode UNION ALL SELECT 'Total', COUNT(CASE WHEN q.qso_time >= current_date THEN 1 END), COUNT(*), COUNT(e.id), MAX(q.qso_time), MAX(e.end_time) FROM qsodata q JOIN adif_export e ON q.export_id = e.id AND e.is_confirmed = true WHERE q.is_temporary = false");
            HandleExportEnabled();

            serverProgressUpdater = new(serverQsoAmountDataTable, serverLogTextBox);
            tcpServer = new(port, serverProgressUpdater);
            await tcpServer.Start(connectionString);
        }

        private void GetDataForServerQsoAmountDataGridView(string connectionString, string selectCommand)
        {
            try
            {
                serverQsoAmountsDataAdapter = new SQLiteDataAdapter(selectCommand, connectionString);
                SQLiteCommandBuilder commandBuilder = new SQLiteCommandBuilder(serverQsoAmountsDataAdapter);
                serverQsoAmountDataTable = new()
                {
                    Locale = CultureInfo.InvariantCulture
                };
                serverQsoAmountsDataAdapter.Fill(serverQsoAmountDataTable);
                serverQsoAmountDataTable.PrimaryKey = [serverQsoAmountDataTable.Columns["QsoAmountMode"]];
                serverQsoAmountsBindingSource.DataSource = serverQsoAmountDataTable;
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show($"Can't retrieve data from DB: {ex.Message}");
            }
        }

        private static void UpdateButton(Button button, bool enabled)
        {
            UpdateButton(button, enabled, null);
        }

        private static void UpdateButton(Button button, bool enabled, Color? backColor)
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

        private void ServerPortTextBox_TextChanged(object sender, EventArgs e)
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

        private void PortTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void EnableClientCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is not CheckBox checkBox)
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

        private void StartClientButton_Click(object sender, EventArgs e)
        {
            List<ListenerConfig>? listeners = dbRepository.GetListenerConfigs();
            if (listeners == null || listeners.Count == 0)
            {
                MessageBox.Show("No active listener configurations found. Please configure at least one active listener.", "No Active Listeners", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string serverIp = clientServerNameIpTextBox.Text;
            int serverPort = Int32.Parse(clientServerPortTextBox.Text);
            BlockingCollection<QsoMessage> qsoMessageQueue = [];
            clientProgressUpdater = new(
                clientQsoReceivedCountLabel,
                clientQsoReceviedAtLabel,
                clientQsoSentToServerCountLabel,
                clientQsoSentToServerAtLabel,
                clientQsoTempSavedCountLabel,
                clientQsoTempSavedAtLabel,
                clientQsoRejectedCountLabel,
                clientQsoRejectedAtLabel,
                clientServerStatusValueLabel,
                clientServerCheckedAtLabel,
                clientLogTextBox);
            clientProgressUpdater.IsDebug = clientLogDetailsCheckBox.Checked;

            QsoMessageSender qsoMessageSender = StartQsoMessageHandler(qsoMessageQueue, serverIp, serverPort, clientProgressUpdater);
            if (!qsoMessageSender.IsConnected())
            {
                MessageBox.Show($"Looks like server is not avalable by address {serverIp}:{serverPort}. Please make sure it started", "Server not available", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            StartTemporarelySavedQsoHandler(qsoMessageQueue, qsoMessageSender, clientProgressUpdater);

            foreach (var listenerConfig in listeners)
            {
                StartClientUdpListener(listenerConfig, qsoMessageQueue, clientProgressUpdater);
            }
            enableClientCheckBox.Enabled = false;
            clientServerNameIpTextBox.Enabled = false;
            clientServerPortTextBox.Enabled = false;
            clientLogTextBox.Clear();
            startClientButton.Text = "Executing...";
            UpdateButton(startClientButton, false, Color.Lavender);
            UpdateButton(stopClientButton, true, Color.RosyBrown);
        }

        private QsoMessageSender StartQsoMessageHandler(BlockingCollection<QsoMessage> qsoMessageQueue, string serverIp, int serverPort, ClientProgressUpdater clientProgressUpdater)
        {
            CancellationTokenSource qsoMessageSenderCancellationTokenSource = CreateLinkedClientCancellationTokenSource();
            var sender = new QsoMessageSender(serverIp, serverPort, qsoMessageQueue, dbRepository, clientProgressUpdater, qsoMessageSenderCancellationTokenSource);
            if (sender.IsConnected()) Task.Run(() => sender.Start());
            return sender;
        }

        private void StartTemporarelySavedQsoHandler(BlockingCollection<QsoMessage> qsoMessageQueue, QsoMessageSender qsoMessageHandler, ClientProgressUpdater progressUpdater)
        {
            CancellationTokenSource temporarelySavedQsoHandlerCancellationTokenSource = CreateLinkedClientCancellationTokenSource();
            var handler = new TemporarelySavedQsoHandler(dbRepository, qsoMessageQueue, qsoMessageHandler, progressUpdater, temporarelySavedQsoHandlerCancellationTokenSource);
            Task.Run(() => handler.Start());
        }

        private void StartClientUdpListener(ListenerConfig listenerConfig, BlockingCollection<QsoMessage> qsoMessageQueue, ClientProgressUpdater clientProgressUpdater)
        {
            CancellationTokenSource clientUdpListenerCancellationTokenSource = CreateLinkedClientCancellationTokenSource();
            var listener = new UdpClientListener(listenerConfig, qsoMessageQueue, clientProgressUpdater, clientUdpListenerCancellationTokenSource);
            Task.Run(() => listener.Start());
        }

        private CancellationTokenSource CreateLinkedClientCancellationTokenSource()
        {
            return CancellationTokenSource.CreateLinkedTokenSource(clientCancellationTokenSource.Token);
        }

        private void StopClientButton_Click(object sender, EventArgs e)
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
            clientProgressUpdater = null;
            enableClientCheckBox.Enabled = true;
            clientServerNameIpTextBox.Enabled = true;
            clientServerPortTextBox.Enabled = true;
            startClientButton.Text = "Start Client";
            clientServerStatusValueLabel.Text = "Unknown";
            clientServerStatusValueLabel.ForeColor = Color.Gray;
            clientServerCheckedAtLabel.Text = "---";
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
            Dictionary<string, string?> settings = dbRepository.LoadSettings();
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
            dbRepository.SaveSetting("ServerEnabled", enableServerCheckBox.Checked.ToString());
            dbRepository.SaveSetting("ServerPort", serverPortTextBox.Text);
            dbRepository.SaveSetting("ClientEnabled", enableClientCheckBox.Checked.ToString());
            dbRepository.SaveSetting("ClientServerNameIp", clientServerNameIpTextBox.Text);
            dbRepository.SaveSetting("ClientServerPort", clientServerPortTextBox.Text);
        }

        private void ListenersConfigButton_Click(object sender, EventArgs e)
        {
            new ListenersForm(connectionString).ShowDialog(this);
        }

        private void clientLogDetailsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (clientProgressUpdater != null)
            {
                clientProgressUpdater.IsDebug = clientLogDetailsCheckBox.Checked;
            }
        }

        private void serverShowLogDetailsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (serverProgressUpdater != null)
            {
                serverProgressUpdater.IsDebug = serverShowLogDetailsCheckBox.Checked;
            }
        }

        private void qsoExportButton_Click(object sender, EventArgs e)
        {
            List<QsoExportExpectedAmounts> expectedAmounts = dbRepository.GetQsoAmountsForExport();
            if (expectedAmounts.Count == 0)
            {
                MessageBox.Show("There are no QSOs in Database yet. Nothing to export", "No Data for export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            new QsoExportForm(dbRepository, expectedAmounts).ShowDialog(this);
        }

        private void qsoImportButton_Click(object sender, EventArgs e)
        {
            new QsoImportForm(dbRepository).ShowDialog(this);
        }

        private void serverQsoAmountsDataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            HandleExportEnabled();
        }
        private void serverQsoAmountsDataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            HandleExportEnabled();
        }

        private void HandleExportEnabled()
        {
            // not only total row should be present
            qsoExportButton.Enabled = serverQsoAmountsDataGridView.RowCount > 1;
        }

    }
}
