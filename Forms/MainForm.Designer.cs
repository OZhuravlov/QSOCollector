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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            mainTabControl = new TabControl();
            clientTab = new TabPage();
            clientServerCheckedAtLabel = new Label();
            clientServerStatusValueLabel = new Label();
            clientServerStatusLastCheckedLabel = new Label();
            clientServerStatusLabel = new Label();
            processingGroupBox = new GroupBox();
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
            serverShowLogDetailsCheckBox = new CheckBox();
            qsoImportButton = new Button();
            serverProcessingGroupBox = new GroupBox();
            serverQsoAmountsDataGridView = new DataGridView();
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
            qsoAmountMode = new DataGridViewTextBoxColumn();
            todayQsoAmount = new DataGridViewTextBoxColumn();
            totalQsoAmount = new DataGridViewTextBoxColumn();
            exportedQsoAmount = new DataGridViewTextBoxColumn();
            lastQsoTime = new DataGridViewTextBoxColumn();
            lastExportedQsoTime = new DataGridViewTextBoxColumn();
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
            mainTabControl.Name = "mainTabControl";
            mainTabControl.SelectedIndex = 0;
            mainTabControl.Size = new Size(789, 648);
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
            clientTab.Size = new Size(781, 558);
            clientTab.TabIndex = 0;
            clientTab.Text = "Client";
            // 
            // clientServerCheckedAtLabel
            // 
            clientServerCheckedAtLabel.BackColor = Color.Transparent;
            clientServerCheckedAtLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientServerCheckedAtLabel.Location = new Point(601, 83);
            clientServerCheckedAtLabel.Name = "clientServerCheckedAtLabel";
            clientServerCheckedAtLabel.Size = new Size(166, 23);
            clientServerCheckedAtLabel.TabIndex = 10;
            clientServerCheckedAtLabel.Text = "---";
            clientServerCheckedAtLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // clientServerStatusValueLabel
            // 
            clientServerStatusValueLabel.BackColor = Color.Thistle;
            clientServerStatusValueLabel.BorderStyle = BorderStyle.FixedSingle;
            clientServerStatusValueLabel.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            clientServerStatusValueLabel.Location = new Point(611, 33);
            clientServerStatusValueLabel.Name = "clientServerStatusValueLabel";
            clientServerStatusValueLabel.Size = new Size(147, 25);
            clientServerStatusValueLabel.TabIndex = 9;
            clientServerStatusValueLabel.Text = "Unknown";
            clientServerStatusValueLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // clientServerStatusLastCheckedLabel
            // 
            clientServerStatusLastCheckedLabel.AutoSize = true;
            clientServerStatusLastCheckedLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            clientServerStatusLastCheckedLabel.Location = new Point(640, 63);
            clientServerStatusLastCheckedLabel.Name = "clientServerStatusLastCheckedLabel";
            clientServerStatusLastCheckedLabel.Size = new Size(84, 20);
            clientServerStatusLastCheckedLabel.TabIndex = 8;
            clientServerStatusLastCheckedLabel.Text = "Checked at";
            // 
            // clientServerStatusLabel
            // 
            clientServerStatusLabel.AutoSize = true;
            clientServerStatusLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            clientServerStatusLabel.Location = new Point(633, 6);
            clientServerStatusLabel.Name = "clientServerStatusLabel";
            clientServerStatusLabel.Size = new Size(99, 20);
            clientServerStatusLabel.TabIndex = 7;
            clientServerStatusLabel.Text = "Server Status";
            // 
            // processingGroupBox
            // 
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
            processingGroupBox.Controls.Add(clientQsoTempSavedLabel);
            processingGroupBox.Controls.Add(clientQsoSentToServerCountLabel);
            processingGroupBox.Controls.Add(clientQsoSentToServerLabel);
            processingGroupBox.Controls.Add(clientQsoReceivedCountLabel);
            processingGroupBox.Controls.Add(clientQsoReceivedLabel);
            processingGroupBox.Controls.Add(clientLogTextBox);
            processingGroupBox.Location = new Point(9, 103);
            processingGroupBox.Name = "processingGroupBox";
            processingGroupBox.Size = new Size(769, 449);
            processingGroupBox.TabIndex = 6;
            processingGroupBox.TabStop = false;
            processingGroupBox.Text = "QSO processing";
            // 
            // clientLogDetailsCheckBox
            // 
            clientLogDetailsCheckBox.AutoSize = true;
            clientLogDetailsCheckBox.Location = new Point(702, 49);
            clientLogDetailsCheckBox.Name = "clientLogDetailsCheckBox";
            clientLogDetailsCheckBox.Size = new Size(18, 17);
            clientLogDetailsCheckBox.TabIndex = 20;
            clientLogDetailsCheckBox.UseVisualStyleBackColor = true;
            clientLogDetailsCheckBox.CheckedChanged += clientLogDetailsCheckBox_CheckedChanged;
            // 
            // clientLogDetailsLabel
            // 
            clientLogDetailsLabel.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientLogDetailsLabel.Location = new Point(670, 8);
            clientLogDetailsLabel.Name = "clientLogDetailsLabel";
            clientLogDetailsLabel.Size = new Size(80, 41);
            clientLogDetailsLabel.TabIndex = 19;
            clientLogDetailsLabel.Text = "Log details";
            clientLogDetailsLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // clientQsoRejectedAtLabel
            // 
            clientQsoRejectedAtLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoRejectedAtLabel.Location = new Point(516, 58);
            clientQsoRejectedAtLabel.Name = "clientQsoRejectedAtLabel";
            clientQsoRejectedAtLabel.Size = new Size(99, 20);
            clientQsoRejectedAtLabel.TabIndex = 18;
            clientQsoRejectedAtLabel.Text = "---";
            clientQsoRejectedAtLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // clientQsoTempSavedAtLabel
            // 
            clientQsoTempSavedAtLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoTempSavedAtLabel.Location = new Point(365, 58);
            clientQsoTempSavedAtLabel.Name = "clientQsoTempSavedAtLabel";
            clientQsoTempSavedAtLabel.Size = new Size(90, 20);
            clientQsoTempSavedAtLabel.TabIndex = 17;
            clientQsoTempSavedAtLabel.Text = "---";
            clientQsoTempSavedAtLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // clientQsoSentToServerAtLabel
            // 
            clientQsoSentToServerAtLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoSentToServerAtLabel.Location = new Point(200, 58);
            clientQsoSentToServerAtLabel.Name = "clientQsoSentToServerAtLabel";
            clientQsoSentToServerAtLabel.Size = new Size(77, 20);
            clientQsoSentToServerAtLabel.TabIndex = 16;
            clientQsoSentToServerAtLabel.Text = "---";
            clientQsoSentToServerAtLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // clientQsoReceviedAtLabel
            // 
            clientQsoReceviedAtLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoReceviedAtLabel.Location = new Point(68, 58);
            clientQsoReceviedAtLabel.Name = "clientQsoReceviedAtLabel";
            clientQsoReceviedAtLabel.Size = new Size(76, 20);
            clientQsoReceviedAtLabel.TabIndex = 15;
            clientQsoReceviedAtLabel.Text = "---";
            clientQsoReceviedAtLabel.TextAlign = ContentAlignment.MiddleCenter;
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
            clientQsoRejectedCountLabel.Location = new Point(598, 29);
            clientQsoRejectedCountLabel.Name = "clientQsoRejectedCountLabel";
            clientQsoRejectedCountLabel.Size = new Size(17, 20);
            clientQsoRejectedCountLabel.TabIndex = 13;
            clientQsoRejectedCountLabel.Text = "0";
            // 
            // clientQsoRejectedLabel
            // 
            clientQsoRejectedLabel.AutoSize = true;
            clientQsoRejectedLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoRejectedLabel.Location = new Point(516, 29);
            clientQsoRejectedLabel.Name = "clientQsoRejectedLabel";
            clientQsoRejectedLabel.Size = new Size(70, 20);
            clientQsoRejectedLabel.TabIndex = 12;
            clientQsoRejectedLabel.Text = "Rejected:";
            // 
            // clientQsoTempSavedCountLabel
            // 
            clientQsoTempSavedCountLabel.AutoSize = true;
            clientQsoTempSavedCountLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoTempSavedCountLabel.Location = new Point(461, 29);
            clientQsoTempSavedCountLabel.Name = "clientQsoTempSavedCountLabel";
            clientQsoTempSavedCountLabel.Size = new Size(17, 20);
            clientQsoTempSavedCountLabel.TabIndex = 11;
            clientQsoTempSavedCountLabel.Text = "0";
            // 
            // clientQsoTempSavedLabel
            // 
            clientQsoTempSavedLabel.AutoSize = true;
            clientQsoTempSavedLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoTempSavedLabel.Location = new Point(318, 29);
            clientQsoTempSavedLabel.Name = "clientQsoTempSavedLabel";
            clientQsoTempSavedLabel.Size = new Size(137, 20);
            clientQsoTempSavedLabel.TabIndex = 10;
            clientQsoTempSavedLabel.Text = "Temporarely saved:";
            // 
            // clientQsoSentToServerCountLabel
            // 
            clientQsoSentToServerCountLabel.AutoSize = true;
            clientQsoSentToServerCountLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoSentToServerCountLabel.Location = new Point(260, 29);
            clientQsoSentToServerCountLabel.Name = "clientQsoSentToServerCountLabel";
            clientQsoSentToServerCountLabel.Size = new Size(17, 20);
            clientQsoSentToServerCountLabel.TabIndex = 9;
            clientQsoSentToServerCountLabel.Text = "0";
            // 
            // clientQsoSentToServerLabel
            // 
            clientQsoSentToServerLabel.AutoSize = true;
            clientQsoSentToServerLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientQsoSentToServerLabel.Location = new Point(155, 29);
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
            clientLogTextBox.Size = new Size(760, 368);
            clientLogTextBox.TabIndex = 5;
            // 
            // listenersConfigButton
            // 
            listenersConfigButton.BackColor = Color.SlateGray;
            listenersConfigButton.FlatStyle = FlatStyle.Popup;
            listenersConfigButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            listenersConfigButton.Location = new Point(9, 50);
            listenersConfigButton.Name = "listenersConfigButton";
            listenersConfigButton.Size = new Size(129, 47);
            listenersConfigButton.TabIndex = 4;
            listenersConfigButton.Text = "UDP Listeners";
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
            stopClientButton.Location = new Point(470, 63);
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
            startClientButton.Location = new Point(470, 17);
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
            serverTab.Controls.Add(serverShowLogDetailsCheckBox);
            serverTab.Controls.Add(qsoImportButton);
            serverTab.Controls.Add(serverProcessingGroupBox);
            serverTab.Controls.Add(stopServerButton);
            serverTab.Controls.Add(qsoExportButton);
            serverTab.Controls.Add(startServerButton);
            serverTab.Controls.Add(serverPortTextBox);
            serverTab.Controls.Add(serverPortLabel);
            serverTab.Controls.Add(enableServerCheckBox);
            serverTab.Location = new Point(4, 32);
            serverTab.Name = "serverTab";
            serverTab.Padding = new Padding(3);
            serverTab.Size = new Size(781, 612);
            serverTab.TabIndex = 1;
            serverTab.Text = "Server";
            serverTab.UseVisualStyleBackColor = true;
            // 
            // serverShowLogDetailsCheckBox
            // 
            serverShowLogDetailsCheckBox.AutoSize = true;
            serverShowLogDetailsCheckBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            serverShowLogDetailsCheckBox.Location = new Point(6, 566);
            serverShowLogDetailsCheckBox.Name = "serverShowLogDetailsCheckBox";
            serverShowLogDetailsCheckBox.Size = new Size(144, 24);
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
            qsoImportButton.Location = new Point(499, 566);
            qsoImportButton.Name = "qsoImportButton";
            qsoImportButton.Size = new Size(96, 35);
            qsoImportButton.TabIndex = 1;
            qsoImportButton.Text = "QSO Import";
            qsoImportButton.UseVisualStyleBackColor = false;
            qsoImportButton.Click += qsoImportButton_Click;
            // 
            // serverProcessingGroupBox
            // 
            serverProcessingGroupBox.Controls.Add(serverQsoAmountsDataGridView);
            serverProcessingGroupBox.Controls.Add(serverLogTextBox);
            serverProcessingGroupBox.Location = new Point(3, 50);
            serverProcessingGroupBox.Name = "serverProcessingGroupBox";
            serverProcessingGroupBox.Size = new Size(775, 461);
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
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            serverQsoAmountsDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            serverQsoAmountsDataGridView.ColumnHeadersHeight = 29;
            serverQsoAmountsDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            serverQsoAmountsDataGridView.Columns.AddRange(new DataGridViewColumn[] { qsoAmountMode, todayQsoAmount, totalQsoAmount, exportedQsoAmount, lastQsoTime, lastExportedQsoTime });
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = SystemColors.Window;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            serverQsoAmountsDataGridView.DefaultCellStyle = dataGridViewCellStyle2;
            serverQsoAmountsDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            serverQsoAmountsDataGridView.Location = new Point(3, 29);
            serverQsoAmountsDataGridView.Name = "serverQsoAmountsDataGridView";
            serverQsoAmountsDataGridView.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = SystemColors.Control;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle3.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            serverQsoAmountsDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            serverQsoAmountsDataGridView.RowHeadersWidth = 51;
            serverQsoAmountsDataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            serverQsoAmountsDataGridView.RowTemplate.Height = 25;
            serverQsoAmountsDataGridView.RowTemplate.ReadOnly = true;
            serverQsoAmountsDataGridView.RowTemplate.Resizable = DataGridViewTriState.False;
            serverQsoAmountsDataGridView.ShowEditingIcon = false;
            serverQsoAmountsDataGridView.Size = new Size(766, 197);
            serverQsoAmountsDataGridView.TabIndex = 7;
            serverQsoAmountsDataGridView.RowsAdded += serverQsoAmountsDataGridView_RowsAdded;
            serverQsoAmountsDataGridView.RowsRemoved += serverQsoAmountsDataGridView_RowsRemoved;
            // 
            // serverLogTextBox
            // 
            serverLogTextBox.BackColor = SystemColors.Window;
            serverLogTextBox.Enabled = false;
            serverLogTextBox.Font = new Font("Consolas", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            serverLogTextBox.ForeColor = SystemColors.WindowText;
            serverLogTextBox.Location = new Point(6, 232);
            serverLogTextBox.Multiline = true;
            serverLogTextBox.Name = "serverLogTextBox";
            serverLogTextBox.PlaceholderText = "Server logs will be here ...";
            serverLogTextBox.ReadOnly = true;
            serverLogTextBox.ScrollBars = ScrollBars.Vertical;
            serverLogTextBox.Size = new Size(763, 272);
            serverLogTextBox.TabIndex = 6;
            // 
            // stopServerButton
            // 
            stopServerButton.AutoSize = true;
            stopServerButton.BackColor = Color.RosyBrown;
            stopServerButton.Enabled = false;
            stopServerButton.FlatStyle = FlatStyle.Popup;
            stopServerButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            stopServerButton.Location = new Point(592, 14);
            stopServerButton.Name = "stopServerButton";
            stopServerButton.Size = new Size(109, 33);
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
            qsoExportButton.Location = new Point(308, 566);
            qsoExportButton.Name = "qsoExportButton";
            qsoExportButton.Size = new Size(110, 35);
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
            startServerButton.Location = new Point(470, 14);
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
            serverPortTextBox.Location = new Point(349, 18);
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
            serverPortLabel.Location = new Point(174, 21);
            serverPortLabel.Name = "serverPortLabel";
            serverPortLabel.Size = new Size(169, 20);
            serverPortLabel.TabIndex = 1;
            serverPortLabel.Text = "Listen for Clients on Port";
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
            // aboutTab
            // 
            aboutTab.Controls.Add(aboutTextBox);
            aboutTab.Location = new Point(4, 32);
            aboutTab.Name = "aboutTab";
            aboutTab.Size = new Size(711, 558);
            aboutTab.TabIndex = 2;
            aboutTab.Text = "About";
            aboutTab.UseVisualStyleBackColor = true;
            // 
            // aboutTextBox
            // 
            aboutTextBox.BackColor = SystemColors.Window;
            aboutTextBox.Font = new Font("Consolas", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            aboutTextBox.ForeColor = SystemColors.WindowText;
            aboutTextBox.Location = new Point(7, 19);
            aboutTextBox.MaxLength = 1000000;
            aboutTextBox.Multiline = true;
            aboutTextBox.Name = "aboutTextBox";
            aboutTextBox.ReadOnly = true;
            aboutTextBox.ScrollBars = ScrollBars.Vertical;
            aboutTextBox.Size = new Size(704, 534);
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
            autoStartCheckbox.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            autoStartCheckbox.Location = new Point(500, 4);
            autoStartCheckbox.Name = "autoStartCheckbox";
            autoStartCheckbox.Size = new Size(275, 24);
            autoStartCheckbox.TabIndex = 1;
            autoStartCheckbox.Text = "Start automatically (Recommended)";
            autoStartCheckbox.UseVisualStyleBackColor = true;
            autoStartCheckbox.CheckedChanged += autoStartCheckbox_CheckedChanged;
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
            todayQsoAmount.Width = 90;
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
            totalQsoAmount.Width = 90;
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
            exportedQsoAmount.Width = 80;
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
            lastQsoTime.Width = 180;
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
            lastExportedQsoTime.Width = 180;
            // 
            // QsoCollectorForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(787, 649);
            Controls.Add(autoStartCheckbox);
            Controls.Add(mainTabControl);
            FormBorderStyle = FormBorderStyle.Fixed3D;
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
        private TabPage aboutTab;
        private TextBox aboutTextBox;
        private DataGridViewTextBoxColumn qsoAmountMode;
        private DataGridViewTextBoxColumn todayQsoAmount;
        private DataGridViewTextBoxColumn totalQsoAmount;
        private DataGridViewTextBoxColumn exportedQsoAmount;
        private DataGridViewTextBoxColumn lastQsoTime;
        private DataGridViewTextBoxColumn lastExportedQsoTime;
    }
}
