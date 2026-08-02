using System.Data.SQLite;

namespace QSOCollector.Forms
{
    partial class ListenerRulesForm
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
            listenerRuleDataGridView = new DataGridView();
            id = new DataGridViewTextBoxColumn();
            ruleName = new DataGridViewTextBoxColumn();
            origFreqFrom = new DataGridViewTextBoxColumn();
            origFreqTo = new DataGridViewTextBoxColumn();
            propagationMode = new DataGridViewTextBoxColumn();
            satName = new DataGridViewTextBoxColumn();
            satMode = new DataGridViewTextBoxColumn();
            targetBand = new DataGridViewTextBoxColumn();
            targetBandRx = new DataGridViewTextBoxColumn();
            targetfreqTx = new DataGridViewTextBoxColumn();
            targetFreqRx = new DataGridViewTextBoxColumn();
            isActive = new DataGridViewCheckBoxColumn();
            assignRuleButton = new Button();
            unassignRulesButton = new Button();
            unassignRuleHintLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)listenerRuleDataGridView).BeginInit();
            SuspendLayout();
            // 
            // listenerRuleDataGridView
            // 
            listenerRuleDataGridView.AllowUserToAddRows = false;
            listenerRuleDataGridView.AllowUserToDeleteRows = false;
            listenerRuleDataGridView.BackgroundColor = SystemColors.ControlLightLight;
            listenerRuleDataGridView.CellBorderStyle = DataGridViewCellBorderStyle.Raised;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = SystemColors.Window;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = Color.LightGray;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            listenerRuleDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            listenerRuleDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            listenerRuleDataGridView.Columns.AddRange(new DataGridViewColumn[] { id, ruleName, origFreqFrom, origFreqTo, propagationMode, satName, satMode, targetBand, targetBandRx, targetfreqTx, targetFreqRx, isActive });
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = SystemColors.Window;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle2.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = Color.LightGray;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.WindowText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            listenerRuleDataGridView.DefaultCellStyle = dataGridViewCellStyle2;
            listenerRuleDataGridView.Location = new Point(3, 4);
            listenerRuleDataGridView.Name = "listenerRuleDataGridView";
            listenerRuleDataGridView.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = SystemColors.Control;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle3.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = Color.LightGray;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.WindowText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            listenerRuleDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = SystemColors.Control;
            dataGridViewCellStyle4.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle4.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = Color.LightGray;
            dataGridViewCellStyle4.SelectionForeColor = SystemColors.WindowText;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.False;
            listenerRuleDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle4;
            listenerRuleDataGridView.Size = new Size(832, 150);
            listenerRuleDataGridView.TabIndex = 1;
            listenerRuleDataGridView.SelectionChanged += ListenerRuleDataGridView_SelectionChanged;
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
            ruleName.Width = 130;
            // 
            // origFreqFrom
            // 
            origFreqFrom.DataPropertyName = "orig_freq_mhz_from";
            origFreqFrom.HeaderText = "Freq from, mHz";
            origFreqFrom.MaxInputLength = 10;
            origFreqFrom.Name = "origFreqFrom";
            origFreqFrom.ReadOnly = true;
            origFreqFrom.Resizable = DataGridViewTriState.False;
            origFreqFrom.Width = 60;
            // 
            // origFreqTo
            // 
            origFreqTo.DataPropertyName = "orig_freq_mhz_to";
            origFreqTo.HeaderText = "Freq to, mHz";
            origFreqTo.MaxInputLength = 10;
            origFreqTo.Name = "origFreqTo";
            origFreqTo.ReadOnly = true;
            origFreqTo.Resizable = DataGridViewTriState.False;
            origFreqTo.Width = 60;
            // 
            // propagationMode
            // 
            propagationMode.DataPropertyName = "propagation_mode";
            propagationMode.HeaderText = "Propagation Mode";
            propagationMode.MaxInputLength = 10;
            propagationMode.Name = "propagationMode";
            propagationMode.ReadOnly = true;
            propagationMode.Resizable = DataGridViewTriState.True;
            propagationMode.Width = 80;
            // 
            // satName
            // 
            satName.DataPropertyName = "sat_name";
            satName.HeaderText = "Satelite name";
            satName.MaxInputLength = 50;
            satName.Name = "satName";
            satName.ReadOnly = true;
            satName.Width = 70;
            // 
            // satMode
            // 
            satMode.DataPropertyName = "sat_mode";
            satMode.HeaderText = "SAT mode";
            satMode.MaxInputLength = 10;
            satMode.Name = "satMode";
            satMode.ReadOnly = true;
            satMode.Width = 40;
            // 
            // targetBand
            // 
            targetBand.DataPropertyName = "band";
            targetBand.HeaderText = "New Band TX";
            targetBand.MaxInputLength = 10;
            targetBand.Name = "targetBand";
            targetBand.ReadOnly = true;
            targetBand.Width = 50;
            // 
            // targetBandRx
            // 
            targetBandRx.DataPropertyName = "band_rx";
            targetBandRx.HeaderText = "New Band RX";
            targetBandRx.MaxInputLength = 10;
            targetBandRx.Name = "targetBandRx";
            targetBandRx.ReadOnly = true;
            targetBandRx.Width = 50;
            // 
            // targetfreqTx
            // 
            targetfreqTx.DataPropertyName = "freq_mhz";
            targetfreqTx.HeaderText = "New Freq TX, mHz";
            targetfreqTx.MaxInputLength = 10;
            targetfreqTx.Name = "targetfreqTx";
            targetfreqTx.ReadOnly = true;
            targetfreqTx.Width = 80;
            // 
            // targetFreqRx
            // 
            targetFreqRx.DataPropertyName = "freq_mhz_rx";
            targetFreqRx.HeaderText = "New Freq RX, mHz";
            targetFreqRx.MaxInputLength = 10;
            targetFreqRx.Name = "targetFreqRx";
            targetFreqRx.ReadOnly = true;
            targetFreqRx.Width = 80;
            // 
            // isActive
            // 
            isActive.DataPropertyName = "is_active";
            isActive.HeaderText = "Is Rule Active?";
            isActive.Name = "isActive";
            isActive.ReadOnly = true;
            isActive.Width = 50;
            // 
            // assignRuleButton
            // 
            assignRuleButton.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            assignRuleButton.Location = new Point(189, 171);
            assignRuleButton.Name = "assignRuleButton";
            assignRuleButton.Size = new Size(148, 37);
            assignRuleButton.TabIndex = 2;
            assignRuleButton.Text = "Assign new Rule";
            assignRuleButton.UseVisualStyleBackColor = true;
            assignRuleButton.Click += AssignRuleButton_Click;
            // 
            // unassignRulesButton
            // 
            unassignRulesButton.Enabled = false;
            unassignRulesButton.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            unassignRulesButton.Location = new Point(442, 171);
            unassignRulesButton.Name = "unassignRulesButton";
            unassignRulesButton.Size = new Size(189, 37);
            unassignRulesButton.TabIndex = 3;
            unassignRulesButton.Text = "Unassign selected Rules";
            unassignRulesButton.UseVisualStyleBackColor = true;
            unassignRulesButton.Click += UnassignRulesButton_Click;
            // 
            // unassignRuleHintLabel
            // 
            unassignRuleHintLabel.AutoSize = true;
            unassignRuleHintLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            unassignRuleHintLabel.Location = new Point(453, 216);
            unassignRuleHintLabel.Name = "unassignRuleHintLabel";
            unassignRuleHintLabel.Size = new Size(183, 15);
            unassignRuleHintLabel.TabIndex = 8;
            unassignRuleHintLabel.Text = "(Select one Rule row to unassign)";
            // 
            // ListenerRulesForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(837, 250);
            Controls.Add(unassignRuleHintLabel);
            Controls.Add(unassignRulesButton);
            Controls.Add(assignRuleButton);
            Controls.Add(listenerRuleDataGridView);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ListenerRulesForm";
            Text = "Listener SAT Rules";
            Load += ListenerRulesForm_Load;
            ((System.ComponentModel.ISupportInitialize)listenerRuleDataGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView listenerRuleDataGridView;
        private Button assignRuleButton;
        private Button unassignRulesButton;
        private BindingSource listenerRuleBindingSource = new BindingSource();
        private SQLiteDataAdapter listenerRuleDataAdapter = new SQLiteDataAdapter();
        private DataGridViewTextBoxColumn id;
        private DataGridViewTextBoxColumn ruleName;
        private DataGridViewTextBoxColumn origFreqFrom;
        private DataGridViewTextBoxColumn origFreqTo;
        private DataGridViewTextBoxColumn propagationMode;
        private DataGridViewTextBoxColumn satName;
        private DataGridViewTextBoxColumn satMode;
        private DataGridViewTextBoxColumn targetBand;
        private DataGridViewTextBoxColumn targetBandRx;
        private DataGridViewTextBoxColumn targetfreqTx;
        private DataGridViewTextBoxColumn targetFreqRx;
        private DataGridViewCheckBoxColumn isActive;
        private Label unassignRuleHintLabel;
    }
}