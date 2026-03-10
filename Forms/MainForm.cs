using Microsoft.Win32;
using QSOCollector.Data;
using QSOCollector.Forms;
using QSOCollector.Helpers;
using QSOCollector.Models;
using QSOCollector.Network.Client;
using QSOCollector.Network.Server;
using QSOCollector.Service;
using Serilog; 
using System.Collections.Concurrent;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace QSOCollector
{
    public partial class QsoCollectorForm : Form
    {
        private readonly ILogger log = Log.ForContext<QsoCollectorForm>();

        private readonly IDbRepository dbRepository;
        private readonly StartupParams startupParams;
        private readonly DataTable serverQsoAmountDataTable;
        private CancellationTokenSource clientCancellationTokenSource = new();
        private CancellationTokenSource serverCancellationTokenSource = new();
        private ClientProgressUpdater? clientProgressUpdater;
        private ServerProgressUpdater? serverProgressUpdater;
        private TcpServer? tcpServer = null;
        private bool isLocalClientRunning = false;
        private bool isLocalServerRunning = false;
        private AutoExportTaskService autoExportTaskService;

        public QsoCollectorForm(StartupParams startupParams, IDbRepository dbRepository)
        {
            this.dbRepository = dbRepository ?? throw new ArgumentNullException(nameof(dbRepository));

            serverQsoAmountDataTable = new()
            {
                Locale = CultureInfo.InvariantCulture
            };

            this.startupParams = startupParams;

            InitializeComponent();
            this.Text += $"        v.{Assembly.GetExecutingAssembly().GetName().Version}";
            RestoreSettingsFromDb();
            HandleServerCheckBoxChanged(enableServerCheckBox);
            HandleClientCheckBoxChanged(enableClientCheckBox);
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
            }
            else
            {
                aboutTab.Visible = false;
            }
        }

        private void QsoCollectorForm_Shown(object sender, EventArgs e)
        {
            HandleStartupParams(startupParams);
            if (enableServerCheckBox.Checked && !isLocalServerRunning && !isLocalClientRunning)
            {
                mainTabControl.SelectedTab = serverTab;
            }
        }

        private void HandleStartupParams(StartupParams startupParams)
        {
            log.Information("Handling application startup parameters: {@startupParams}", startupParams);

            enableDebugWhenAutoStartCheckbox.Checked = startupParams.Debug;
            if (startupParams.StartServer)
            {
                string logMessage = "Auto-starting server requested";
                if (startupParams.Debug)
                {
                    log.Information("{message} with Log Details enabled", logMessage);
                    serverShowLogDetailsCheckBox.Checked = true;
                }
                else
                {
                    log.Information(logMessage);
                }

                Thread.Sleep(2000);
                AutoStartServer();
                if (!isLocalServerRunning)
                {
                    log.Warning("Server hasn't been auto-started");
                    startupParams.IsQuiet = false;
                }
            }

            if (startupParams.StartClient)
            {
                string logMessage = "Auto-starting client requested";
                if (startupParams.Debug)
                {
                    log.Information("{message} with Log Details enabled", logMessage);
                    clientLogDetailsCheckBox.Checked = true;
                }

                Thread.Sleep(2000);
                AutoStartClient();
                if (!isLocalClientRunning)
                {
                    log.Warning("Client hasn't been auto-started");
                    startupParams.IsQuiet = false;
                }
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

                try
                {
                    StartClientButton_Click(startClientButton, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    log.Error(ex, "Error while auto-starting Client");
                    throw;
                }

                clientTab.Refresh();
            }
            else
            {
                string message = "Cannot start client automatically because Client is not enabled";
                log.Error(message);
                MessageBox.Show(message, "Cannot Start Client", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AutoStartServer()
        {
            enableServerCheckBox.Enabled = true;
            HandleServerCheckBoxChanged(enableServerCheckBox);
            ServerPortTextBox_TextChanged(serverPortTextBox, EventArgs.Empty);
            if (startServerButton.Enabled)
            {
                string logMessage = "Auto-starting server...";
                log.Information(logMessage);
                serverLogTextBox.AppendText($"{logMessage}\r\n");
                mainTabControl.SelectedTab = serverTab;

                try
                {
                    StartServerButton_Click(startServerButton, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    log.Error(ex, "Error while auto-staring Server");
                    throw;
                }

                serverTab.Refresh();
            }
            else
            {
                string message = "Cannot start server automatically because Server is not enabled";
                log.Error(message);
                MessageBox.Show(message, "Cannot Start Server", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void QsoCollectorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            log.Information("Main form closing, checking if client or server are still running");
            if (isLocalClientRunning)
            {
                string logMessage = "The client is still running.";
                DialogResult result = MessageBox.Show($"{logMessage} Are you sure you want to exit?", "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
                else
                {
                    log.Warning("User confirmed to exit. Client will be stopped");
                }
            }

            if (!e.Cancel && isLocalServerRunning)
            {
                string logMessage = "The server is still running.";
                DialogResult result = MessageBox.Show($"{logMessage} Are you sure you want to exit?", "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
                else
                {
                    log.Warning("User confirmed to exit. Server will be stopped");
                }
            }
            if (!e.Cancel)
            {
                log.Information("Saving form values to DB before exit");
                SaveSettingsToDB();
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
                log.Warning("HandleServerCheckBoxChanged was called with null CheckBox");
                return;
            }
            HandleCheckBoxChanged(serverCheckBox);
            if (serverCheckBox.Checked)
            {
                log.Verbose("Server enabled, populating QSO Amount DataGridView");
                StopServer();
                serverQsoAmountsDataGridView.DataSource = serverQsoAmountsBindingSource;
                PopulateServerQsoAmountDataGridView();
            }
            else
            {
                log.Verbose("Server disabled, clearing log textbox and QSO Amount DataGridView");
                serverLogTextBox.Clear();
                ClearDataForServerQsoAmountDataGridView();
            }

            if (serverCheckBox.Checked && string.IsNullOrEmpty(serverPortTextBox.Text))
            {
                log.Debug("Server enabled but port is not set");
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
            log.Information("Stopping Server with UI button");
            StopServer();
        }

        private void StopServer()
        {
            serverCancellationTokenSource = RenewToken(serverCancellationTokenSource);
            tcpServer?.Stop();
            tcpServer = null;
            enableServerCheckBox.Enabled = true;
            serverPortTextBox.Enabled = true;
            startServerButton.Text = "Start Server";
            isLocalServerRunning = false;
            serverLogTextBox.AppendText("Server stopped...\r\n");
            ButtonStyleHandler.Update(startServerButton, true, Color.DarkSeaGreen);
            ButtonStyleHandler.Update(stopServerButton, false);
            ButtonStyleHandler.Update(resetServerButton, true, Color.LightGray);
            ButtonStyleHandler.Update(qsoAutoExportButton, true, Color.LightGray);
            ButtonStyleHandler.Update(qsoAutoExportButton, false, Color.Lavender);
            StopAutoExportTask();
        }

        private void StartServerButton_Click(object sender, EventArgs e)
        {
            log.Information("Starting Server ...");
            int port = Int32.Parse(serverPortTextBox.Text);
            StartServer(port);
            enableServerCheckBox.Enabled = false;
            serverPortTextBox.Enabled = false;
            serverLogTextBox.Clear();
            startServerButton.Text = "Executing...";
            ButtonStyleHandler.Update(startServerButton, false, Color.Lavender);
            ButtonStyleHandler.Update(stopServerButton, true, Color.RosyBrown);
            ButtonStyleHandler.Update(resetServerButton, false, Color.Lavender);
            ButtonStyleHandler.Update(qsoAutoExportButton, true, Color.LightGray);
            string logMessage = $"Server started on port {port}";
            log.Information(logMessage);
            serverLogTextBox.AppendText($"{logMessage}\r\n");
            StartAutoExportTask();
        }

        private async void StartServer(int port)
        {
            HandleExportEnabled();

            try
            {
                serverProgressUpdater = new(serverQsoAmountDataTable, serverLogTextBox);
                tcpServer = new(port, serverProgressUpdater, dbRepository);
                isLocalServerRunning = true;
                await tcpServer.Start();
                await RefreshServerQsoAmountDataTable();
            }
            catch (Exception ex)
            {
                string message = $"Cannot start server on port {port}";
                log.Error(ex, message);
                MessageBox.Show($"{message}: {ex.Message}", "Cannot Start Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                StopServer();
                throw;
            }
            finally
            {
                isLocalServerRunning = false;
            }
        }

        private async Task RefreshServerQsoAmountDataTable()
        {
            while (enableServerCheckBox.Checked)
            {
                PopulateServerQsoAmountDataGridView();
                await Task.Delay(60000);
            }
        }

        private void PopulateServerQsoAmountDataGridView()
        {
            string selectCommand = "SELECT q.mode QsoAmountMode, COUNT(CASE WHEN q.qso_time >= current_date THEN 1 END) TodayQsoAmount, COUNT(*) TotalQsoAmount, COUNT(e.id) ExportedQsoAmount, MAX(q.qso_time) LastQsoTime, MAX(CASE WHEN e.id IS NOT NULL THEN q.qso_time END) LastExportedQsoTime FROM qsodata q LEFT JOIN adif_export e ON q.export_id = e.id AND e.is_confirmed = true WHERE q.is_temporary = false GROUP BY q.mode UNION ALL SELECT 'Total', COUNT(CASE WHEN q.qso_time >= current_date THEN 1 END), COUNT(*), COUNT(e.id), MAX(q.qso_time), MAX(e.end_time) FROM qsodata q LEFT JOIN adif_export e ON q.export_id = e.id AND e.is_confirmed = true WHERE q.is_temporary = false";
            try
            {
                log.Debug("Populate QSO Amount table");
                serverQsoAmountsDataAdapter = new SQLiteDataAdapter(selectCommand, dbRepository.GetConnectionString());
                SQLiteCommandBuilder commandBuilder = new(serverQsoAmountsDataAdapter);
                ClearDataForServerQsoAmountDataGridView();
                serverQsoAmountsDataAdapter.Fill(serverQsoAmountDataTable);
                serverQsoAmountDataTable.PrimaryKey = [serverQsoAmountDataTable.Columns["QsoAmountMode"]];
                serverQsoAmountsBindingSource.DataSource = serverQsoAmountDataTable;
            }
            catch (SQLiteException ex)
            {
                string message = "Can't retrieve data from DB";
                log.Error(ex, message);
                MessageBox.Show($"{message}: {ex.Message}");
            }
        }

        private void ClearDataForServerQsoAmountDataGridView()
        {
            log.Verbose("Clearing QSO Amount table data");
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
                log.Warning("HandleClientCheckBoxChanged was called with null CheckBox");
                return;
            }
            HandleCheckBoxChanged(clientCheckBox);

            if (clientCheckBox.Checked)
            {
                StopClient();
            }
            else
            {
                log.Verbose("Client disabled, clearing log textbox");
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
                MessageBox.Show(
                    "No active listener configurations found. Please configure at least one active listener.",
                    "No Active Listeners",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
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

            CancellationTokenSource qsoMessageSenderCancellationTokenSource = CreateLinkedClientCancellationTokenSource();
            QsoMessageSender qsoMessageSender = new(serverIp, serverPort, qsoMessageQueue, dbRepository, clientProgressUpdater, qsoMessageSenderCancellationTokenSource);

            bool keepClientRunning;
            if (qsoMessageSender.IsConnected())
            {
                keepClientRunning = true;
            }
            else
            {
                string message = $"Looks like server is not avalable by address {serverIp}:{serverPort}";
                keepClientRunning = false;
                if (startupParams.IsQuiet)
                {
                    clientLogTextBox.AppendText(message + "\r\n");
                    keepClientRunning = true;
                }
                else
                {
                    DialogResult result = MessageBox.Show(message + ". Would you like Client to start and wait for Server connection?", "Server not available", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    keepClientRunning = result == DialogResult.Yes;
                }
                if (!keepClientRunning)
                {
                    return;
                }
            }

            if (keepClientRunning) Task.Run(() => qsoMessageSender.Start());

            StartTemporarelySavedQsoHandler(qsoMessageQueue, qsoMessageSender, clientProgressUpdater);
            Dictionary<int, UdpClient> forwardUdpClients = StartForwardUdpClients(listeners, clientProgressUpdater);
            StartClientUdpListeners(listeners, forwardUdpClients, qsoMessageQueue, clientProgressUpdater);

            enableClientCheckBox.Enabled = false;
            clientServerNameIpTextBox.Enabled = false;
            clientServerPortTextBox.Enabled = false;
            clientLogTextBox.Clear();
            startClientButton.Text = "Executing...";
            ButtonStyleHandler.Update(startClientButton, false, Color.Lavender);
            ButtonStyleHandler.Update(stopClientButton, true, Color.RosyBrown);
            ButtonStyleHandler.Update(resetClientButton, false, Color.Lavender);
            isLocalClientRunning = true;
            resetClientButton.Enabled = false;
        }

        private static Dictionary<int, UdpClient> StartForwardUdpClients(List<ListenerConfig> listeners, ClientProgressUpdater clientProgressUpdater)
        {
            Dictionary<int, UdpClient> forwardUdpClients = [];
            List<int> forwardPorts = [.. listeners.Where(l => l.ForwardPort != null).Select(l => l.ForwardPort.Value).Distinct()];
            foreach (var port in forwardPorts)
            {
                UdpClient forwardUdpClient = new();
                forwardUdpClient.Connect("localhost", port);
                forwardUdpClients[port] = forwardUdpClient;
                clientProgressUpdater.UpdateLog($"UDP client to forward to port {port} started");
            }
            return forwardUdpClients;
        }

        private void StartClientUdpListeners(List<ListenerConfig> listeners, Dictionary<int, UdpClient> forwardUdpClients, BlockingCollection<QsoMessage> qsoMessageQueue, ClientProgressUpdater clientProgressUpdater)
        {
            foreach (var listenerConfig in listeners)
            {
                UdpClient? forwardUdpClient = listenerConfig.ForwardPort == null ? null : forwardUdpClients[listenerConfig.ForwardPort.Value];
                StartClientUdpListener(listenerConfig, forwardUdpClient, qsoMessageQueue, clientProgressUpdater);
            }
        }

        private void StartTemporarelySavedQsoHandler(BlockingCollection<QsoMessage> qsoMessageQueue, QsoMessageSender qsoMessageHandler, ClientProgressUpdater progressUpdater)
        {
            CancellationTokenSource temporarelySavedQsoHandlerCancellationTokenSource = CreateLinkedClientCancellationTokenSource();
            var handler = new TemporarelySavedQsoHandler(dbRepository, qsoMessageQueue, qsoMessageHandler, progressUpdater, temporarelySavedQsoHandlerCancellationTokenSource);
            Task.Run(() => handler.Start());
        }

        private void StartClientUdpListener(ListenerConfig listenerConfig, UdpClient? forwardUdpClient, BlockingCollection<QsoMessage> qsoMessageQueue, ClientProgressUpdater clientProgressUpdater)
        {
            CancellationTokenSource clientUdpListenerCancellationTokenSource = CreateLinkedClientCancellationTokenSource();
            var listener = new UdpClientListener(listenerConfig, forwardUdpClient, qsoMessageQueue, clientProgressUpdater, clientUdpListenerCancellationTokenSource);
            Task.Run(() => listener.Start());
        }

        private CancellationTokenSource CreateLinkedClientCancellationTokenSource()
        {
            return CancellationTokenSource.CreateLinkedTokenSource(clientCancellationTokenSource.Token);
        }

        private CancellationTokenSource CreateLinkedServerCancellationTokenSource()
        {
            return CancellationTokenSource.CreateLinkedTokenSource(serverCancellationTokenSource.Token);
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
            clientCancellationTokenSource = RenewToken(clientCancellationTokenSource);
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
            ButtonStyleHandler.Update(resetClientButton, true, Color.LightGray);
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

        private void RestoreSettingsFromDb()
        {
            Dictionary<string, string?> settings = dbRepository.LoadSettings();
            log.Information("Restoring Main Form setting from DB: {settings}", settings);

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

        private void SaveSettingsToDB()
        {
            dbRepository.SaveSetting("ServerEnabled", enableServerCheckBox.Checked.ToString());
            dbRepository.SaveSetting("ServerPort", serverPortTextBox.Text);
            dbRepository.SaveSetting("ClientEnabled", enableClientCheckBox.Checked.ToString());
            dbRepository.SaveSetting("ClientServerNameIp", clientServerNameIpTextBox.Text);
            dbRepository.SaveSetting("ClientServerPort", clientServerPortTextBox.Text);
            dbRepository.SaveSetting("AutoStart", autoStartCheckbox.Checked.ToString());
            if (log.IsEnabled(Serilog.Events.LogEventLevel.Information))
            {
                log.Information("Saved Main Form setting to DB: {Settings}", dbRepository.LoadSettings());
            }
        }

        private void ListenersConfigButton_Click(object sender, EventArgs e)
        {
            new ListenersForm(dbRepository, isLocalClientRunning).ShowDialog(this);
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
                PopulateServerQsoAmountDataGridView();
                HandleExportEnabled();
            }
        }

        private void qsoImportButton_Click(object sender, EventArgs e)
        {
            new QsoImportForm(dbRepository).ShowDialog(this);
            if (enableServerCheckBox.Checked)
            {
                PopulateServerQsoAmountDataGridView();
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
            RegistryKey? rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            enableDebugWhenAutoStartCheckbox.Enabled = autoStartCheckbox.Checked;
            string? applicationName = Assembly.GetExecutingAssembly().GetName().Name;

            if (string.IsNullOrEmpty(applicationName))
            {
                log.Error("Application name is null or empty, cannot set auto-start registry key");
                return;
            }

            if (autoStartCheckbox.Checked)
            {
                log.Information("Enabling auto-start with Windows");
                string startupCommand = GetAppStartupCmd();
                log.Debug("Auto-start command: {startupCommand}", startupCommand);
                rk.SetValue(applicationName, startupCommand);
            }
            else
            {
                log.Information("Disabling auto-start with Windows if exists");
                rk.DeleteValue(applicationName, false);
            }
        }

        private string GetAppStartupCmd()
        {
            StringBuilder sb = new();
            sb.Append($"\"{Application.ExecutablePath}\"");
            if (enableServerCheckBox.Checked)
            {
                sb.Append(" --start-server");
            }
            if (enableClientCheckBox.Checked)
            {
                sb.Append(" --start-client");
            }
            if (enableClientCheckBox.Checked || enableServerCheckBox.Checked)
            {
                sb.Append(" --quiet");
                if (enableDebugWhenAutoStartCheckbox.Checked)
                {
                    sb.Append(" --debug");
                }
            }
            return sb.ToString();
        }

        private void resetClientButton_Click(object sender, EventArgs e)
        {
            log.Debug("Resetting Client Form called");
            SaveSettingsToDB();
            new ClientCleanupForm(dbRepository).ShowDialog(this);
            RestoreSettingsFromDb();
            HandleClientServerChanged();
        }

        private void enableDebugWhenAutoStartCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            autoStartCheckbox_CheckedChanged(autoStartCheckbox, EventArgs.Empty);
        }

        private void resetServerButton_Click(object sender, EventArgs e)
        {
            log.Debug("Resetting Server Form called");
            new ServerCleanupForm(dbRepository).ShowDialog(this);
            HandleServerCheckBoxChanged(enableServerCheckBox);
        }

        private void mainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            var tabControl = sender as TabControl;
            TabPage? selectedTab = tabControl?.SelectedTab;
            if (selectedTab == null) return;

            TextBox? textBox = selectedTab.Name switch
            {
                "clientTab" => clientLogTextBox,
                "serverTab" => serverLogTextBox,
                _ => null
            };

            if (textBox != null)
            {
                textBox.SelectionStart = textBox.Text.Length;
                textBox.ScrollToCaret();
            }
        }

        private void qsoAutoExportButton_Click(object sender, EventArgs e)
        {
            new QsoAutoExportForm(dbRepository).ShowDialog(this);
            StartAutoExportTask();
        }

        private void StartAutoExportTask()
        {
            StopAutoExportTask();
            autoExportTaskService = new(dbRepository);
            autoExportTaskService.Init();
            Task.Run(() => autoExportTaskService.Start());
        }

        private void StopAutoExportTask()
        {
            if (autoExportTaskService == null)
            {
                log.Debug("Auto export task service is not running, no need to stop");
                return;
            }
            autoExportTaskService.Stop();
        }

        private void CancelToken(CancellationTokenSource? tokenSource)
        {
            if (tokenSource == null || tokenSource.IsCancellationRequested)
            {
                log.Information("No cancellation token to be cancelled");
                return;
            }

            log.Information("Cancelling token {token}", tokenSource);
            tokenSource.Cancel();
            if (tokenSource.IsCancellationRequested)
            {
                tokenSource.Dispose();
            }
        }
        private CancellationTokenSource RenewToken(CancellationTokenSource? tokenSource)
        {
            CancelToken(tokenSource);
            return new CancellationTokenSource();
        }

        private void premiumCallsignsButton_Click(object sender, EventArgs e)
        {
            new PremiunCallsignsForm(dbRepository).ShowDialog(this);
        }
    }
}
