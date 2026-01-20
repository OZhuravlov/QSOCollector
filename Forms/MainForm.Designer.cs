using System.Data.SQLite;

namespace QSOCollector
{
    partial class QsoCollectorForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            mainTabControl = new TabControl();
            clientTab = new TabPage();
            clientServerCheckedAtLabel = new Label();
            clientServerStatusValueLabel = new Label();
            clientServerStatusLastCheckedLabel = new Label();
            clientServerStatusLabel = new Label();
            processingGroupBox = new GroupBox();
            resetClientButton = new Button();
            clientLogDetailsCheckBox = new CheckBox();
            clientLogDetailsLabel = new Label();
            clientQsoRejectedAtLabel = new Label();
            clientQsoTempSavedAtLabel = new Label();
            clientQsoSentToServerAtLabel = new Label();
            clientQsoReceviedAtLabel = new Label();
            clientQsoLastTimeLabel = new Label();
            clientQsoRejectedCountLabel = new Label();
            clientQsoRejectedLabel = new Label();
            clientQsoTempSavedCountLabel = new Label();
            clientLogTextBox = new TextBox();
            clientQsoTempSavedLabel = new Label();
            clientQsoSentToServerCountLabel = new Label();
            clientQsoSentToServerLabel = new Label();
            clientQsoReceivedCountLabel = new Label();
            clientQsoReceivedLabel = new Label();
            listenersConfigButton = new Button();
            ClientServerConfigGroupBox = new GroupBox();
            clientServerPortLabel = new Label();
            clientServerPortTextBox = new TextBox();
            clientServerNameIpTextBox = new TextBox();
            clientServerNameLabel = new Label();
            stopClientButton = new Button();
            startClientButton = new Button();
            enableClientCheckBox = new CheckBox();
            serverTab = new TabPage();
            resetServerButton = new Button();
            serverShowLogDetailsCheckBox = new CheckBox();
            qsoImportButton = new Button();
            serverProcessingGroupBox = new GroupBox();
            serverQsoAmountsDataGridView = new DataGridView();
            qsoAmountMode = new DataGridViewTextBoxColumn();
            todayQsoAmount = new DataGridViewTextBoxColumn();
            totalQsoAmount = new DataGridViewTextBoxColumn();
            exportedQsoAmount = new DataGridViewTextBoxColumn();
            lastQsoTime = new DataGridViewTextBoxColumn();
            lastExportedQsoTime = new DataGridViewTextBoxColumn();
            serverLogTextBox = new TextBox();
            stopServerButton = new Button();
            qsoExportButton = new Button();
            startServerButton = new Button();
            serverPortTextBox = new TextBox();
            serverPortLabel = new Label();
            enableServerCheckBox = new CheckBox();
            aboutTab = new TabPage();
            aboutTextBox = new TextBox();
            sqliteConnection1 = new Microsoft.Data.Sqlite.SqliteConnection();
            trayNotifyIcon = new NotifyIcon(components);
            autoStartCheckbox = new CheckBox();
            enableDebugWhenAutoStartCheckbox = new CheckBox();
            myToolTip = new ToolTip(components);
            mainTabControl.SuspendLayout();
            clientTab.SuspendLayout();
            processingGroupBox.SuspendLayout();
            ClientServerConfigGroupBox.SuspendLayout();
            serverTab.SuspendLayout();
            serverProcessingGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)serverQsoAmountsDataGridView).BeginInit();
            aboutTab.SuspendLayout();
            SuspendLayout();
            // 
            // mainTabControl
            // 
            mainTabControl.Controls.Add(clientTab);
            mainTabControl.Controls.Add(serverTab);
            mainTabControl.Controls.Add(aboutTab);
            mainTabControl.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            mainTabControl.Location = new Point(-1, 0);
            mainTabControl.Margin = new Padding(3, 2, 3, 2);
            mainTabControl.Name = "mainTabControl";
            mainTabControl.SelectedIndex = 0;
            mainTabControl.Size = new Size(690, 486);
            mainTabControl.TabIndex = 0;
            // 
            // clientTab
            // 
            clientTab.BackColor = Color.Transparent;
            clientTab.Controls.Add(clientServerCheckedAtLabel);
            clientTab.Controls.Add(clientServerStatusValueLabel);
            clientTab.Controls.Add(clientServerStatusLastCheckedLabel);
            clientTab.Controls.Add(clientServerStatusLabel);
            clientTab.Controls.Add(processingGroupBox);
            clientTab.Controls.Add(listenersConfigButton);
            clientTab.Controls.Add(ClientServerConfigGroupBox);
            clientTab.Controls.Add(stopClientButton);
            clientTab.Controls.Add(startClientButton);
            clientTab.Controls.Add(enableClientCheckBox);
            clientTab.Location = new Point(4, 28);
            clientTab.Margin = new Padding(3, 2, 3, 2);
            clientTab.Name = "clientTab";
            clientTab.Padding = new Padding(3, 2, 3, 2);
            clientTab.Size = new Size(682, 454);
            clientTab.TabIndex = 0;
            clientTab.Text = "Client";
            // 
            // clientServerCheckedAtLabel
            // 
            clientServerCheckedAtLabel.BackColor = Color.Transparent;
            clientServerCheckedAtLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientServerCheckedAtLabel.Location = new Point(526, 62);
            clientServerCheckedAtLabel.Name = "clientServerCheckedAtLabel";
            clientServerCheckedAtLabel.Size = new Size(145, 17);
            clientServerCheckedAtLabel.TabIndex = 10;
            clientServerCheckedAtLabel.Text = "---";
            clientServerCheckedAtLabel.TextAlign = ContentAlignment.MiddleCenter;
            myToolTip.SetToolTip(clientServerCheckedAtLabel, "when Server status was checked (UTC)");
            // 
            // clientServerStatusValueLabel
            // 
            clientServerStatusValueLabel.BackColor = Color.Thistle;
            clientServerStatusValueLabel.BorderStyle = BorderStyle.FixedSingle;
            clientServerStatusValueLabel.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            clientServerStatusValueLabel.Location = new Point(535, 19);
            clientServerStatusValueLabel.Name = "clientServerStatusValueLabel";
            clientServerStatusValueLabel.Size = new Size(129, 25);
            clientServerStatusValueLabel.TabIndex = 9;
            clientServerStatusValueLabel.Text = "Unknown";
            clientServerStatusValueLabel.TextAlign = ContentAlignment.MiddleCenter;
            myToolTip.SetToolTip(clientServerStatusValueLabel, "Last known QSOCollector Server status");
            // 
            // clientServerStatusLastCheckedLabel
            // 
            clientServerStatusLastCheckedLabel.AutoSize = true;
            clientServerStatusLastCheckedLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            clientServerStatusLastCheckedLabel.Location = new Point(560, 47);
            clientServerStatusLastCheckedLabel.Name = "clientServerStatusLastCheckedLabel";
            clientServerStatusLastCheckedLabel.Size = new Size(65, 15);
            clientServerStatusLastCheckedLabel.TabIndex = 8;
            clientServerStatusLastCheckedLabel.Text = "Checked at";
            myToolTip.SetToolTip(clientServerStatusLastCheckedLabel, "when Server status was checked (UTC)");
            // 
            // clientServerStatusLabel
            // 
            clientServerStatusLabel.AutoSize = true;
            clientServerStatusLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            clientServerStatusLabel.Location = new Point(554, 4);
            clientServerStatusLabel.Name = "clientServerStatusLabel";
            clientServerStatusLabel.Size = new Size(76, 15);
            clientServerStatusLabel.TabIndex = 7;
            clientServerStatusLabel.Text = "Server Status";
            myToolTip.SetToolTip(clientServerStatusLabel, "QSOCollector Server status");
            // 
            // processingGroupBox
            // 
            processingGroupBox.Controls.Add(resetClientButton);
            processingGroupBox.Controls.Add(clientLogDetailsCheckBox);
            processingGroupBox.Controls.Add(clientLogDetailsLabel);
            processingGroupBox.Controls.Add(clientQsoRejectedAtLabel);
            processingGroupBox.Controls.Add(clientQsoTempSavedAtLabel);
            processingGroupBox.Controls.Add(clientQsoSentToServerAtLabel);
            processingGroupBox.Controls.Add(clientQsoReceviedAtLabel);
            processingGroupBox.Controls.Add(clientQsoLastTimeLabel);
            processingGroupBox.Controls.Add(clientQsoRejectedCountLabel);
            processingGroupBox.Controls.Add(clientQsoRejectedLabel);
            processingGroupBox.Controls.Add(clientQsoTempSavedCountLabel);
            processingGroupBox.Controls.Add(clientLogTextBox);
            processingGroupBox.Controls.Add(clientQsoTempSavedLabel);
            processingGroupBox.Controls.Add(clientQsoSentToServerCountLabel);
            processingGroupBox.Controls.Add(clientQsoSentToServerLabel);
            processingGroupBox.Controls.Add(clientQsoReceivedCountLabel);
            processingGroupBox.Controls.Add(clientQsoReceivedLabel);
            processingGroupBox.Location = new Point(0, 77);
            processingGroupBox.Margin = new Padding(3, 2, 3, 2);
            processingGroupBox.Name = "processingGroupBox";
            processingGroupBox.Padding = new Padding(3, 2, 3, 2);
            processingGroupBox.Size = new Size(681, 377);
            processingGroupBox.TabIndex = 6;
            processingGroupBox.TabStop = false;
            processingGroupBox.Text = "QSO processing";
            // 
            // resetClientButton
            // 
            resetClientButton.BackColor = Color.LightGray;
            resetClientButton.FlatStyle = FlatStyle.Popup;
            resetClientButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            resetClientButton.Location = new Point(502, 23);
            resetClientButton.Margin = new Padding(3, 2, 3, 2);
            resetClientButton.Name = "resetClientButton";
            resetClientButton.Size = new Size(95, 28);
            resetClientButton.TabIndex = 11;
            resetClientButton.Text = "Reset Client";
            myToolTip.SetToolTip(resetClientButton, "This helps to reset Client partially or completely");
            resetClientButton.UseVisualStyleBackColor = false;
            resetClientButton.Click += resetClientButton_Click;
            // 
            // clientLogDetailsCheckBox
            // 
            clientLogDetailsCheckBox.AutoSize = true;
            clientLogDetailsCheckBox.Location = new Point(631, 37);
            clientLogDetailsCheckBox.Margin = new Padding(3, 2, 3, 2);
            clientLogDetailsCheckBox.Name = "clientLogDetailsCheckBox";
            clientLogDetailsCheckBox.Size = new Size(15, 14);
            clientLogDetailsCheckBox.TabIndex = 20;
            myToolTip.SetToolTip(clientLogDetailsCheckBox, "Enables more detailed log");
            clientLogDetailsCheckBox.UseVisualStyleBackColor = true;
            clientLogDetailsCheckBox.CheckedChanged += clientLogDetailsCheckBox_CheckedChanged;
            // 
            // clientLogDetailsLabel
            // 
            clientLogDetailsLabel.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientLogDetailsLabel.Location = new Point(603, 6);
            clientLogDetailsLabel.Name = "clientLogDetailsLabel";
            clientLogDetailsLabel.Size = new Size(70, 29);
            clientLogDetailsLabel.TabIndex = 19;
            clientLogDetailsLabel.Text = "Log details";
            clientLogDetailsLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // clientQsoRejectedAtLabel
            // 
            clientQsoRejectedAtLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoRejectedAtLabel.Location = new Point(397, 44);
            clientQsoRejectedAtLabel.Name = "clientQsoRejectedAtLabel";
            clientQsoRejectedAtLabel.Size = new Size(87, 15);
            clientQsoRejectedAtLabel.TabIndex = 18;
            clientQsoRejectedAtLabel.Text = "---";
            clientQsoRejectedAtLabel.TextAlign = ContentAlignment.MiddleCenter;
            myToolTip.SetToolTip(clientQsoRejectedAtLabel, "When last time (UTC) QSO qas rejected amount rejected by Client because of incorrect format");
            // 
            // clientQsoTempSavedAtLabel
            // 
            clientQsoTempSavedAtLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoTempSavedAtLabel.Location = new Point(294, 44);
            clientQsoTempSavedAtLabel.Name = "clientQsoTempSavedAtLabel";
            clientQsoTempSavedAtLabel.Size = new Size(79, 15);
            clientQsoTempSavedAtLabel.TabIndex = 17;
            clientQsoTempSavedAtLabel.Text = "---";
            clientQsoTempSavedAtLabel.TextAlign = ContentAlignment.MiddleCenter;
            myToolTip.SetToolTip(clientQsoTempSavedAtLabel, "When last time (UTC) QSO temporarely saved on Client side (because Server was not available)");
            // 
            // clientQsoSentToServerAtLabel
            // 
            clientQsoSentToServerAtLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoSentToServerAtLabel.Location = new Point(174, 44);
            clientQsoSentToServerAtLabel.Name = "clientQsoSentToServerAtLabel";
            clientQsoSentToServerAtLabel.Size = new Size(67, 15);
            clientQsoSentToServerAtLabel.TabIndex = 16;
            clientQsoSentToServerAtLabel.Text = "---";
            clientQsoSentToServerAtLabel.TextAlign = ContentAlignment.MiddleCenter;
            myToolTip.SetToolTip(clientQsoSentToServerAtLabel, "When last time (UTC) QSO send to QSOCollector Server");
            // 
            // clientQsoReceviedAtLabel
            // 
            clientQsoReceviedAtLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoReceviedAtLabel.Location = new Point(60, 44);
            clientQsoReceviedAtLabel.Name = "clientQsoReceviedAtLabel";
            clientQsoReceviedAtLabel.Size = new Size(66, 15);
            clientQsoReceviedAtLabel.TabIndex = 15;
            clientQsoReceviedAtLabel.Text = "---";
            clientQsoReceviedAtLabel.TextAlign = ContentAlignment.MiddleCenter;
            myToolTip.SetToolTip(clientQsoReceviedAtLabel, "When last time (UTC) QSO from logger(s) received");
            // 
            // clientQsoLastTimeLabel
            // 
            clientQsoLastTimeLabel.AutoSize = true;
            clientQsoLastTimeLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoLastTimeLabel.Location = new Point(8, 44);
            clientQsoLastTimeLabel.Name = "clientQsoLastTimeLabel";
            clientQsoLastTimeLabel.Size = new Size(41, 15);
            clientQsoLastTimeLabel.TabIndex = 14;
            clientQsoLastTimeLabel.Text = "Last at";
            // 
            // clientQsoRejectedCountLabel
            // 
            clientQsoRejectedCountLabel.AutoSize = true;
            clientQsoRejectedCountLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoRejectedCountLabel.Location = new Point(468, 22);
            clientQsoRejectedCountLabel.Name = "clientQsoRejectedCountLabel";
            clientQsoRejectedCountLabel.Size = new Size(13, 15);
            clientQsoRejectedCountLabel.TabIndex = 13;
            clientQsoRejectedCountLabel.Text = "0";
            myToolTip.SetToolTip(clientQsoRejectedCountLabel, "QSO amount rejected by Client (since started) because of incorrect format");
            // 
            // clientQsoRejectedLabel
            // 
            clientQsoRejectedLabel.AutoSize = true;
            clientQsoRejectedLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoRejectedLabel.Location = new Point(397, 22);
            clientQsoRejectedLabel.Name = "clientQsoRejectedLabel";
            clientQsoRejectedLabel.Size = new Size(55, 15);
            clientQsoRejectedLabel.TabIndex = 12;
            clientQsoRejectedLabel.Text = "Rejected:";
            myToolTip.SetToolTip(clientQsoRejectedLabel, "QSOs rejected by clien because of incorrect format");
            // 
            // clientQsoTempSavedCountLabel
            // 
            clientQsoTempSavedCountLabel.AutoSize = true;
            clientQsoTempSavedCountLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoTempSavedCountLabel.Location = new Point(369, 22);
            clientQsoTempSavedCountLabel.Name = "clientQsoTempSavedCountLabel";
            clientQsoTempSavedCountLabel.Size = new Size(13, 15);
            clientQsoTempSavedCountLabel.TabIndex = 11;
            clientQsoTempSavedCountLabel.Text = "0";
            myToolTip.SetToolTip(clientQsoTempSavedCountLabel, "QSO amount temporarely saved on Client (because Server was not available)");
            // 
            // clientLogTextBox
            // 
            clientLogTextBox.BackColor = SystemColors.Window;
            clientLogTextBox.Font = new Font("Consolas", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            clientLogTextBox.ForeColor = SystemColors.InfoText;
            clientLogTextBox.ImeMode = ImeMode.NoControl;
            clientLogTextBox.Location = new Point(0, 61);
            clientLogTextBox.Margin = new Padding(3, 2, 3, 2);
            clientLogTextBox.MaxLength = 2147483646;
            clientLogTextBox.Multiline = true;
            clientLogTextBox.Name = "clientLogTextBox";
            clientLogTextBox.PlaceholderText = "Client logs will be here ...";
            clientLogTextBox.ReadOnly = true;
            clientLogTextBox.ScrollBars = ScrollBars.Both;
            clientLogTextBox.Size = new Size(679, 320);
            clientLogTextBox.TabIndex = 5;
            // 
            // clientQsoTempSavedLabel
            // 
            clientQsoTempSavedLabel.AutoSize = true;
            clientQsoTempSavedLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoTempSavedLabel.Location = new Point(244, 22);
            clientQsoTempSavedLabel.Name = "clientQsoTempSavedLabel";
            clientQsoTempSavedLabel.Size = new Size(108, 15);
            clientQsoTempSavedLabel.TabIndex = 10;
            clientQsoTempSavedLabel.Text = "Temporarely saved:";
            myToolTip.SetToolTip(clientQsoTempSavedLabel, "QSOs temporarely saved on Client (because Server was not available)");
            // 
            // clientQsoSentToServerCountLabel
            // 
            clientQsoSentToServerCountLabel.AutoSize = true;
            clientQsoSentToServerCountLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoSentToServerCountLabel.Location = new Point(213, 22);
            clientQsoSentToServerCountLabel.Name = "clientQsoSentToServerCountLabel";
            clientQsoSentToServerCountLabel.Size = new Size(13, 15);
            clientQsoSentToServerCountLabel.TabIndex = 9;
            clientQsoSentToServerCountLabel.Text = "0";
            myToolTip.SetToolTip(clientQsoSentToServerCountLabel, "QSO amount sent to QSOColector Server since Client started");
            // 
            // clientQsoSentToServerLabel
            // 
            clientQsoSentToServerLabel.AutoSize = true;
            clientQsoSentToServerLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoSentToServerLabel.Location = new Point(121, 22);
            clientQsoSentToServerLabel.Name = "clientQsoSentToServerLabel";
            clientQsoSentToServerLabel.Size = new Size(82, 15);
            clientQsoSentToServerLabel.TabIndex = 8;
            clientQsoSentToServerLabel.Text = "Sent to Server:";
            myToolTip.SetToolTip(clientQsoSentToServerLabel, "QSOs sent to QSOColector Server since Client started");
            // 
            // clientQsoReceivedCountLabel
            // 
            clientQsoReceivedCountLabel.AutoSize = true;
            clientQsoReceivedCountLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoReceivedCountLabel.Location = new Point(68, 22);
            clientQsoReceivedCountLabel.Name = "clientQsoReceivedCountLabel";
            clientQsoReceivedCountLabel.Size = new Size(13, 15);
            clientQsoReceivedCountLabel.TabIndex = 7;
            clientQsoReceivedCountLabel.Text = "0";
            myToolTip.SetToolTip(clientQsoReceivedCountLabel, "QSO amount received from logger(s) running on the currect PC");
            // 
            // clientQsoReceivedLabel
            // 
            clientQsoReceivedLabel.AutoSize = true;
            clientQsoReceivedLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoReceivedLabel.Location = new Point(8, 22);
            clientQsoReceivedLabel.Name = "clientQsoReceivedLabel";
            clientQsoReceivedLabel.Size = new Size(57, 15);
            clientQsoReceivedLabel.TabIndex = 6;
            clientQsoReceivedLabel.Text = "Received:";
            myToolTip.SetToolTip(clientQsoReceivedLabel, "QSOs received from logger(s) running on the currect PC");
            // 
            // listenersConfigButton
            // 
            listenersConfigButton.BackColor = Color.SlateGray;
            listenersConfigButton.FlatStyle = FlatStyle.Popup;
            listenersConfigButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            listenersConfigButton.Location = new Point(8, 38);
            listenersConfigButton.Margin = new Padding(3, 2, 3, 2);
            listenersConfigButton.Name = "listenersConfigButton";
            listenersConfigButton.Size = new Size(113, 35);
            listenersConfigButton.TabIndex = 4;
            listenersConfigButton.Text = "UDP Listeners";
            myToolTip.SetToolTip(listenersConfigButton, "Local QSO listeners configuration");
            listenersConfigButton.UseVisualStyleBackColor = false;
            listenersConfigButton.Click += ListenersConfigButton_Click;
            // 
            // ClientServerConfigGroupBox
            // 
            ClientServerConfigGroupBox.Controls.Add(clientServerPortLabel);
            ClientServerConfigGroupBox.Controls.Add(clientServerPortTextBox);
            ClientServerConfigGroupBox.Controls.Add(clientServerNameIpTextBox);
            ClientServerConfigGroupBox.Controls.Add(clientServerNameLabel);
            ClientServerConfigGroupBox.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ClientServerConfigGroupBox.Location = new Point(126, 4);
            ClientServerConfigGroupBox.Margin = new Padding(3, 2, 3, 2);
            ClientServerConfigGroupBox.Name = "ClientServerConfigGroupBox";
            ClientServerConfigGroupBox.Padding = new Padding(3, 2, 3, 2);
            ClientServerConfigGroupBox.Size = new Size(201, 68);
            ClientServerConfigGroupBox.TabIndex = 3;
            ClientServerConfigGroupBox.TabStop = false;
            ClientServerConfigGroupBox.Text = "Server config";
            // 
            // clientServerPortLabel
            // 
            clientServerPortLabel.AutoSize = true;
            clientServerPortLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientServerPortLabel.Location = new Point(5, 43);
            clientServerPortLabel.Name = "clientServerPortLabel";
            clientServerPortLabel.Size = new Size(29, 15);
            clientServerPortLabel.TabIndex = 3;
            clientServerPortLabel.Text = "Port";
            // 
            // clientServerPortTextBox
            // 
            clientServerPortTextBox.Location = new Point(78, 43);
            clientServerPortTextBox.Margin = new Padding(3, 2, 3, 2);
            clientServerPortTextBox.Name = "clientServerPortTextBox";
            clientServerPortTextBox.Size = new Size(62, 23);
            clientServerPortTextBox.TabIndex = 2;
            myToolTip.SetToolTip(clientServerPortTextBox, "Port used by QSO Collector Server");
            clientServerPortTextBox.TextChanged += clientServerPortTextBox_TextChanged;
            clientServerPortTextBox.KeyPress += PortTextBox_KeyPress;
            // 
            // clientServerNameIpTextBox
            // 
            clientServerNameIpTextBox.Location = new Point(79, 19);
            clientServerNameIpTextBox.Margin = new Padding(3, 2, 3, 2);
            clientServerNameIpTextBox.MaxLength = 200;
            clientServerNameIpTextBox.Name = "clientServerNameIpTextBox";
            clientServerNameIpTextBox.Size = new Size(112, 23);
            clientServerNameIpTextBox.TabIndex = 1;
            myToolTip.SetToolTip(clientServerNameIpTextBox, "IP Address of PC where QSO Collector is running in Server mode");
            clientServerNameIpTextBox.TextChanged += clientServerNameIpTextBox_TextChanged;
            // 
            // clientServerNameLabel
            // 
            clientServerNameLabel.AutoSize = true;
            clientServerNameLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientServerNameLabel.Location = new Point(5, 21);
            clientServerNameLabel.Name = "clientServerNameLabel";
            clientServerNameLabel.Size = new Size(62, 15);
            clientServerNameLabel.TabIndex = 0;
            clientServerNameLabel.Text = "IP Address";
            // 
            // stopClientButton
            // 
            stopClientButton.BackColor = Color.RosyBrown;
            stopClientButton.Enabled = false;
            stopClientButton.FlatStyle = FlatStyle.Popup;
            stopClientButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            stopClientButton.Location = new Point(411, 47);
            stopClientButton.Margin = new Padding(3, 2, 3, 2);
            stopClientButton.Name = "stopClientButton";
            stopClientButton.Size = new Size(97, 26);
            stopClientButton.TabIndex = 2;
            stopClientButton.Text = "Stop Client";
            myToolTip.SetToolTip(stopClientButton, "Stops QSO listeners");
            stopClientButton.UseVisualStyleBackColor = false;
            stopClientButton.Click += StopClientButton_Click;
            // 
            // startClientButton
            // 
            startClientButton.BackColor = Color.DarkSeaGreen;
            startClientButton.FlatStyle = FlatStyle.Popup;
            startClientButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            startClientButton.Location = new Point(411, 13);
            startClientButton.Margin = new Padding(3, 2, 3, 2);
            startClientButton.Name = "startClientButton";
            startClientButton.Size = new Size(97, 27);
            startClientButton.TabIndex = 1;
            startClientButton.Text = "Start Client";
            myToolTip.SetToolTip(startClientButton, "Starts listening for all ports (loggers) configured in \"UPD Listeners\"");
            startClientButton.UseVisualStyleBackColor = false;
            startClientButton.Click += StartClientButton_Click;
            // 
            // enableClientCheckBox
            // 
            enableClientCheckBox.AutoSize = true;
            enableClientCheckBox.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            enableClientCheckBox.Location = new Point(8, 14);
            enableClientCheckBox.Margin = new Padding(3, 2, 3, 2);
            enableClientCheckBox.Name = "enableClientCheckBox";
            enableClientCheckBox.Size = new Size(62, 19);
            enableClientCheckBox.TabIndex = 0;
            enableClientCheckBox.Text = "Enable";
            myToolTip.SetToolTip(enableClientCheckBox, "Enables Client mode");
            enableClientCheckBox.UseVisualStyleBackColor = true;
            enableClientCheckBox.CheckedChanged += EnableClientCheckBox_CheckedChanged;
            // 
            // serverTab
            // 
            serverTab.Controls.Add(resetServerButton);
            serverTab.Controls.Add(serverShowLogDetailsCheckBox);
            serverTab.Controls.Add(qsoImportButton);
            serverTab.Controls.Add(serverProcessingGroupBox);
            serverTab.Controls.Add(stopServerButton);
            serverTab.Controls.Add(qsoExportButton);
            serverTab.Controls.Add(startServerButton);
            serverTab.Controls.Add(serverPortTextBox);
            serverTab.Controls.Add(serverPortLabel);
            serverTab.Controls.Add(enableServerCheckBox);
            serverTab.Location = new Point(4, 28);
            serverTab.Margin = new Padding(3, 2, 3, 2);
            serverTab.Name = "serverTab";
            serverTab.Padding = new Padding(3, 2, 3, 2);
            serverTab.Size = new Size(682, 454);
            serverTab.TabIndex = 1;
            serverTab.Text = "Server";
            serverTab.UseVisualStyleBackColor = true;
            // 
            // resetServerButton
            // 
            resetServerButton.BackColor = Color.LightGray;
            resetServerButton.Enabled = false;
            resetServerButton.FlatStyle = FlatStyle.Popup;
            resetServerButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            resetServerButton.Location = new Point(578, 10);
            resetServerButton.Margin = new Padding(3, 2, 3, 2);
            resetServerButton.Name = "resetServerButton";
            resetServerButton.Size = new Size(98, 25);
            resetServerButton.TabIndex = 12;
            resetServerButton.Text = "Reset Server";
            myToolTip.SetToolTip(resetServerButton, "This helps to reset Server completely. Use only before Expedition starts to delete previous dxpedition data or to remove  test data");
            resetServerButton.UseVisualStyleBackColor = false;
            resetServerButton.Click += resetServerButton_Click;
            // 
            // serverShowLogDetailsCheckBox
            // 
            serverShowLogDetailsCheckBox.AutoSize = true;
            serverShowLogDetailsCheckBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            serverShowLogDetailsCheckBox.Location = new Point(5, 424);
            serverShowLogDetailsCheckBox.Margin = new Padding(3, 2, 3, 2);
            serverShowLogDetailsCheckBox.Name = "serverShowLogDetailsCheckBox";
            serverShowLogDetailsCheckBox.Size = new Size(115, 19);
            serverShowLogDetailsCheckBox.TabIndex = 8;
            serverShowLogDetailsCheckBox.Text = "Show Log details";
            serverShowLogDetailsCheckBox.UseVisualStyleBackColor = true;
            serverShowLogDetailsCheckBox.CheckedChanged += serverShowLogDetailsCheckBox_CheckedChanged;
            // 
            // qsoImportButton
            // 
            qsoImportButton.BackColor = Color.Transparent;
            qsoImportButton.Enabled = false;
            qsoImportButton.FlatStyle = FlatStyle.System;
            qsoImportButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            qsoImportButton.Location = new Point(437, 420);
            qsoImportButton.Margin = new Padding(3, 2, 3, 2);
            qsoImportButton.Name = "qsoImportButton";
            qsoImportButton.Size = new Size(84, 30);
            qsoImportButton.TabIndex = 1;
            qsoImportButton.Text = "QSO Import";
            qsoImportButton.UseVisualStyleBackColor = false;
            qsoImportButton.Click += qsoImportButton_Click;
            // 
            // serverProcessingGroupBox
            // 
            serverProcessingGroupBox.Controls.Add(serverQsoAmountsDataGridView);
            serverProcessingGroupBox.Controls.Add(serverLogTextBox);
            serverProcessingGroupBox.Location = new Point(3, 38);
            serverProcessingGroupBox.Margin = new Padding(3, 2, 3, 2);
            serverProcessingGroupBox.Name = "serverProcessingGroupBox";
            serverProcessingGroupBox.Padding = new Padding(3, 2, 3, 2);
            serverProcessingGroupBox.Size = new Size(678, 382);
            serverProcessingGroupBox.TabIndex = 7;
            serverProcessingGroupBox.TabStop = false;
            serverProcessingGroupBox.Text = "Processing";
            // 
            // serverQsoAmountsDataGridView
            // 
            serverQsoAmountsDataGridView.AllowUserToAddRows = false;
            serverQsoAmountsDataGridView.AllowUserToDeleteRows = false;
            serverQsoAmountsDataGridView.AllowUserToResizeColumns = false;
            serverQsoAmountsDataGridView.AllowUserToResizeRows = false;
            serverQsoAmountsDataGridView.BackgroundColor = SystemColors.Control;
            serverQsoAmountsDataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = SystemColors.Control;
            dataGridViewCellStyle4.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle4.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.True;
            serverQsoAmountsDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            serverQsoAmountsDataGridView.ColumnHeadersHeight = 29;
            serverQsoAmountsDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            serverQsoAmountsDataGridView.Columns.AddRange(new DataGridViewColumn[] { qsoAmountMode, todayQsoAmount, totalQsoAmount, exportedQsoAmount, lastQsoTime, lastExportedQsoTime });
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = SystemColors.Window;
            dataGridViewCellStyle5.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle5.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = DataGridViewTriState.False;
            serverQsoAmountsDataGridView.DefaultCellStyle = dataGridViewCellStyle5;
            serverQsoAmountsDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            serverQsoAmountsDataGridView.Location = new Point(3, 22);
            serverQsoAmountsDataGridView.Margin = new Padding(3, 2, 3, 2);
            serverQsoAmountsDataGridView.Name = "serverQsoAmountsDataGridView";
            serverQsoAmountsDataGridView.ReadOnly = true;
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = SystemColors.Control;
            dataGridViewCellStyle6.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle6.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = DataGridViewTriState.False;
            serverQsoAmountsDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            serverQsoAmountsDataGridView.RowHeadersWidth = 51;
            serverQsoAmountsDataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            serverQsoAmountsDataGridView.RowTemplate.ReadOnly = true;
            serverQsoAmountsDataGridView.RowTemplate.Resizable = DataGridViewTriState.False;
            serverQsoAmountsDataGridView.ShowEditingIcon = false;
            serverQsoAmountsDataGridView.Size = new Size(670, 148);
            serverQsoAmountsDataGridView.TabIndex = 7;
            serverQsoAmountsDataGridView.RowsAdded += serverQsoAmountsDataGridView_RowsAdded;
            serverQsoAmountsDataGridView.RowsRemoved += serverQsoAmountsDataGridView_RowsRemoved;
            // 
            // qsoAmountMode
            // 
            qsoAmountMode.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            qsoAmountMode.DataPropertyName = "QsoAmountMode";
            qsoAmountMode.Frozen = true;
            qsoAmountMode.HeaderText = "Mode";
            qsoAmountMode.MaxInputLength = 8;
            qsoAmountMode.MinimumWidth = 6;
            qsoAmountMode.Name = "qsoAmountMode";
            qsoAmountMode.ReadOnly = true;
            qsoAmountMode.Resizable = DataGridViewTriState.False;
            qsoAmountMode.SortMode = DataGridViewColumnSortMode.NotSortable;
            qsoAmountMode.Width = 60;
            // 
            // todayQsoAmount
            // 
            todayQsoAmount.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            todayQsoAmount.DataPropertyName = "TodayQsoAmount";
            todayQsoAmount.Frozen = true;
            todayQsoAmount.HeaderText = "Today QSO";
            todayQsoAmount.MaxInputLength = 5;
            todayQsoAmount.MinimumWidth = 6;
            todayQsoAmount.Name = "todayQsoAmount";
            todayQsoAmount.ReadOnly = true;
            todayQsoAmount.SortMode = DataGridViewColumnSortMode.NotSortable;
            todayQsoAmount.Width = 80;
            // 
            // totalQsoAmount
            // 
            totalQsoAmount.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            totalQsoAmount.DataPropertyName = "TotalQsoAmount";
            totalQsoAmount.Frozen = true;
            totalQsoAmount.HeaderText = "Total QSO";
            totalQsoAmount.MaxInputLength = 6;
            totalQsoAmount.MinimumWidth = 6;
            totalQsoAmount.Name = "totalQsoAmount";
            totalQsoAmount.ReadOnly = true;
            totalQsoAmount.SortMode = DataGridViewColumnSortMode.NotSortable;
            totalQsoAmount.Width = 80;
            // 
            // exportedQsoAmount
            // 
            exportedQsoAmount.DataPropertyName = "ExportedQsoAmount";
            exportedQsoAmount.Frozen = true;
            exportedQsoAmount.HeaderText = "Exported QSOs";
            exportedQsoAmount.MaxInputLength = 6;
            exportedQsoAmount.MinimumWidth = 6;
            exportedQsoAmount.Name = "exportedQsoAmount";
            exportedQsoAmount.ReadOnly = true;
            exportedQsoAmount.Resizable = DataGridViewTriState.False;
            exportedQsoAmount.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // lastQsoTime
            // 
            lastQsoTime.DataPropertyName = "LastQsoTime";
            lastQsoTime.Frozen = true;
            lastQsoTime.HeaderText = "Latest Logged QSO";
            lastQsoTime.MaxInputLength = 20;
            lastQsoTime.MinimumWidth = 6;
            lastQsoTime.Name = "lastQsoTime";
            lastQsoTime.ReadOnly = true;
            lastQsoTime.Resizable = DataGridViewTriState.False;
            lastQsoTime.SortMode = DataGridViewColumnSortMode.NotSortable;
            lastQsoTime.Width = 160;
            // 
            // lastExportedQsoTime
            // 
            lastExportedQsoTime.DataPropertyName = "LastExportedQsoTime";
            lastExportedQsoTime.Frozen = true;
            lastExportedQsoTime.HeaderText = "Latest Exported QSO";
            lastExportedQsoTime.MaxInputLength = 20;
            lastExportedQsoTime.MinimumWidth = 6;
            lastExportedQsoTime.Name = "lastExportedQsoTime";
            lastExportedQsoTime.ReadOnly = true;
            lastExportedQsoTime.Resizable = DataGridViewTriState.False;
            lastExportedQsoTime.SortMode = DataGridViewColumnSortMode.NotSortable;
            lastExportedQsoTime.Width = 160;
            // 
            // serverLogTextBox
            // 
            serverLogTextBox.BackColor = SystemColors.Window;
            serverLogTextBox.Enabled = false;
            serverLogTextBox.Font = new Font("Consolas", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            serverLogTextBox.ForeColor = SystemColors.WindowText;
            serverLogTextBox.Location = new Point(3, 174);
            serverLogTextBox.Margin = new Padding(3, 2, 3, 2);
            serverLogTextBox.Multiline = true;
            serverLogTextBox.Name = "serverLogTextBox";
            serverLogTextBox.PlaceholderText = "Server logs will be here ...";
            serverLogTextBox.ReadOnly = true;
            serverLogTextBox.ScrollBars = ScrollBars.Vertical;
            serverLogTextBox.Size = new Size(676, 204);
            serverLogTextBox.TabIndex = 6;
            // 
            // stopServerButton
            // 
            stopServerButton.AutoSize = true;
            stopServerButton.BackColor = Color.RosyBrown;
            stopServerButton.Enabled = false;
            stopServerButton.FlatStyle = FlatStyle.Popup;
            stopServerButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            stopServerButton.Location = new Point(447, 10);
            stopServerButton.Margin = new Padding(3, 2, 3, 2);
            stopServerButton.Name = "stopServerButton";
            stopServerButton.Size = new Size(95, 25);
            stopServerButton.TabIndex = 5;
            stopServerButton.Text = "Stop Server";
            stopServerButton.UseVisualStyleBackColor = false;
            stopServerButton.Click += StopServerButton_Click;
            // 
            // qsoExportButton
            // 
            qsoExportButton.BackColor = Color.Transparent;
            qsoExportButton.Enabled = false;
            qsoExportButton.FlatStyle = FlatStyle.System;
            qsoExportButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            qsoExportButton.ForeColor = SystemColors.ControlText;
            qsoExportButton.Location = new Point(270, 420);
            qsoExportButton.Margin = new Padding(3, 2, 3, 2);
            qsoExportButton.Name = "qsoExportButton";
            qsoExportButton.Size = new Size(96, 30);
            qsoExportButton.TabIndex = 0;
            qsoExportButton.Text = "QSO Export";
            qsoExportButton.UseVisualStyleBackColor = false;
            qsoExportButton.Click += qsoExportButton_Click;
            // 
            // startServerButton
            // 
            startServerButton.AutoSize = true;
            startServerButton.BackColor = Color.DarkSeaGreen;
            startServerButton.Enabled = false;
            startServerButton.FlatStyle = FlatStyle.Popup;
            startServerButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            startServerButton.ForeColor = Color.Black;
            startServerButton.Location = new Point(340, 10);
            startServerButton.Margin = new Padding(3, 2, 3, 2);
            startServerButton.Name = "startServerButton";
            startServerButton.Size = new Size(94, 25);
            startServerButton.TabIndex = 3;
            startServerButton.Text = "Start Server";
            startServerButton.UseVisualStyleBackColor = false;
            startServerButton.Click += StartServerButton_Click;
            // 
            // serverPortTextBox
            // 
            serverPortTextBox.Enabled = false;
            serverPortTextBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            serverPortTextBox.Location = new Point(249, 14);
            serverPortTextBox.Margin = new Padding(3, 2, 3, 2);
            serverPortTextBox.Name = "serverPortTextBox";
            serverPortTextBox.PlaceholderText = "(port number)";
            serverPortTextBox.Size = new Size(72, 23);
            serverPortTextBox.TabIndex = 2;
            serverPortTextBox.TextChanged += ServerPortTextBox_TextChanged;
            serverPortTextBox.KeyPress += PortTextBox_KeyPress;
            // 
            // serverPortLabel
            // 
            serverPortLabel.AutoSize = true;
            serverPortLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            serverPortLabel.Location = new Point(96, 16);
            serverPortLabel.Name = "serverPortLabel";
            serverPortLabel.Size = new Size(137, 15);
            serverPortLabel.TabIndex = 1;
            serverPortLabel.Text = "Listen for Clients on Port";
            // 
            // enableServerCheckBox
            // 
            enableServerCheckBox.AutoSize = true;
            enableServerCheckBox.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            enableServerCheckBox.Location = new Point(16, 15);
            enableServerCheckBox.Margin = new Padding(3, 2, 3, 2);
            enableServerCheckBox.Name = "enableServerCheckBox";
            enableServerCheckBox.Size = new Size(62, 19);
            enableServerCheckBox.TabIndex = 0;
            enableServerCheckBox.Text = "Enable";
            enableServerCheckBox.UseVisualStyleBackColor = true;
            enableServerCheckBox.CheckedChanged += EnableServerCheckBox_CheckedChanged;
            // 
            // aboutTab
            // 
            aboutTab.Controls.Add(aboutTextBox);
            aboutTab.Location = new Point(4, 28);
            aboutTab.Margin = new Padding(3, 2, 3, 2);
            aboutTab.Name = "aboutTab";
            aboutTab.Size = new Size(682, 454);
            aboutTab.TabIndex = 2;
            aboutTab.Text = "About";
            aboutTab.UseVisualStyleBackColor = true;
            // 
            // aboutTextBox
            // 
            aboutTextBox.BackColor = SystemColors.Window;
            aboutTextBox.Font = new Font("Consolas", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            aboutTextBox.ForeColor = SystemColors.WindowText;
            aboutTextBox.Location = new Point(6, 14);
            aboutTextBox.Margin = new Padding(3, 2, 3, 2);
            aboutTextBox.MaxLength = 1000000;
            aboutTextBox.Multiline = true;
            aboutTextBox.Name = "aboutTextBox";
            aboutTextBox.ReadOnly = true;
            aboutTextBox.ScrollBars = ScrollBars.Vertical;
            aboutTextBox.Size = new Size(616, 402);
            aboutTextBox.TabIndex = 7;
            // 
            // sqliteConnection1
            // 
            sqliteConnection1.DefaultTimeout = 30;
            // 
            // trayNotifyIcon
            // 
            trayNotifyIcon.Text = "QSO Collector running";
            trayNotifyIcon.Visible = true;
            trayNotifyIcon.DoubleClick += trayNotifyIcon_DoubleClick;
            // 
            // autoStartCheckbox
            // 
            autoStartCheckbox.AutoSize = true;
            autoStartCheckbox.Checked = true;
            autoStartCheckbox.CheckState = CheckState.Checked;
            autoStartCheckbox.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            autoStartCheckbox.Location = new Point(397, 4);
            autoStartCheckbox.Margin = new Padding(3, 2, 3, 2);
            autoStartCheckbox.Name = "autoStartCheckbox";
            autoStartCheckbox.Size = new Size(130, 19);
            autoStartCheckbox.TabIndex = 1;
            autoStartCheckbox.Text = "Start with Windows";
            autoStartCheckbox.UseVisualStyleBackColor = true;
            autoStartCheckbox.CheckedChanged += autoStartCheckbox_CheckedChanged;
            // 
            // enableDebugWhenAutoStartCheckbox
            // 
            enableDebugWhenAutoStartCheckbox.AutoSize = true;
            enableDebugWhenAutoStartCheckbox.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            enableDebugWhenAutoStartCheckbox.Location = new Point(576, 4);
            enableDebugWhenAutoStartCheckbox.Margin = new Padding(3, 2, 3, 2);
            enableDebugWhenAutoStartCheckbox.Name = "enableDebugWhenAutoStartCheckbox";
            enableDebugWhenAutoStartCheckbox.Size = new Size(107, 19);
            enableDebugWhenAutoStartCheckbox.TabIndex = 2;
            enableDebugWhenAutoStartCheckbox.Text = "in debug mode";
            enableDebugWhenAutoStartCheckbox.UseVisualStyleBackColor = true;
            enableDebugWhenAutoStartCheckbox.CheckedChanged += enableDebugWhenAutoStartCheckbox_CheckedChanged;
            // 
            // QsoCollectorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(689, 487);
            Controls.Add(autoStartCheckbox);
            Controls.Add(enableDebugWhenAutoStartCheckbox);
            Controls.Add(mainTabControl);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Margin = new Padding(3, 2, 3, 2);
            MaximizeBox = false;
            Name = "QsoCollectorForm";
            Text = "DXpedition QSO Collector © Alex UR8UQ";
            FormClosing += QsoCollectorForm_FormClosing;
            Shown += QsoCollectorForm_Shown;
            SizeChanged += QsoCollectorForm_SizeChanged;
            mainTabControl.ResumeLayout(false);
            clientTab.ResumeLayout(false);
            clientTab.PerformLayout();
            processingGroupBox.ResumeLayout(false);
            processingGroupBox.PerformLayout();
            ClientServerConfigGroupBox.ResumeLayout(false);
            ClientServerConfigGroupBox.PerformLayout();
            serverTab.ResumeLayout(false);
            serverTab.PerformLayout();
            serverProcessingGroupBox.ResumeLayout(false);
            serverProcessingGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)serverQsoAmountsDataGridView).EndInit();
            aboutTab.ResumeLayout(false);
            aboutTab.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TabControl mainTabControl;
        private TabPage clientTab;
        private TabPage serverTab;
        private CheckBox enableClientCheckBox;
        private CheckBox enableServerCheckBox;
        private Button startServerButton;
        private TextBox serverPortTextBox;
        private Label serverPortLabel;
        private Button qsoExportButton;
        private Button qsoImportButton;
        private Button stopServerButton;
        private Button startClientButton;
        private Button stopClientButton;
        private GroupBox ClientServerConfigGroupBox;
        private Label clientServerPortLabel;
        private TextBox clientServerPortTextBox;
        private TextBox clientServerNameIpTextBox;
        private Label clientServerNameLabel;
        private Button listenersConfigButton;
        private TextBox clientLogTextBox;
        private TextBox serverLogTextBox;
        private GroupBox processingGroupBox;
        private Label clientQsoReceivedLabel;
        private Label clientQsoReceivedCountLabel;
        private Label clientQsoSentToServerLabel;
        private Label clientQsoTempSavedLabel;
        private Label clientQsoSentToServerCountLabel;
        private Label clientQsoRejectedCountLabel;
        private Label clientQsoRejectedLabel;
        private Label clientQsoTempSavedCountLabel;
        private Label clientServerStatusLabel;
        private Label clientQsoLastTimeLabel;
        private Label clientServerStatusLastCheckedLabel;
        private Label clientServerStatusValueLabel;
        private Label clientServerCheckedAtLabel;
        private Microsoft.Data.Sqlite.SqliteConnection sqliteConnection1;
        private Label clientQsoRejectedAtLabel;
        private Label clientQsoTempSavedAtLabel;
        private Label clientQsoSentToServerAtLabel;
        private Label clientQsoReceviedAtLabel;
        private GroupBox serverProcessingGroupBox;
        private DataGridView serverQsoAmountsDataGridView;
        private BindingSource serverQsoAmountsBindingSource = new BindingSource();
        private SQLiteDataAdapter serverQsoAmountsDataAdapter = new SQLiteDataAdapter();
        private CheckBox clientLogDetailsCheckBox;
        private Label clientLogDetailsLabel;
        private CheckBox serverShowLogDetailsCheckBox;
        private NotifyIcon trayNotifyIcon;
        private CheckBox autoStartCheckbox;
        private CheckBox enableDebugWhenAutoStartCheckbox;
        private TabPage aboutTab;
        private TextBox aboutTextBox;
        private Button resetClientButton;
        private ToolTip myToolTip;
        private DataGridViewTextBoxColumn qsoAmountMode;
        private DataGridViewTextBoxColumn todayQsoAmount;
        private DataGridViewTextBoxColumn totalQsoAmount;
        private DataGridViewTextBoxColumn exportedQsoAmount;
        private DataGridViewTextBoxColumn lastQsoTime;
        private DataGridViewTextBoxColumn lastExportedQsoTime;
        private Button resetServerButton;
    }
}
