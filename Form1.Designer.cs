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
            mainTabControl = new TabControl();
            clientTab = new TabPage();
            clientServerCheckedAtLabel = new Label();
            clientServerStatusValueLabel = new Label();
            clientServerStatusLastCheckedLabel = new Label();
            clientServerStatusLabel = new Label();
            processingGroupBox = new GroupBox();
            clientQsoRejectedAtLabel = new Label();
            clientQsoTempSavedAtLabel = new Label();
            clientQsoSentToServerAtLabel = new Label();
            clientQsoReceviedAtLabel = new Label();
            clientQsoLastTimeLabel = new Label();
            clientQsoRejectedCountLabel = new Label();
            clientQsoRejectedLabel = new Label();
            clientQsoTempSavedCountLabel = new Label();
            clientQsoTempSavedLabel = new Label();
            clientQsoSentToServerCountLabel = new Label();
            clientQsoSentToServerLabel = new Label();
            clientQsoReceivedCountLabel = new Label();
            clientQsoReceivedLabel = new Label();
            clientLogTextBox = new TextBox();
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
            serverLogTextBox = new TextBox();
            stopServerButton = new Button();
            serverQsoGroupBox = new GroupBox();
            qsoStatisticsButton = new Button();
            qsoImportButton = new Button();
            qsoExportButton = new Button();
            startServerButton = new Button();
            serverPortTextBox = new TextBox();
            serverPortLabel = new Label();
            enableServerCheckBox = new CheckBox();
            sqliteConnection1 = new Microsoft.Data.Sqlite.SqliteConnection();
            mainTabControl.SuspendLayout();
            clientTab.SuspendLayout();
            processingGroupBox.SuspendLayout();
            ClientServerConfigGroupBox.SuspendLayout();
            serverTab.SuspendLayout();
            serverQsoGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // mainTabControl
            // 
            mainTabControl.Controls.Add(clientTab);
            mainTabControl.Controls.Add(serverTab);
            mainTabControl.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            mainTabControl.Location = new Point(-1, 0);
            mainTabControl.Name = "mainTabControl";
            mainTabControl.SelectedIndex = 0;
            mainTabControl.Size = new Size(693, 594);
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
            clientTab.Location = new Point(4, 32);
            clientTab.Name = "clientTab";
            clientTab.Padding = new Padding(3);
            clientTab.Size = new Size(685, 558);
            clientTab.TabIndex = 0;
            clientTab.Text = "Client";
            // 
            // clientServerCheckedAtLabel
            // 
            clientServerCheckedAtLabel.AutoSize = true;
            clientServerCheckedAtLabel.BackColor = Color.Transparent;
            clientServerCheckedAtLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientServerCheckedAtLabel.Location = new Point(585, 83);
            clientServerCheckedAtLabel.Name = "clientServerCheckedAtLabel";
            clientServerCheckedAtLabel.Size = new Size(27, 20);
            clientServerCheckedAtLabel.TabIndex = 10;
            clientServerCheckedAtLabel.Text = "---";
            clientServerCheckedAtLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // clientServerStatusValueLabel
            // 
            clientServerStatusValueLabel.AutoSize = true;
            clientServerStatusValueLabel.BackColor = Color.Gray;
            clientServerStatusValueLabel.BorderStyle = BorderStyle.FixedSingle;
            clientServerStatusValueLabel.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            clientServerStatusValueLabel.Location = new Point(551, 31);
            clientServerStatusValueLabel.Name = "clientServerStatusValueLabel";
            clientServerStatusValueLabel.Size = new Size(88, 25);
            clientServerStatusValueLabel.TabIndex = 9;
            clientServerStatusValueLabel.Text = "Unknown";
            clientServerStatusValueLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // clientServerStatusLastCheckedLabel
            // 
            clientServerStatusLastCheckedLabel.AutoSize = true;
            clientServerStatusLastCheckedLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            clientServerStatusLastCheckedLabel.Location = new Point(538, 63);
            clientServerStatusLastCheckedLabel.Name = "clientServerStatusLastCheckedLabel";
            clientServerStatusLastCheckedLabel.Size = new Size(112, 20);
            clientServerStatusLastCheckedLabel.TabIndex = 8;
            clientServerStatusLastCheckedLabel.Text = "Last checked at";
            // 
            // clientServerStatusLabel
            // 
            clientServerStatusLabel.AutoSize = true;
            clientServerStatusLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            clientServerStatusLabel.Location = new Point(540, 6);
            clientServerStatusLabel.Name = "clientServerStatusLabel";
            clientServerStatusLabel.Size = new Size(99, 20);
            clientServerStatusLabel.TabIndex = 7;
            clientServerStatusLabel.Text = "Server Status";
            // 
            // processingGroupBox
            // 
            processingGroupBox.Controls.Add(clientQsoRejectedAtLabel);
            processingGroupBox.Controls.Add(clientQsoTempSavedAtLabel);
            processingGroupBox.Controls.Add(clientQsoSentToServerAtLabel);
            processingGroupBox.Controls.Add(clientQsoReceviedAtLabel);
            processingGroupBox.Controls.Add(clientQsoLastTimeLabel);
            processingGroupBox.Controls.Add(clientQsoRejectedCountLabel);
            processingGroupBox.Controls.Add(clientQsoRejectedLabel);
            processingGroupBox.Controls.Add(clientQsoTempSavedCountLabel);
            processingGroupBox.Controls.Add(clientQsoTempSavedLabel);
            processingGroupBox.Controls.Add(clientQsoSentToServerCountLabel);
            processingGroupBox.Controls.Add(clientQsoSentToServerLabel);
            processingGroupBox.Controls.Add(clientQsoReceivedCountLabel);
            processingGroupBox.Controls.Add(clientQsoReceivedLabel);
            processingGroupBox.Controls.Add(clientLogTextBox);
            processingGroupBox.Location = new Point(9, 103);
            processingGroupBox.Name = "processingGroupBox";
            processingGroupBox.Size = new Size(670, 449);
            processingGroupBox.TabIndex = 6;
            processingGroupBox.TabStop = false;
            processingGroupBox.Text = "QSO processing";
            // 
            // clientQsoRejectedAtLabel
            // 
            clientQsoRejectedAtLabel.AutoSize = true;
            clientQsoRejectedAtLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoRejectedAtLabel.Location = new Point(548, 58);
            clientQsoRejectedAtLabel.Name = "clientQsoRejectedAtLabel";
            clientQsoRejectedAtLabel.Size = new Size(27, 20);
            clientQsoRejectedAtLabel.TabIndex = 18;
            clientQsoRejectedAtLabel.Text = "---";
            clientQsoRejectedAtLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // clientQsoTempSavedAtLabel
            // 
            clientQsoTempSavedAtLabel.AutoSize = true;
            clientQsoTempSavedAtLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoTempSavedAtLabel.Location = new Point(416, 58);
            clientQsoTempSavedAtLabel.Name = "clientQsoTempSavedAtLabel";
            clientQsoTempSavedAtLabel.Size = new Size(27, 20);
            clientQsoTempSavedAtLabel.TabIndex = 17;
            clientQsoTempSavedAtLabel.Text = "---";
            clientQsoTempSavedAtLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // clientQsoSentToServerAtLabel
            // 
            clientQsoSentToServerAtLabel.AutoSize = true;
            clientQsoSentToServerAtLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoSentToServerAtLabel.Location = new Point(247, 58);
            clientQsoSentToServerAtLabel.Name = "clientQsoSentToServerAtLabel";
            clientQsoSentToServerAtLabel.Size = new Size(27, 20);
            clientQsoSentToServerAtLabel.TabIndex = 16;
            clientQsoSentToServerAtLabel.Text = "---";
            clientQsoSentToServerAtLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // clientQsoReceviedAtLabel
            // 
            clientQsoReceviedAtLabel.AutoSize = true;
            clientQsoReceviedAtLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoReceviedAtLabel.Location = new Point(68, 58);
            clientQsoReceviedAtLabel.Name = "clientQsoReceviedAtLabel";
            clientQsoReceviedAtLabel.Size = new Size(27, 20);
            clientQsoReceviedAtLabel.TabIndex = 15;
            clientQsoReceviedAtLabel.Text = "---";
            clientQsoReceviedAtLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // clientQsoLastTimeLabel
            // 
            clientQsoLastTimeLabel.AutoSize = true;
            clientQsoLastTimeLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoLastTimeLabel.Location = new Point(9, 58);
            clientQsoLastTimeLabel.Name = "clientQsoLastTimeLabel";
            clientQsoLastTimeLabel.Size = new Size(52, 20);
            clientQsoLastTimeLabel.TabIndex = 14;
            clientQsoLastTimeLabel.Text = "Last at";
            // 
            // clientQsoRejectedCountLabel
            // 
            clientQsoRejectedCountLabel.AutoSize = true;
            clientQsoRejectedCountLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoRejectedCountLabel.Location = new Point(624, 29);
            clientQsoRejectedCountLabel.Name = "clientQsoRejectedCountLabel";
            clientQsoRejectedCountLabel.Size = new Size(17, 20);
            clientQsoRejectedCountLabel.TabIndex = 13;
            clientQsoRejectedCountLabel.Text = "0";
            // 
            // clientQsoRejectedLabel
            // 
            clientQsoRejectedLabel.AutoSize = true;
            clientQsoRejectedLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoRejectedLabel.Location = new Point(548, 29);
            clientQsoRejectedLabel.Name = "clientQsoRejectedLabel";
            clientQsoRejectedLabel.Size = new Size(70, 20);
            clientQsoRejectedLabel.TabIndex = 12;
            clientQsoRejectedLabel.Text = "Rejected:";
            // 
            // clientQsoTempSavedCountLabel
            // 
            clientQsoTempSavedCountLabel.AutoSize = true;
            clientQsoTempSavedCountLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoTempSavedCountLabel.Location = new Point(500, 29);
            clientQsoTempSavedCountLabel.Name = "clientQsoTempSavedCountLabel";
            clientQsoTempSavedCountLabel.Size = new Size(17, 20);
            clientQsoTempSavedCountLabel.TabIndex = 11;
            clientQsoTempSavedCountLabel.Text = "0";
            // 
            // clientQsoTempSavedLabel
            // 
            clientQsoTempSavedLabel.AutoSize = true;
            clientQsoTempSavedLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoTempSavedLabel.Location = new Point(357, 29);
            clientQsoTempSavedLabel.Name = "clientQsoTempSavedLabel";
            clientQsoTempSavedLabel.Size = new Size(137, 20);
            clientQsoTempSavedLabel.TabIndex = 10;
            clientQsoTempSavedLabel.Text = "Temporarely saved:";
            // 
            // clientQsoSentToServerCountLabel
            // 
            clientQsoSentToServerCountLabel.AutoSize = true;
            clientQsoSentToServerCountLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoSentToServerCountLabel.Location = new Point(277, 29);
            clientQsoSentToServerCountLabel.Name = "clientQsoSentToServerCountLabel";
            clientQsoSentToServerCountLabel.Size = new Size(17, 20);
            clientQsoSentToServerCountLabel.TabIndex = 9;
            clientQsoSentToServerCountLabel.Text = "0";
            // 
            // clientQsoSentToServerLabel
            // 
            clientQsoSentToServerLabel.AutoSize = true;
            clientQsoSentToServerLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoSentToServerLabel.Location = new Point(170, 29);
            clientQsoSentToServerLabel.Name = "clientQsoSentToServerLabel";
            clientQsoSentToServerLabel.Size = new Size(104, 20);
            clientQsoSentToServerLabel.TabIndex = 8;
            clientQsoSentToServerLabel.Text = "Sent to Server:";
            // 
            // clientQsoReceivedCountLabel
            // 
            clientQsoReceivedCountLabel.AutoSize = true;
            clientQsoReceivedCountLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoReceivedCountLabel.Location = new Point(78, 29);
            clientQsoReceivedCountLabel.Name = "clientQsoReceivedCountLabel";
            clientQsoReceivedCountLabel.Size = new Size(17, 20);
            clientQsoReceivedCountLabel.TabIndex = 7;
            clientQsoReceivedCountLabel.Text = "0";
            // 
            // clientQsoReceivedLabel
            // 
            clientQsoReceivedLabel.AutoSize = true;
            clientQsoReceivedLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoReceivedLabel.Location = new Point(9, 29);
            clientQsoReceivedLabel.Name = "clientQsoReceivedLabel";
            clientQsoReceivedLabel.Size = new Size(72, 20);
            clientQsoReceivedLabel.TabIndex = 6;
            clientQsoReceivedLabel.Text = "Received:";
            // 
            // clientLogTextBox
            // 
            clientLogTextBox.BackColor = SystemColors.Window;
            clientLogTextBox.Font = new Font("Consolas", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            clientLogTextBox.ForeColor = SystemColors.InfoText;
            clientLogTextBox.ImeMode = ImeMode.NoControl;
            clientLogTextBox.Location = new Point(6, 81);
            clientLogTextBox.MaxLength = 2147483646;
            clientLogTextBox.Multiline = true;
            clientLogTextBox.Name = "clientLogTextBox";
            clientLogTextBox.PlaceholderText = "Client logs will be here ...";
            clientLogTextBox.ReadOnly = true;
            clientLogTextBox.ScrollBars = ScrollBars.Both;
            clientLogTextBox.Size = new Size(664, 368);
            clientLogTextBox.TabIndex = 5;
            // 
            // listenersConfigButton
            // 
            listenersConfigButton.FlatStyle = FlatStyle.Popup;
            listenersConfigButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            listenersConfigButton.Location = new Point(9, 50);
            listenersConfigButton.Name = "listenersConfigButton";
            listenersConfigButton.Size = new Size(108, 47);
            listenersConfigButton.TabIndex = 4;
            listenersConfigButton.Text = "Listeners";
            listenersConfigButton.UseVisualStyleBackColor = true;
            listenersConfigButton.Click += ListenersConfigButton_Click;
            // 
            // ClientServerConfigGroupBox
            // 
            ClientServerConfigGroupBox.Controls.Add(clientServerPortLabel);
            ClientServerConfigGroupBox.Controls.Add(clientServerPortTextBox);
            ClientServerConfigGroupBox.Controls.Add(clientServerNameIpTextBox);
            ClientServerConfigGroupBox.Controls.Add(clientServerNameLabel);
            ClientServerConfigGroupBox.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ClientServerConfigGroupBox.Location = new Point(144, 6);
            ClientServerConfigGroupBox.Name = "ClientServerConfigGroupBox";
            ClientServerConfigGroupBox.Size = new Size(230, 91);
            ClientServerConfigGroupBox.TabIndex = 3;
            ClientServerConfigGroupBox.TabStop = false;
            ClientServerConfigGroupBox.Text = "Server config";
            // 
            // clientServerPortLabel
            // 
            clientServerPortLabel.AutoSize = true;
            clientServerPortLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientServerPortLabel.Location = new Point(6, 57);
            clientServerPortLabel.Name = "clientServerPortLabel";
            clientServerPortLabel.Size = new Size(35, 20);
            clientServerPortLabel.TabIndex = 3;
            clientServerPortLabel.Text = "Port";
            // 
            // clientServerPortTextBox
            // 
            clientServerPortTextBox.Location = new Point(89, 57);
            clientServerPortTextBox.Name = "clientServerPortTextBox";
            clientServerPortTextBox.Size = new Size(70, 27);
            clientServerPortTextBox.TabIndex = 2;
            clientServerPortTextBox.TextChanged += clientServerPortTextBox_TextChanged;
            clientServerPortTextBox.KeyPress += PortTextBox_KeyPress;
            // 
            // clientServerNameIpTextBox
            // 
            clientServerNameIpTextBox.Location = new Point(90, 25);
            clientServerNameIpTextBox.MaxLength = 200;
            clientServerNameIpTextBox.Name = "clientServerNameIpTextBox";
            clientServerNameIpTextBox.Size = new Size(128, 27);
            clientServerNameIpTextBox.TabIndex = 1;
            clientServerNameIpTextBox.TextChanged += clientServerNameIpTextBox_TextChanged;
            // 
            // clientServerNameLabel
            // 
            clientServerNameLabel.AutoSize = true;
            clientServerNameLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientServerNameLabel.Location = new Point(6, 28);
            clientServerNameLabel.Name = "clientServerNameLabel";
            clientServerNameLabel.Size = new Size(78, 20);
            clientServerNameLabel.TabIndex = 0;
            clientServerNameLabel.Text = "IP Address";
            // 
            // stopClientButton
            // 
            stopClientButton.BackColor = Color.RosyBrown;
            stopClientButton.Enabled = false;
            stopClientButton.FlatStyle = FlatStyle.Popup;
            stopClientButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            stopClientButton.Location = new Point(380, 63);
            stopClientButton.Name = "stopClientButton";
            stopClientButton.Size = new Size(111, 34);
            stopClientButton.TabIndex = 2;
            stopClientButton.Text = "Stop Client";
            stopClientButton.UseVisualStyleBackColor = false;
            stopClientButton.Click += StopClientButton_Click;
            // 
            // startClientButton
            // 
            startClientButton.BackColor = Color.DarkSeaGreen;
            startClientButton.FlatStyle = FlatStyle.Popup;
            startClientButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            startClientButton.Location = new Point(380, 17);
            startClientButton.Name = "startClientButton";
            startClientButton.Size = new Size(111, 36);
            startClientButton.TabIndex = 1;
            startClientButton.Text = "Start Client";
            startClientButton.UseVisualStyleBackColor = false;
            startClientButton.Click += StartClientButton_Click;
            // 
            // enableClientCheckBox
            // 
            enableClientCheckBox.AutoSize = true;
            enableClientCheckBox.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            enableClientCheckBox.Location = new Point(9, 18);
            enableClientCheckBox.Name = "enableClientCheckBox";
            enableClientCheckBox.Size = new Size(77, 24);
            enableClientCheckBox.TabIndex = 0;
            enableClientCheckBox.Text = "Enable";
            enableClientCheckBox.UseVisualStyleBackColor = true;
            enableClientCheckBox.CheckedChanged += EnableClientCheckBox_CheckedChanged;
            // 
            // serverTab
            // 
            serverTab.Controls.Add(serverLogTextBox);
            serverTab.Controls.Add(stopServerButton);
            serverTab.Controls.Add(serverQsoGroupBox);
            serverTab.Controls.Add(startServerButton);
            serverTab.Controls.Add(serverPortTextBox);
            serverTab.Controls.Add(serverPortLabel);
            serverTab.Controls.Add(enableServerCheckBox);
            serverTab.Location = new Point(4, 32);
            serverTab.Name = "serverTab";
            serverTab.Padding = new Padding(3);
            serverTab.Size = new Size(685, 558);
            serverTab.TabIndex = 1;
            serverTab.Text = "Server";
            serverTab.UseVisualStyleBackColor = true;
            // 
            // serverLogTextBox
            // 
            serverLogTextBox.BackColor = SystemColors.Window;
            serverLogTextBox.Enabled = false;
            serverLogTextBox.Font = new Font("Consolas", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            serverLogTextBox.ForeColor = SystemColors.WindowText;
            serverLogTextBox.Location = new Point(3, 50);
            serverLogTextBox.Multiline = true;
            serverLogTextBox.Name = "serverLogTextBox";
            serverLogTextBox.PlaceholderText = "Server logs will be here ...";
            serverLogTextBox.Size = new Size(664, 296);
            serverLogTextBox.TabIndex = 6;
            // 
            // stopServerButton
            // 
            stopServerButton.AutoSize = true;
            stopServerButton.BackColor = Color.RosyBrown;
            stopServerButton.Enabled = false;
            stopServerButton.FlatStyle = FlatStyle.Popup;
            stopServerButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            stopServerButton.Location = new Point(426, 11);
            stopServerButton.Name = "stopServerButton";
            stopServerButton.Size = new Size(109, 33);
            stopServerButton.TabIndex = 5;
            stopServerButton.Text = "Stop Server";
            stopServerButton.UseVisualStyleBackColor = false;
            stopServerButton.Click += StopServerButton_Click;
            // 
            // serverQsoGroupBox
            // 
            serverQsoGroupBox.Controls.Add(qsoStatisticsButton);
            serverQsoGroupBox.Controls.Add(qsoImportButton);
            serverQsoGroupBox.Controls.Add(qsoExportButton);
            serverQsoGroupBox.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            serverQsoGroupBox.Location = new Point(9, 352);
            serverQsoGroupBox.Name = "serverQsoGroupBox";
            serverQsoGroupBox.Size = new Size(658, 61);
            serverQsoGroupBox.TabIndex = 4;
            serverQsoGroupBox.TabStop = false;
            serverQsoGroupBox.Text = "QSOs";
            // 
            // qsoStatisticsButton
            // 
            qsoStatisticsButton.BackColor = Color.Transparent;
            qsoStatisticsButton.Enabled = false;
            qsoStatisticsButton.FlatStyle = FlatStyle.System;
            qsoStatisticsButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            qsoStatisticsButton.Location = new Point(486, 22);
            qsoStatisticsButton.Name = "qsoStatisticsButton";
            qsoStatisticsButton.Size = new Size(115, 35);
            qsoStatisticsButton.TabIndex = 2;
            qsoStatisticsButton.Text = "Statistics";
            qsoStatisticsButton.UseVisualStyleBackColor = false;
            // 
            // qsoImportButton
            // 
            qsoImportButton.BackColor = Color.Transparent;
            qsoImportButton.Enabled = false;
            qsoImportButton.FlatStyle = FlatStyle.System;
            qsoImportButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            qsoImportButton.Location = new Point(290, 20);
            qsoImportButton.Name = "qsoImportButton";
            qsoImportButton.Size = new Size(115, 35);
            qsoImportButton.TabIndex = 1;
            qsoImportButton.Text = "Import";
            qsoImportButton.UseVisualStyleBackColor = false;
            // 
            // qsoExportButton
            // 
            qsoExportButton.BackColor = Color.Transparent;
            qsoExportButton.Enabled = false;
            qsoExportButton.FlatStyle = FlatStyle.System;
            qsoExportButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            qsoExportButton.ForeColor = SystemColors.ControlText;
            qsoExportButton.Location = new Point(91, 20);
            qsoExportButton.Name = "qsoExportButton";
            qsoExportButton.Size = new Size(115, 35);
            qsoExportButton.TabIndex = 0;
            qsoExportButton.Text = "Export";
            qsoExportButton.UseVisualStyleBackColor = false;
            // 
            // startServerButton
            // 
            startServerButton.AutoSize = true;
            startServerButton.BackColor = Color.DarkSeaGreen;
            startServerButton.Enabled = false;
            startServerButton.FlatStyle = FlatStyle.Popup;
            startServerButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            startServerButton.ForeColor = Color.Black;
            startServerButton.Location = new Point(307, 11);
            startServerButton.Name = "startServerButton";
            startServerButton.Size = new Size(107, 33);
            startServerButton.TabIndex = 3;
            startServerButton.Text = "Start Server";
            startServerButton.UseVisualStyleBackColor = false;
            startServerButton.Click += StartServerButton_Click;
            // 
            // serverPortTextBox
            // 
            serverPortTextBox.Enabled = false;
            serverPortTextBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            serverPortTextBox.Location = new Point(204, 17);
            serverPortTextBox.Name = "serverPortTextBox";
            serverPortTextBox.PlaceholderText = "(port number)";
            serverPortTextBox.Size = new Size(82, 27);
            serverPortTextBox.TabIndex = 2;
            serverPortTextBox.TextChanged += ServerPortTextBox_TextChanged;
            serverPortTextBox.KeyPress += PortTextBox_KeyPress;
            // 
            // serverPortLabel
            // 
            serverPortLabel.AutoSize = true;
            serverPortLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            serverPortLabel.Location = new Point(100, 20);
            serverPortLabel.Name = "serverPortLabel";
            serverPortLabel.Size = new Size(98, 20);
            serverPortLabel.TabIndex = 1;
            serverPortLabel.Text = "Listen on Port";
            // 
            // enableServerCheckBox
            // 
            enableServerCheckBox.AutoSize = true;
            enableServerCheckBox.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            enableServerCheckBox.Location = new Point(18, 20);
            enableServerCheckBox.Name = "enableServerCheckBox";
            enableServerCheckBox.Size = new Size(77, 24);
            enableServerCheckBox.TabIndex = 0;
            enableServerCheckBox.Text = "Enable";
            enableServerCheckBox.UseVisualStyleBackColor = true;
            enableServerCheckBox.CheckedChanged += EnableServerCheckBox_CheckedChanged;
            // 
            // sqliteConnection1
            // 
            sqliteConnection1.DefaultTimeout = 30;
            // 
            // QsoCollectorForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(694, 597);
            Controls.Add(mainTabControl);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            Name = "QsoCollectorForm";
            Text = "QSO Collector";
            FormClosing += QsoCollectorForm_FormClosing;
            mainTabControl.ResumeLayout(false);
            clientTab.ResumeLayout(false);
            clientTab.PerformLayout();
            processingGroupBox.ResumeLayout(false);
            processingGroupBox.PerformLayout();
            ClientServerConfigGroupBox.ResumeLayout(false);
            ClientServerConfigGroupBox.PerformLayout();
            serverTab.ResumeLayout(false);
            serverTab.PerformLayout();
            serverQsoGroupBox.ResumeLayout(false);
            ResumeLayout(false);
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
        private GroupBox serverQsoGroupBox;
        private Button qsoExportButton;
        private Button qsoStatisticsButton;
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
    }
}
