namespace QSOCollector.Forms
{
    partial class RuleForm
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
            components = new System.ComponentModel.Container();
            nameLabel = new Label();
            nameTextBox = new TextBox();
            conditionGroupBox = new GroupBox();
            origFreqTo = new TextBox();
            andSourecFreqLabel = new Label();
            origFreqFrom = new TextBox();
            conditionSourecFreqLabel = new Label();
            populateGroupBox = new GroupBox();
            freqTxTextBox = new TextBox();
            FreqTxLabel = new Label();
            freqRxTextBox = new TextBox();
            freqRxLabel = new Label();
            bandRxComboBox = new ComboBox();
            bandRxLabel = new Label();
            bandTxComboBox = new ComboBox();
            bandTxLabel = new Label();
            satModeTextBox = new TextBox();
            satModeLabel = new Label();
            satNameTextBox = new TextBox();
            satNameLabel = new Label();
            propModeComboBox = new ComboBox();
            propModeLabel = new Label();
            bindingSource1 = new BindingSource(components);
            cancelButton = new Button();
            saveButton = new Button();
            conditionGroupBox.SuspendLayout();
            populateGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)bindingSource1).BeginInit();
            SuspendLayout();
            // 
            // nameLabel
            // 
            nameLabel.AutoSize = true;
            nameLabel.Location = new Point(10, 19);
            nameLabel.Name = "nameLabel";
            nameLabel.Size = new Size(39, 15);
            nameLabel.TabIndex = 1;
            nameLabel.Text = "Name";
            // 
            // nameTextBox
            // 
            nameTextBox.Location = new Point(56, 17);
            nameTextBox.Name = "nameTextBox";
            nameTextBox.Size = new Size(153, 23);
            nameTextBox.TabIndex = 2;
            nameTextBox.TextChanged += TextBox_TextChanged;
            // 
            // conditionGroupBox
            // 
            conditionGroupBox.Controls.Add(origFreqTo);
            conditionGroupBox.Controls.Add(andSourecFreqLabel);
            conditionGroupBox.Controls.Add(origFreqFrom);
            conditionGroupBox.Controls.Add(conditionSourecFreqLabel);
            conditionGroupBox.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            conditionGroupBox.Location = new Point(5, 55);
            conditionGroupBox.Name = "conditionGroupBox";
            conditionGroupBox.Size = new Size(463, 48);
            conditionGroupBox.TabIndex = 3;
            conditionGroupBox.TabStop = false;
            conditionGroupBox.Text = "Condition";
            // 
            // origFreqTo
            // 
            origFreqTo.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            origFreqTo.Location = new Point(333, 17);
            origFreqTo.Name = "origFreqTo";
            origFreqTo.Size = new Size(60, 23);
            origFreqTo.TabIndex = 3;
            origFreqTo.TextChanged += TextBox_TextChanged;
            origFreqTo.KeyPress += Freq_KeyPress;
            // 
            // andSourecFreqLabel
            // 
            andSourecFreqLabel.AutoSize = true;
            andSourecFreqLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            andSourecFreqLabel.Location = new Point(300, 22);
            andSourecFreqLabel.Name = "andSourecFreqLabel";
            andSourecFreqLabel.Size = new Size(27, 15);
            andSourecFreqLabel.TabIndex = 2;
            andSourecFreqLabel.Text = "and";
            // 
            // origFreqFrom
            // 
            origFreqFrom.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            origFreqFrom.Location = new Point(230, 17);
            origFreqFrom.Name = "origFreqFrom";
            origFreqFrom.Size = new Size(60, 23);
            origFreqFrom.TabIndex = 1;
            origFreqFrom.TextChanged += TextBox_TextChanged;
            origFreqFrom.KeyPress += Freq_KeyPress;
            // 
            // conditionSourecFreqLabel
            // 
            conditionSourecFreqLabel.AutoSize = true;
            conditionSourecFreqLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            conditionSourecFreqLabel.Location = new Point(8, 22);
            conditionSourecFreqLabel.Name = "conditionSourecFreqLabel";
            conditionSourecFreqLabel.Size = new Size(208, 15);
            conditionSourecFreqLabel.TabIndex = 0;
            conditionSourecFreqLabel.Text = "If Source TX frequency between, mHz";
            // 
            // populateGroupBox
            // 
            populateGroupBox.Controls.Add(freqTxTextBox);
            populateGroupBox.Controls.Add(FreqTxLabel);
            populateGroupBox.Controls.Add(freqRxTextBox);
            populateGroupBox.Controls.Add(freqRxLabel);
            populateGroupBox.Controls.Add(bandRxComboBox);
            populateGroupBox.Controls.Add(bandRxLabel);
            populateGroupBox.Controls.Add(bandTxComboBox);
            populateGroupBox.Controls.Add(bandTxLabel);
            populateGroupBox.Controls.Add(satModeTextBox);
            populateGroupBox.Controls.Add(satModeLabel);
            populateGroupBox.Controls.Add(satNameTextBox);
            populateGroupBox.Controls.Add(satNameLabel);
            populateGroupBox.Controls.Add(propModeComboBox);
            populateGroupBox.Controls.Add(propModeLabel);
            populateGroupBox.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            populateGroupBox.Location = new Point(9, 109);
            populateGroupBox.Name = "populateGroupBox";
            populateGroupBox.Size = new Size(459, 154);
            populateGroupBox.TabIndex = 4;
            populateGroupBox.TabStop = false;
            populateGroupBox.Text = "Then populate/override ADIF fields";
            // 
            // freqTxTextBox
            // 
            freqTxTextBox.AutoCompleteMode = AutoCompleteMode.Suggest;
            freqTxTextBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            freqTxTextBox.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            freqTxTextBox.Location = new Point(143, 112);
            freqTxTextBox.Name = "freqTxTextBox";
            freqTxTextBox.Size = new Size(70, 23);
            freqTxTextBox.TabIndex = 11;
            freqTxTextBox.TextChanged += TextBox_TextChanged;
            freqTxTextBox.KeyPress += Freq_KeyPress;
            // 
            // FreqTxLabel
            // 
            FreqTxLabel.AutoSize = true;
            FreqTxLabel.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FreqTxLabel.Location = new Point(5, 117);
            FreqTxLabel.Name = "FreqTxLabel";
            FreqTxLabel.Size = new Size(136, 13);
            FreqTxLabel.TabIndex = 10;
            FreqTxLabel.Text = "Frequency TX mHz (FREQ)";
            // 
            // freqRxTextBox
            // 
            freqRxTextBox.AutoCompleteMode = AutoCompleteMode.Suggest;
            freqRxTextBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            freqRxTextBox.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            freqRxTextBox.Location = new Point(383, 114);
            freqRxTextBox.Name = "freqRxTextBox";
            freqRxTextBox.Size = new Size(70, 23);
            freqRxTextBox.TabIndex = 13;
            freqRxTextBox.TextChanged += TextBox_TextChanged;
            freqRxTextBox.KeyPress += Freq_KeyPress;
            // 
            // freqRxLabel
            // 
            freqRxLabel.AutoSize = true;
            freqRxLabel.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            freqRxLabel.Location = new Point(222, 117);
            freqRxLabel.Name = "freqRxLabel";
            freqRxLabel.Size = new Size(155, 13);
            freqRxLabel.TabIndex = 12;
            freqRxLabel.Text = "Frequency RX mHz (FREQ_RX)";
            // 
            // bandRxComboBox
            // 
            bandRxComboBox.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            bandRxComboBox.FormattingEnabled = true;
            bandRxComboBox.Location = new Point(338, 83);
            bandRxComboBox.Name = "bandRxComboBox";
            bandRxComboBox.Size = new Size(115, 23);
            bandRxComboBox.TabIndex = 9;
            bandRxComboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            // 
            // bandRxLabel
            // 
            bandRxLabel.AutoSize = true;
            bandRxLabel.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            bandRxLabel.Location = new Point(222, 87);
            bandRxLabel.Name = "bandRxLabel";
            bandRxLabel.Size = new Size(105, 13);
            bandRxLabel.TabIndex = 8;
            bandRxLabel.Text = "Band RX (BAND_RX)";
            // 
            // bandTxComboBox
            // 
            bandTxComboBox.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            bandTxComboBox.FormattingEnabled = true;
            bandTxComboBox.Location = new Point(95, 82);
            bandTxComboBox.Name = "bandTxComboBox";
            bandTxComboBox.Size = new Size(106, 23);
            bandTxComboBox.TabIndex = 7;
            bandTxComboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            // 
            // bandTxLabel
            // 
            bandTxLabel.AutoSize = true;
            bandTxLabel.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            bandTxLabel.Location = new Point(5, 87);
            bandTxLabel.Name = "bandTxLabel";
            bandTxLabel.Size = new Size(86, 13);
            bandTxLabel.TabIndex = 6;
            bandTxLabel.Text = "Band TX (BAND)";
            // 
            // satModeTextBox
            // 
            satModeTextBox.AutoCompleteMode = AutoCompleteMode.Suggest;
            satModeTextBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            satModeTextBox.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            satModeTextBox.Location = new Point(392, 49);
            satModeTextBox.Name = "satModeTextBox";
            satModeTextBox.Size = new Size(53, 23);
            satModeTextBox.TabIndex = 5;
            satModeTextBox.TextChanged += SatModeTextBox_TextChanged;
            satModeTextBox.KeyPress += SatModeTextBox_KeyPress;
            // 
            // satModeLabel
            // 
            satModeLabel.AutoSize = true;
            satModeLabel.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            satModeLabel.Location = new Point(245, 53);
            satModeLabel.Name = "satModeLabel";
            satModeLabel.Size = new Size(145, 13);
            satModeLabel.TabIndex = 4;
            satModeLabel.Text = "Satellite mode (SAT_MODE)";
            // 
            // satNameTextBox
            // 
            satNameTextBox.AutoCompleteCustomSource.AddRange(new string[] { "QO-100", "SO-50", "RS-44", "ISS", "FO-29", "AO-7", "AO-73" });
            satNameTextBox.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            satNameTextBox.Location = new Point(158, 48);
            satNameTextBox.Name = "satNameTextBox";
            satNameTextBox.Size = new Size(71, 23);
            satNameTextBox.TabIndex = 3;
            satNameTextBox.Text = "QO-100";
            satNameTextBox.TextChanged += SatNameTextBox_TextChanged;
            satNameTextBox.KeyPress += SatNameTextBox_KeyPress;
            // 
            // satNameLabel
            // 
            satNameLabel.AutoSize = true;
            satNameLabel.Font = new Font("Segoe UI Semibold", 7.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            satNameLabel.Location = new Point(7, 53);
            satNameLabel.Name = "satNameLabel";
            satNameLabel.Size = new Size(144, 13);
            satNameLabel.TabIndex = 2;
            satNameLabel.Text = "Satellite name (SAT_NAME)";
            // 
            // propModeComboBox
            // 
            propModeComboBox.BackColor = SystemColors.Window;
            propModeComboBox.Enabled = false;
            propModeComboBox.FlatStyle = FlatStyle.Flat;
            propModeComboBox.FormattingEnabled = true;
            propModeComboBox.Items.AddRange(new object[] { "SAT" });
            propModeComboBox.Location = new Point(192, 17);
            propModeComboBox.Name = "propModeComboBox";
            propModeComboBox.Size = new Size(94, 23);
            propModeComboBox.TabIndex = 1;
            propModeComboBox.Text = "SAT";
            propModeComboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            // 
            // propModeLabel
            // 
            propModeLabel.AutoSize = true;
            propModeLabel.Font = new Font("Segoe UI Semibold", 7.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            propModeLabel.Location = new Point(5, 22);
            propModeLabel.Name = "propModeLabel";
            propModeLabel.Size = new Size(178, 13);
            propModeLabel.TabIndex = 0;
            propModeLabel.Text = "Propagation Mode (PROP_MODE)";
            // 
            // cancelButton
            // 
            cancelButton.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            cancelButton.Location = new Point(120, 264);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(101, 27);
            cancelButton.TabIndex = 5;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            // 
            // saveButton
            // 
            saveButton.Enabled = false;
            saveButton.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            saveButton.Location = new Point(286, 264);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(104, 27);
            saveButton.TabIndex = 6;
            saveButton.Text = "Save";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += SaveButton_Click;
            // 
            // RuleForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new Size(472, 298);
            Controls.Add(saveButton);
            Controls.Add(cancelButton);
            Controls.Add(populateGroupBox);
            Controls.Add(conditionGroupBox);
            Controls.Add(nameTextBox);
            Controls.Add(nameLabel);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "RuleForm";
            Text = "Rule";
            Load += RuleForm_Load;
            conditionGroupBox.ResumeLayout(false);
            conditionGroupBox.PerformLayout();
            populateGroupBox.ResumeLayout(false);
            populateGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)bindingSource1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox nameTextBox;
        private GroupBox conditionGroupBox;
        private Label nameLabel;
        private Label conditionSourecFreqLabel;
        private Label andSourecFreqLabel;
        private TextBox origFreqFrom;
        private TextBox origFreqTo;
        private GroupBox populateGroupBox;
        private Label propModeLabel;
        private TextBox satNameTextBox;
        private Label satNameLabel;
        private ComboBox propModeComboBox;
        private Label satModeLabel;
        private TextBox satModeTextBox;
        private ComboBox bandTxComboBox;
        private Label bandTxLabel;
        private Label bandRxLabel;
        private ComboBox bandRxComboBox;
        private BindingSource bindingSource1;
        private Label freqRxLabel;
        private TextBox freqRxTextBox;
        private TextBox freqTxTextBox;
        private Label FreqTxLabel;
        private Button cancelButton;
        private Button saveButton;
    }
}