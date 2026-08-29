using System.Data.SQLite;

namespace QSOCollector
{
    partial class QsoImportForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            closeCancelButton = new Button();
            importButton = new Button();
            chooseFileButton = new Button();
            filePathLabel = new Label();
            filePreviewTextBox = new TextBox();
            importBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            importProgressBar = new ProgressBar();
            importProgressStep = new Label();
            applyRuleDataGridView = new DataGridView();
            id = new DataGridViewTextBoxColumn();
            ruleName = new DataGridViewTextBoxColumn();
            isApplyForImport = new DataGridViewCheckBoxColumn();
            ruleDetails = new DataGridViewButtonColumn();
            RulesApplyLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)applyRuleDataGridView).BeginInit();
            SuspendLayout();
            // 
            // closeCancelButton
            // 
            closeCancelButton.BackColor = Color.RosyBrown;
            closeCancelButton.FlatStyle = FlatStyle.Popup;
            closeCancelButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            closeCancelButton.Location = new Point(24, 428);
            closeCancelButton.Margin = new Padding(3, 2, 3, 2);
            closeCancelButton.Name = "closeCancelButton";
            closeCancelButton.Size = new Size(158, 28);
            closeCancelButton.TabIndex = 11;
            closeCancelButton.Text = "Close";
            closeCancelButton.UseVisualStyleBackColor = false;
            // 
            // importButton
            // 
            importButton.BackColor = Color.Transparent;
            importButton.Enabled = false;
            importButton.FlatStyle = FlatStyle.Popup;
            importButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            importButton.Location = new Point(307, 428);
            importButton.Margin = new Padding(3, 2, 3, 2);
            importButton.Name = "importButton";
            importButton.Size = new Size(159, 28);
            importButton.TabIndex = 12;
            importButton.Text = "Import";
            importButton.UseVisualStyleBackColor = false;
            importButton.Click += ImportButton_Click;
            // 
            // chooseFileButton
            // 
            chooseFileButton.BackColor = Color.SteelBlue;
            chooseFileButton.FlatStyle = FlatStyle.Popup;
            chooseFileButton.ForeColor = Color.Transparent;
            chooseFileButton.Location = new Point(5, 11);
            chooseFileButton.Margin = new Padding(3, 2, 3, 2);
            chooseFileButton.Name = "chooseFileButton";
            chooseFileButton.Size = new Size(65, 46);
            chooseFileButton.TabIndex = 14;
            chooseFileButton.Text = "Choose ADIF file";
            chooseFileButton.UseVisualStyleBackColor = false;
            chooseFileButton.Click += ChooseFileButton_Click;
            // 
            // filePathLabel
            // 
            filePathLabel.Location = new Point(74, 15);
            filePathLabel.Name = "filePathLabel";
            filePathLabel.Size = new Size(417, 42);
            filePathLabel.TabIndex = 15;
            filePathLabel.Text = "No file chosen";
            // 
            // filePreviewTextBox
            // 
            filePreviewTextBox.BackColor = Color.Silver;
            filePreviewTextBox.Location = new Point(11, 76);
            filePreviewTextBox.Margin = new Padding(3, 2, 3, 2);
            filePreviewTextBox.Multiline = true;
            filePreviewTextBox.Name = "filePreviewTextBox";
            filePreviewTextBox.PlaceholderText = "File preview will be here (limited to 32767 chars) ...";
            filePreviewTextBox.ReadOnly = true;
            filePreviewTextBox.ScrollBars = ScrollBars.Both;
            filePreviewTextBox.Size = new Size(479, 239);
            filePreviewTextBox.TabIndex = 16;
            filePreviewTextBox.WordWrap = false;
            // 
            // importBackgroundWorker
            // 
            importBackgroundWorker.WorkerReportsProgress = true;
            importBackgroundWorker.DoWork += ImportBackgroundWorker_DoWork;
            importBackgroundWorker.ProgressChanged += ImportBackgroundWorker_ProgressChanged;
            importBackgroundWorker.RunWorkerCompleted += ImportBackgroundWorker_RunWorkerCompleted;
            // 
            // importProgressBar
            // 
            importProgressBar.Location = new Point(11, 465);
            importProgressBar.Margin = new Padding(3, 2, 3, 2);
            importProgressBar.Name = "importProgressBar";
            importProgressBar.Size = new Size(123, 13);
            importProgressBar.Style = ProgressBarStyle.Marquee;
            importProgressBar.TabIndex = 17;
            importProgressBar.Visible = false;
            // 
            // importProgressStep
            // 
            importProgressStep.Location = new Point(140, 462);
            importProgressStep.Name = "importProgressStep";
            importProgressStep.Size = new Size(343, 19);
            importProgressStep.TabIndex = 18;
            importProgressStep.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // applyRuleDataGridView
            // 
            applyRuleDataGridView.AllowUserToAddRows = false;
            applyRuleDataGridView.AllowUserToDeleteRows = false;
            applyRuleDataGridView.BackgroundColor = SystemColors.ControlLightLight;
            applyRuleDataGridView.CellBorderStyle = DataGridViewCellBorderStyle.Raised;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = SystemColors.Window;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = Color.LightGray;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            applyRuleDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            applyRuleDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            applyRuleDataGridView.Columns.AddRange(new DataGridViewColumn[] { id, ruleName, isApplyForImport, ruleDetails });
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = SystemColors.Window;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle2.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = Color.LightGray;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.WindowText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            applyRuleDataGridView.DefaultCellStyle = dataGridViewCellStyle2;
            applyRuleDataGridView.Location = new Point(106, 323);
            applyRuleDataGridView.Name = "applyRuleDataGridView";
            applyRuleDataGridView.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = SystemColors.Control;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle3.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = Color.LightGray;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.WindowText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            applyRuleDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = SystemColors.Control;
            dataGridViewCellStyle4.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle4.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = Color.LightGray;
            dataGridViewCellStyle4.SelectionForeColor = SystemColors.WindowText;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.False;
            applyRuleDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle4;
            applyRuleDataGridView.Size = new Size(365, 91);
            applyRuleDataGridView.TabIndex = 19;
            applyRuleDataGridView.CellContentClick += ApplyRuleDataGridView_CellContentClick;
            // 
            // id
            // 
            id.DataPropertyName = "id";
            id.HeaderText = "Id";
            id.MaxInputLength = 10;
            id.Name = "id";
            id.ReadOnly = true;
            id.Visible = false;
            // 
            // ruleName
            // 
            ruleName.DataPropertyName = "name";
            ruleName.HeaderText = "Name";
            ruleName.MaxInputLength = 50;
            ruleName.Name = "ruleName";
            ruleName.ReadOnly = true;
            ruleName.Width = 200;
            // 
            // isApplyForImport
            // 
            isApplyForImport.DataPropertyName = "is_apply_for_import";
            isApplyForImport.HeaderText = "Apply?";
            isApplyForImport.Name = "isApplyForImport";
            isApplyForImport.ReadOnly = true;
            isApplyForImport.Width = 50;
            // 
            // ruleDetails
            // 
            ruleDetails.HeaderText = "Details";
            ruleDetails.Name = "ruleDetails";
            ruleDetails.ReadOnly = true;
            ruleDetails.Resizable = DataGridViewTriState.True;
            ruleDetails.SortMode = DataGridViewColumnSortMode.Automatic;
            ruleDetails.Text = "View";
            ruleDetails.ToolTipText = "Click to see more details";
            ruleDetails.UseColumnTextForButtonValue = true;
            ruleDetails.Width = 50;
            // 
            // RulesApplyLabel
            // 
            RulesApplyLabel.AutoSize = true;
            RulesApplyLabel.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            RulesApplyLabel.Location = new Point(11, 357);
            RulesApplyLabel.Name = "RulesApplyLabel";
            RulesApplyLabel.Size = new Size(88, 19);
            RulesApplyLabel.TabIndex = 20;
            RulesApplyLabel.Text = "Apply Rules";
            // 
            // QsoImportForm
            // 
            AccessibleRole = AccessibleRole.None;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = closeCancelButton;
            ClientSize = new Size(495, 495);
            Controls.Add(RulesApplyLabel);
            Controls.Add(applyRuleDataGridView);
            Controls.Add(importProgressStep);
            Controls.Add(importProgressBar);
            Controls.Add(filePreviewTextBox);
            Controls.Add(filePathLabel);
            Controls.Add(chooseFileButton);
            Controls.Add(importButton);
            Controls.Add(closeCancelButton);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Margin = new Padding(3, 2, 3, 2);
            MaximizeBox = false;
            Name = "QsoImportForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Import from ADIF";
            Load += QsoImportForm_Load;
            ((System.ComponentModel.ISupportInitialize)applyRuleDataGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button closeCancelButton;
        private Button importButton;
        private Button chooseFileButton;
        private Label filePathLabel;
        private TextBox filePreviewTextBox;
        private System.ComponentModel.BackgroundWorker importBackgroundWorker;
        private ProgressBar importProgressBar;
        private Label importProgressStep;
        private DataGridView applyRuleDataGridView;
        private BindingSource ruleBindingSource = new BindingSource();
        private SQLiteDataAdapter ruleDataAdapter = new SQLiteDataAdapter();
        private Label RulesApplyLabel;
        private DataGridViewTextBoxColumn id;
        private DataGridViewTextBoxColumn ruleName;
        private DataGridViewCheckBoxColumn isApplyForImport;
        private DataGridViewButtonColumn ruleDetails;
    }
}