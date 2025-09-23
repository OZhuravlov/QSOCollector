namespace QSOCollector
{
    partial class qsoCollectorForm
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
            mainTabControl.SuspendLayout();
            clientTab.SuspendLayout();
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
            mainTabControl.Size = new Size(683, 452);
            mainTabControl.TabIndex = 0;
            // 
            // clientTab
            // 
            clientTab.BackColor = Color.Transparent;
            clientTab.Controls.Add(clientLogTextBox);
            clientTab.Controls.Add(listenersConfigButton);
            clientTab.Controls.Add(ClientServerConfigGroupBox);
            clientTab.Controls.Add(stopClientButton);
            clientTab.Controls.Add(startClientButton);
            clientTab.Controls.Add(enableClientCheckBox);
            clientTab.Location = new Point(4, 32);
            clientTab.Name = "clientTab";
            clientTab.Padding = new Padding(3);
            clientTab.Size = new Size(675, 416);
            clientTab.TabIndex = 0;
            clientTab.Text = "Client";
            // 
            // clientLogTextBox
            // 
            clientLogTextBox.BackColor = SystemColors.Window;
            clientLogTextBox.Font = new Font("Consolas", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            clientLogTextBox.ForeColor = SystemColors.InfoText;
            clientLogTextBox.ImeMode = ImeMode.NoControl;
            clientLogTextBox.Location = new Point(3, 104);
            clientLogTextBox.MaxLength = 2147483646;
            clientLogTextBox.Multiline = true;
            clientLogTextBox.Name = "clientLogTextBox";
            clientLogTextBox.PlaceholderText = "Client logs will here ...";
            clientLogTextBox.ReadOnly = true;
            clientLogTextBox.ScrollBars = ScrollBars.Both;
            clientLogTextBox.Size = new Size(664, 312);
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
            listenersConfigButton.Click += listenersConfigButton_Click;
            // 
            // ClientServerConfigGroupBox
            // 
            ClientServerConfigGroupBox.Controls.Add(clientServerPortLabel);
            ClientServerConfigGroupBox.Controls.Add(clientServerPortTextBox);
            ClientServerConfigGroupBox.Controls.Add(clientServerNameIpTextBox);
            ClientServerConfigGroupBox.Controls.Add(clientServerNameLabel);
            ClientServerConfigGroupBox.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ClientServerConfigGroupBox.Location = new Point(200, 6);
            ClientServerConfigGroupBox.Name = "ClientServerConfigGroupBox";
            ClientServerConfigGroupBox.Size = new Size(289, 91);
            ClientServerConfigGroupBox.TabIndex = 3;
            ClientServerConfigGroupBox.TabStop = false;
            ClientServerConfigGroupBox.Text = "Server config";
            // 
            // clientServerPortLabel
            // 
            clientServerPortLabel.AutoSize = true;
            clientServerPortLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientServerPortLabel.Location = new Point(16, 61);
            clientServerPortLabel.Name = "clientServerPortLabel";
            clientServerPortLabel.Size = new Size(35, 20);
            clientServerPortLabel.TabIndex = 3;
            clientServerPortLabel.Text = "Port";
            // 
            // clientServerPortTextBox
            // 
            clientServerPortTextBox.Location = new Point(89, 57);
            clientServerPortTextBox.Name = "clientServerPortTextBox";
            clientServerPortTextBox.Size = new Size(125, 27);
            clientServerPortTextBox.TabIndex = 2;
            clientServerPortTextBox.TextChanged += clientServerPortTextBox_TextChanged;
            clientServerPortTextBox.KeyPress += portTextBox_KeyPress;
            // 
            // clientServerNameIpTextBox
            // 
            clientServerNameIpTextBox.Location = new Point(86, 21);
            clientServerNameIpTextBox.MaxLength = 200;
            clientServerNameIpTextBox.Name = "clientServerNameIpTextBox";
            clientServerNameIpTextBox.Size = new Size(197, 27);
            clientServerNameIpTextBox.TabIndex = 1;
            clientServerNameIpTextBox.TextChanged += clientServerNameIpTextBox_TextChanged;
            // 
            // clientServerNameLabel
            // 
            clientServerNameLabel.AutoSize = true;
            clientServerNameLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clientServerNameLabel.Location = new Point(16, 28);
            clientServerNameLabel.Name = "clientServerNameLabel";
            clientServerNameLabel.Size = new Size(67, 20);
            clientServerNameLabel.TabIndex = 0;
            clientServerNameLabel.Text = "Name/IP";
            // 
            // stopClientButton
            // 
            stopClientButton.BackColor = Color.RosyBrown;
            stopClientButton.Enabled = false;
            stopClientButton.FlatStyle = FlatStyle.Popup;
            stopClientButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            stopClientButton.Location = new Point(539, 67);
            stopClientButton.Name = "stopClientButton";
            stopClientButton.Size = new Size(111, 34);
            stopClientButton.TabIndex = 2;
            stopClientButton.Text = "Stop Client";
            stopClientButton.UseVisualStyleBackColor = false;
            stopClientButton.Click += stopClientButton_Click;
            // 
            // startClientButton
            // 
            startClientButton.BackColor = Color.DarkSeaGreen;
            startClientButton.FlatStyle = FlatStyle.Popup;
            startClientButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            startClientButton.Location = new Point(539, 22);
            startClientButton.Name = "startClientButton";
            startClientButton.Size = new Size(111, 36);
            startClientButton.TabIndex = 1;
            startClientButton.Text = "Start Client";
            startClientButton.UseVisualStyleBackColor = false;
            startClientButton.Click += startClientButton_Click;
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
            enableClientCheckBox.CheckedChanged += enableClientCheckBox_CheckedChanged;
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
            serverTab.Size = new Size(675, 416);
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
            stopServerButton.Click += stopServerButton_Click;
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
            startServerButton.Click += startServerButton_Click;
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
            serverPortTextBox.TextChanged += serverPortTextBox_TextChanged;
            serverPortTextBox.KeyPress += portTextBox_KeyPress;
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
            enableServerCheckBox.CheckedChanged += enableServerCheckBox_CheckedChanged;
            // 
            // qsoCollectorForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(682, 453);
            Controls.Add(mainTabControl);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            Name = "qsoCollectorForm";
            Text = "QSO Collector";
            FormClosing += qsoCollectorForm_FormClosing;
            mainTabControl.ResumeLayout(false);
            clientTab.ResumeLayout(false);
            clientTab.PerformLayout();
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
    }
}
