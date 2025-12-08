using Microsoft.Win32;
using QSOCollector.Data;
using QSOCollector.Helpers;
using QSOCollector.Models;
using QSOCollector.Network;
using System.Collections.Concurrent;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace QSOCollector
{
    public partial class QsoCollectorForm : Form
    {
        private readonly string connectionString;
        private readonly DbRepository dbRepository;
        private readonly StartupParams startupParams;
        private CancellationTokenSource? clientCancellationTokenSource = new();
        private ClientProgressUpdater? clientProgressUpdater;
        private ServerProgressUpdater? serverProgressUpdater;
        private DataTable? serverQsoAmountDataTable;
        private TcpServer? tcpServer = null;
        private bool isLocalClientRunning = false;
        private bool isLocalServerRunning = false;

        public QsoCollectorForm(string connectionString, StartupParams startupParams)
        {
            this.connectionString = connectionString;
            dbRepository = new DbRepository(connectionString);
            this.startupParams = startupParams;

            InitializeComponent();
            this.Text += $"        v.{Assembly.GetExecutingAssembly().GetName().Version}";
            RestoreSavedFormValuesFromDB();
            HandleServerCheckBoxChanged(enableServerCheckBox);
            serverLogTextBox.Clear();
            HandleClientCheckBoxChanged(enableClientCheckBox);
            clientLogTextBox.Clear();
            PopulateAboutTab();
        }

        private void PopulateAboutTab()
        {
            string readme = Assembly.GetExecutingAssembly().GetName().Name + ".README.md";
            Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(readme);
            if (stream != null)
            {
                using StreamReader reader = new(stream);
                string result = reader.ReadToEnd();
                aboutTextBox.Text = result;
            } else
            {
                aboutTab.Visible = false;
            }
        }

        private void QsoCollectorForm_Shown(object sender, EventArgs e)
        {
            HandleStartupParams(startupParams);
        }

        private void HandleStartupParams(StartupParams startupParams)
        {
            if (startupParams.StartServer)
            {
                Thread.Sleep(2000);
                AutoStartServer();
            }

            if (startupParams.StartClient)
            {
                Thread.Sleep(2000);
                AutoStartClient();
            }

            if (startupParams.IsQuiet)
            {
                this.WindowState = FormWindowState.Minimized;
                this.Refresh();
                MinimizeAppToTray();
            }
        }

        private void AutoStartClient()
        {
            enableClientCheckBox.Enabled = true;
            HandleClientCheckBoxChanged(enableClientCheckBox);
            clientServerPortTextBox_TextChanged(clientServerPortTextBox, EventArgs.Empty);
            if (startClientButton.Enabled)
            {
                mainTabControl.SelectedTab = clientTab;
                StartClientButton_Click(startClientButton, EventArgs.Empty);
                clientTab.Refresh();
            }
            else
            {
                MessageBox.Show("Cannot start client automatically because client is not configured properly", "Cannot Start Clienmt", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AutoStartServer()
        {
            enableServerCheckBox.Enabled = true;
            HandleServerCheckBoxChanged(enableServerCheckBox);
            ServerPortTextBox_TextChanged(serverPortTextBox, EventArgs.Empty);
            if (startServerButton.Enabled)
            {
                mainTabControl.SelectedTab = serverTab;
                StartServerButton_Click(startServerButton, EventArgs.Empty);
                serverTab.Refresh();
            }
            else
            {
                MessageBox.Show("Cannot start client automatically because server is not configured properly", "Cannot Start Server", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void QsoCollectorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isLocalClientRunning)
            {
                DialogResult result = MessageBox.Show("The client is still running. Are you sure you want to exit?", "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }

            if (!e.Cancel && isLocalServerRunning)
            {
                DialogResult result = MessageBox.Show("The server is still running. Are you sure you want to exit?", "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }
            if (!e.Cancel)
            {
                SaveFormValuesToDB();
            }
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
                serverQsoAmountsDataGridView.DataSource = serverQsoAmountsBindingSource;
                GetDataForServerQsoAmountDataGridView();
            }
            else
            {
                serverLogTextBox.Clear();
                ClearDataForServerQsoAmountDataGridView();
            }

            if (serverCheckBox.Checked && string.IsNullOrEmpty(serverPortTextBox.Text))
            {
                ButtonStyleHandler.Update(startServerButton, false);
                serverPortTextBox.Focus();
            }
            autoStartCheckbox_CheckedChanged(autoStartCheckbox, EventArgs.Empty);
        }

        private static void HandleCheckBoxChanged(CheckBox checkbox)
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
                    ButtonStyleHandler.Update(button, enabled);
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
            isLocalServerRunning = false;
            serverLogTextBox.AppendText("Server stopped...\r\n");
            ButtonStyleHandler.Update(startServerButton, true, Color.DarkSeaGreen);
            ButtonStyleHandler.Update(stopServerButton, false);
        }

        private void StartServerButton_Click(object sender, EventArgs e)
        {
            int port = Int32.Parse(serverPortTextBox.Text);
            StartServer(port);
            enableServerCheckBox.Enabled = false;
            serverPortTextBox.Enabled = false;
            serverLogTextBox.Clear();
            startServerButton.Text = "Executing...";
            ButtonStyleHandler.Update(startServerButton, false, Color.Lavender);
            ButtonStyleHandler.Update(stopServerButton, true, Color.RosyBrown);
            serverLogTextBox.AppendText("Server started...\r\n");
        }

        private async void StartServer(int port)
        {
            HandleExportEnabled();

            serverProgressUpdater = new(serverQsoAmountDataTable, serverLogTextBox);
            tcpServer = new(port, serverProgressUpdater);
            isLocalServerRunning = true;
            await tcpServer.Start(connectionString);
        }

        private void GetDataForServerQsoAmountDataGridView()
        {
            string selectCommand = "SELECT q.mode QsoAmountMode, COUNT(CASE WHEN q.qso_time >= current_date THEN 1 END) TodayQsoAmount, COUNT(*) TotalQsoAmount, COUNT(e.id) ExportedQsoAmount, MAX(q.qso_time) LastQsoTime, MAX(CASE WHEN e.id IS NOT NULL THEN q.qso_time END) LastExportedQsoTime FROM qsodata q LEFT JOIN adif_export e ON q.export_id = e.id AND e.is_confirmed = true WHERE q.is_temporary = false GROUP BY q.mode UNION ALL SELECT 'Total', COUNT(CASE WHEN q.qso_time >= current_date THEN 1 END), COUNT(*), COUNT(e.id), MAX(q.qso_time), MAX(e.end_time) FROM qsodata q LEFT JOIN adif_export e ON q.export_id = e.id AND e.is_confirmed = true WHERE q.is_temporary = false";
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

        private void ClearDataForServerQsoAmountDataGridView()
        {
            serverQsoAmountDataTable?.Rows.Clear();
        }

        private void ServerPortTextBox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(serverPortTextBox.Text))
            {
                ButtonStyleHandler.Update(startServerButton, false);
            }
            else
            {
                ButtonStyleHandler.Update(startServerButton, true, Color.DarkSeaGreen);
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
                ButtonStyleHandler.Update(startClientButton, false);
                clientServerNameIpTextBox.Focus();
            }
            autoStartCheckbox_CheckedChanged(autoStartCheckbox, EventArgs.Empty);
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
                clientLogTextBox)
            {
                IsDebug = clientLogDetailsCheckBox.Checked
            };

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
            ButtonStyleHandler.Update(startClientButton, false, Color.Lavender);
            ButtonStyleHandler.Update(stopClientButton, true, Color.RosyBrown);
            isLocalClientRunning = true;
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
            ButtonStyleHandler.Update(startClientButton, true, Color.DarkSeaGreen);
            ButtonStyleHandler.Update(stopClientButton, false);
            isLocalClientRunning = false;
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
                ButtonStyleHandler.Update(startClientButton, false);
                ButtonStyleHandler.Update(stopClientButton, false);
            }
            else
            {
                ButtonStyleHandler.Update(startClientButton, true, Color.DarkSeaGreen);
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
            if (settings.TryGetValue("AutoStart", out var autoStart))
                autoStartCheckbox.Checked = Convert.ToBoolean(autoStart);
        }

        private void SaveFormValuesToDB()
        {
            dbRepository.SaveSetting("ServerEnabled", enableServerCheckBox.Checked.ToString());
            dbRepository.SaveSetting("ServerPort", serverPortTextBox.Text);
            dbRepository.SaveSetting("ClientEnabled", enableClientCheckBox.Checked.ToString());
            dbRepository.SaveSetting("ClientServerNameIp", clientServerNameIpTextBox.Text);
            dbRepository.SaveSetting("ClientServerPort", clientServerPortTextBox.Text);
            dbRepository.SaveSetting("AutoStart", autoStartCheckbox.Checked.ToString());
        }

        private void ListenersConfigButton_Click(object sender, EventArgs e)
        {
            new ListenersForm(connectionString, isLocalClientRunning).ShowDialog(this);
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
            if (enableServerCheckBox.Checked)
            {
                GetDataForServerQsoAmountDataGridView();
                HandleExportEnabled();
            }
        }

        private void qsoImportButton_Click(object sender, EventArgs e)
        {
            new QsoImportForm(dbRepository).ShowDialog(this);
            if (enableServerCheckBox.Checked)
            {
                GetDataForServerQsoAmountDataGridView();
                HandleExportEnabled();
            }
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

        private void QsoCollectorForm_SizeChanged(object sender, EventArgs e)
        {
            bool mousePointerNotOnTaskBar = Screen.GetWorkingArea(this).Contains(Cursor.Position);
            if (this.WindowState == FormWindowState.Minimized && mousePointerNotOnTaskBar)
            {
                MinimizeAppToTray();
                return;
            }
            trayNotifyIcon.Visible = false;
            this.ShowInTaskbar = true;
        }

        private void trayNotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            if (this.WindowState == FormWindowState.Normal)
            {
                this.ShowInTaskbar = true;
                trayNotifyIcon.Visible = false;
            }
        }

        private void MinimizeAppToTray()
        {
            string[] roles = [];
            if (startupParams.StartServer) roles = [.. roles, "Server"];
            if (startupParams.StartClient) roles = [.. roles, "Client"];
            string message = "QSO Collector " + string.Join(" and ", roles) + ((roles.Length > 1) ? " are " : " is ") + "running on background\nDouble-click to restore";
            trayNotifyIcon.BalloonTipText = message;
            trayNotifyIcon.Text = message;

            trayNotifyIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            trayNotifyIcon.ShowBalloonTip(1000);
            this.ShowInTaskbar = false;
            trayNotifyIcon.Visible = true;
        }

        private void autoStartCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (autoStartCheckbox.Checked)
            {
                rk.SetValue(Assembly.GetExecutingAssembly().GetName().Name, GetAppStartupCmd());
            }
            else
            {
                rk.DeleteValue(Assembly.GetExecutingAssembly().GetName().Name, false);
            }
        }

        private string GetAppStartupCmd()
        {
            StringBuilder sb = new();
            sb.Append($"\"{Application.ExecutablePath}\"");
            if (enableServerCheckBox.Enabled)
            {
                sb.Append(" --start-server");
            }
            if (enableClientCheckBox.Enabled)
            {
                sb.Append(" --start-client");
            }
            if (enableClientCheckBox.Enabled || enableServerCheckBox.Enabled)
            {
                sb.Append(" --quiet");
            }
            return sb.ToString();
        }
    }
}
