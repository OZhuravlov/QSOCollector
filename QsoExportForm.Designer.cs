namespace QSOCollector
{
    partial class QsoExportForm
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
            mainFilterGroupBox = new GroupBox();
            dateToTimePicker = new DateTimePicker();
            exportQsoDateToLabel = new Label();
            exportQsoDateFromLabel = new Label();
            allQSOsRadioButton = new RadioButton();
            dateFromTimePicker = new DateTimePicker();
            byDateRadioButton = new RadioButton();
            newQSOsRadioButton = new RadioButton();
            amountsGroupBox = new GroupBox();
            filteredAmountLabel = new Label();
            totalLabel = new Label();
            filteredLabel = new Label();
            totalAmountLabel = new Label();
            secondaryFiltersGroupBox = new GroupBox();
            sourceIpComboBox = new ComboBox();
            sourceIpLabel = new Label();
            programIdComboBox = new ComboBox();
            programIdLabel = new Label();
            operatorComboBox = new ComboBox();
            OperatorLabel = new Label();
            modeGroupComboBox = new ComboBox();
            modeGroupLabel = new Label();
            bandComboBox = new ComboBox();
            bandLabel = new Label();
            modeComboBox = new ComboBox();
            modeLabel = new Label();
            resetSecondaryFiltersButton = new Button();
            exportButton = new Button();
            exportProgressBar = new ProgressBar();
            mainFilterGroupBox.SuspendLayout();
            amountsGroupBox.SuspendLayout();
            secondaryFiltersGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // mainFilterGroupBox
            // 
            mainFilterGroupBox.Controls.Add(dateToTimePicker);
            mainFilterGroupBox.Controls.Add(exportQsoDateToLabel);
            mainFilterGroupBox.Controls.Add(exportQsoDateFromLabel);
            mainFilterGroupBox.Controls.Add(allQSOsRadioButton);
            mainFilterGroupBox.Controls.Add(dateFromTimePicker);
            mainFilterGroupBox.Controls.Add(byDateRadioButton);
            mainFilterGroupBox.Controls.Add(newQSOsRadioButton);
            mainFilterGroupBox.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            mainFilterGroupBox.Location = new Point(5, 9);
            mainFilterGroupBox.Name = "mainFilterGroupBox";
            mainFilterGroupBox.Size = new Size(399, 114);
            mainFilterGroupBox.TabIndex = 0;
            mainFilterGroupBox.TabStop = false;
            mainFilterGroupBox.Text = "Main filter";
            // 
            // dateToTimePicker
            // 
            dateToTimePicker.Checked = false;
            dateToTimePicker.Enabled = false;
            dateToTimePicker.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dateToTimePicker.Format = DateTimePickerFormat.Short;
            dateToTimePicker.Location = new Point(218, 63);
            dateToTimePicker.Name = "dateToTimePicker";
            dateToTimePicker.Size = new Size(138, 27);
            dateToTimePicker.TabIndex = 4;
            dateToTimePicker.ValueChanged += dateToTimePicker_ValueChanged;
            dateToTimePicker.EnabledChanged += dateToTimePicker_EnabledChanged;
            // 
            // exportQsoDateToLabel
            // 
            exportQsoDateToLabel.AutoSize = true;
            exportQsoDateToLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            exportQsoDateToLabel.Location = new Point(131, 65);
            exportQsoDateToLabel.Name = "exportQsoDateToLabel";
            exportQsoDateToLabel.Size = new Size(61, 20);
            exportQsoDateToLabel.TabIndex = 3;
            exportQsoDateToLabel.Text = "Date To";
            // 
            // exportQsoDateFromLabel
            // 
            exportQsoDateFromLabel.AutoSize = true;
            exportQsoDateFromLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            exportQsoDateFromLabel.Location = new Point(131, 34);
            exportQsoDateFromLabel.Name = "exportQsoDateFromLabel";
            exportQsoDateFromLabel.Size = new Size(79, 20);
            exportQsoDateFromLabel.TabIndex = 2;
            exportQsoDateFromLabel.Text = "Date From";
            // 
            // allQSOsRadioButton
            // 
            allQSOsRadioButton.AutoSize = true;
            allQSOsRadioButton.Font = new Font("Segoe UI", 9F);
            allQSOsRadioButton.Location = new Point(6, 84);
            allQSOsRadioButton.Name = "allQSOsRadioButton";
            allQSOsRadioButton.Size = new Size(88, 24);
            allQSOsRadioButton.TabIndex = 2;
            allQSOsRadioButton.TabStop = true;
            allQSOsRadioButton.Text = "All QSOs";
            allQSOsRadioButton.UseVisualStyleBackColor = true;
            allQSOsRadioButton.CheckedChanged += radioButtons_CheckedChanged;
            // 
            // dateFromTimePicker
            // 
            dateFromTimePicker.Checked = false;
            dateFromTimePicker.Enabled = false;
            dateFromTimePicker.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dateFromTimePicker.Format = DateTimePickerFormat.Short;
            dateFromTimePicker.Location = new Point(218, 32);
            dateFromTimePicker.Name = "dateFromTimePicker";
            dateFromTimePicker.Size = new Size(138, 27);
            dateFromTimePicker.TabIndex = 1;
            dateFromTimePicker.ValueChanged += dateFromTimePicker_ValueChanged;
            dateFromTimePicker.EnabledChanged += dateFromTimePicker_EnabledChanged;
            // 
            // byDateRadioButton
            // 
            byDateRadioButton.AutoSize = true;
            byDateRadioButton.Font = new Font("Segoe UI", 9F);
            byDateRadioButton.Location = new Point(7, 55);
            byDateRadioButton.Name = "byDateRadioButton";
            byDateRadioButton.Size = new Size(80, 24);
            byDateRadioButton.TabIndex = 1;
            byDateRadioButton.TabStop = true;
            byDateRadioButton.Text = "By date";
            byDateRadioButton.UseVisualStyleBackColor = true;
            byDateRadioButton.CheckedChanged += radioButtons_CheckedChanged;
            // 
            // newQSOsRadioButton
            // 
            newQSOsRadioButton.AutoSize = true;
            newQSOsRadioButton.Checked = true;
            newQSOsRadioButton.Font = new Font("Segoe UI", 9F);
            newQSOsRadioButton.Location = new Point(6, 25);
            newQSOsRadioButton.Name = "newQSOsRadioButton";
            newQSOsRadioButton.Size = new Size(100, 24);
            newQSOsRadioButton.TabIndex = 0;
            newQSOsRadioButton.TabStop = true;
            newQSOsRadioButton.Text = "New QSOs";
            newQSOsRadioButton.UseVisualStyleBackColor = true;
            newQSOsRadioButton.CheckedChanged += radioButtons_CheckedChanged;
            // 
            // amountsGroupBox
            // 
            amountsGroupBox.Controls.Add(filteredAmountLabel);
            amountsGroupBox.Controls.Add(totalLabel);
            amountsGroupBox.Controls.Add(filteredLabel);
            amountsGroupBox.Controls.Add(totalAmountLabel);
            amountsGroupBox.Location = new Point(9, 332);
            amountsGroupBox.Name = "amountsGroupBox";
            amountsGroupBox.Size = new Size(399, 53);
            amountsGroupBox.TabIndex = 1;
            amountsGroupBox.TabStop = false;
            amountsGroupBox.Text = "QSO Amount";
            // 
            // filteredAmountLabel
            // 
            filteredAmountLabel.AutoSize = true;
            filteredAmountLabel.Location = new Point(90, 23);
            filteredAmountLabel.Name = "filteredAmountLabel";
            filteredAmountLabel.Size = new Size(17, 20);
            filteredAmountLabel.TabIndex = 3;
            filteredAmountLabel.Text = "0";
            // 
            // totalLabel
            // 
            totalLabel.AutoSize = true;
            totalLabel.Location = new Point(208, 23);
            totalLabel.Name = "totalLabel";
            totalLabel.RightToLeft = RightToLeft.No;
            totalLabel.Size = new Size(60, 20);
            totalLabel.TabIndex = 2;
            totalLabel.Text = "of Total";
            // 
            // filteredLabel
            // 
            filteredLabel.AutoSize = true;
            filteredLabel.Location = new Point(6, 23);
            filteredLabel.Name = "filteredLabel";
            filteredLabel.Size = new Size(59, 20);
            filteredLabel.TabIndex = 1;
            filteredLabel.Text = "Filtered";
            // 
            // totalAmountLabel
            // 
            totalAmountLabel.AutoSize = true;
            totalAmountLabel.Location = new Point(288, 23);
            totalAmountLabel.Name = "totalAmountLabel";
            totalAmountLabel.Size = new Size(17, 20);
            totalAmountLabel.TabIndex = 0;
            totalAmountLabel.Text = "0";
            // 
            // secondaryFiltersGroupBox
            // 
            secondaryFiltersGroupBox.Controls.Add(sourceIpComboBox);
            secondaryFiltersGroupBox.Controls.Add(sourceIpLabel);
            secondaryFiltersGroupBox.Controls.Add(programIdComboBox);
            secondaryFiltersGroupBox.Controls.Add(programIdLabel);
            secondaryFiltersGroupBox.Controls.Add(operatorComboBox);
            secondaryFiltersGroupBox.Controls.Add(OperatorLabel);
            secondaryFiltersGroupBox.Controls.Add(modeGroupComboBox);
            secondaryFiltersGroupBox.Controls.Add(modeGroupLabel);
            secondaryFiltersGroupBox.Controls.Add(bandComboBox);
            secondaryFiltersGroupBox.Controls.Add(bandLabel);
            secondaryFiltersGroupBox.Controls.Add(modeComboBox);
            secondaryFiltersGroupBox.Controls.Add(modeLabel);
            secondaryFiltersGroupBox.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            secondaryFiltersGroupBox.Location = new Point(5, 129);
            secondaryFiltersGroupBox.Name = "secondaryFiltersGroupBox";
            secondaryFiltersGroupBox.Size = new Size(399, 123);
            secondaryFiltersGroupBox.TabIndex = 2;
            secondaryFiltersGroupBox.TabStop = false;
            secondaryFiltersGroupBox.Text = "Secondary filters";
            // 
            // sourceIpComboBox
            // 
            sourceIpComboBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            sourceIpComboBox.FormattingEnabled = true;
            sourceIpComboBox.Location = new Point(283, 85);
            sourceIpComboBox.Name = "sourceIpComboBox";
            sourceIpComboBox.Size = new Size(105, 28);
            sourceIpComboBox.TabIndex = 10;
            sourceIpComboBox.SelectedValueChanged += sourceIpComboBox_SelectedValueChanged;
            // 
            // sourceIpLabel
            // 
            sourceIpLabel.AutoSize = true;
            sourceIpLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            sourceIpLabel.Location = new Point(208, 91);
            sourceIpLabel.Name = "sourceIpLabel";
            sourceIpLabel.Size = new Size(70, 20);
            sourceIpLabel.TabIndex = 9;
            sourceIpLabel.Text = "Source IP";
            // 
            // programIdComboBox
            // 
            programIdComboBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            programIdComboBox.FormattingEnabled = true;
            programIdComboBox.ItemHeight = 20;
            programIdComboBox.Location = new Point(283, 25);
            programIdComboBox.Name = "programIdComboBox";
            programIdComboBox.Size = new Size(105, 28);
            programIdComboBox.TabIndex = 3;
            programIdComboBox.SelectedValueChanged += programIdComboBox_SelectedValueChanged;
            // 
            // programIdLabel
            // 
            programIdLabel.AutoSize = true;
            programIdLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            programIdLabel.Location = new Point(212, 28);
            programIdLabel.Name = "programIdLabel";
            programIdLabel.Size = new Size(66, 20);
            programIdLabel.TabIndex = 1;
            programIdLabel.Text = "Program";
            // 
            // operatorComboBox
            // 
            operatorComboBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            operatorComboBox.FormattingEnabled = true;
            operatorComboBox.Location = new Point(283, 55);
            operatorComboBox.Name = "operatorComboBox";
            operatorComboBox.Size = new Size(105, 28);
            operatorComboBox.TabIndex = 8;
            operatorComboBox.SelectedValueChanged += operatorComboBox_SelectedValueChanged;
            // 
            // OperatorLabel
            // 
            OperatorLabel.AutoSize = true;
            OperatorLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            OperatorLabel.Location = new Point(209, 59);
            OperatorLabel.Name = "OperatorLabel";
            OperatorLabel.Size = new Size(69, 20);
            OperatorLabel.TabIndex = 7;
            OperatorLabel.Text = "Operator";
            // 
            // modeGroupComboBox
            // 
            modeGroupComboBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            modeGroupComboBox.FormattingEnabled = true;
            modeGroupComboBox.Location = new Point(96, 24);
            modeGroupComboBox.Name = "modeGroupComboBox";
            modeGroupComboBox.Size = new Size(106, 28);
            modeGroupComboBox.TabIndex = 0;
            modeGroupComboBox.SelectedValueChanged += modeGroupComboBox_SelectedValueChanged;
            // 
            // modeGroupLabel
            // 
            modeGroupLabel.AutoSize = true;
            modeGroupLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            modeGroupLabel.Location = new Point(3, 28);
            modeGroupLabel.Name = "modeGroupLabel";
            modeGroupLabel.Size = new Size(92, 20);
            modeGroupLabel.TabIndex = 6;
            modeGroupLabel.Text = "Mode group";
            // 
            // bandComboBox
            // 
            bandComboBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            bandComboBox.FormattingEnabled = true;
            bandComboBox.Location = new Point(96, 84);
            bandComboBox.Name = "bandComboBox";
            bandComboBox.Size = new Size(105, 28);
            bandComboBox.TabIndex = 2;
            bandComboBox.SelectedValueChanged += bandComboBox_SelectedValueChanged;
            // 
            // bandLabel
            // 
            bandLabel.AutoSize = true;
            bandLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            bandLabel.Location = new Point(5, 87);
            bandLabel.Name = "bandLabel";
            bandLabel.Size = new Size(43, 20);
            bandLabel.TabIndex = 4;
            bandLabel.Text = "Band";
            // 
            // modeComboBox
            // 
            modeComboBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            modeComboBox.FormattingEnabled = true;
            modeComboBox.Location = new Point(96, 54);
            modeComboBox.Name = "modeComboBox";
            modeComboBox.Size = new Size(106, 28);
            modeComboBox.TabIndex = 1;
            modeComboBox.SelectedValueChanged += modeComboBox_SelectedValueChanged;
            // 
            // modeLabel
            // 
            modeLabel.AutoSize = true;
            modeLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            modeLabel.Location = new Point(4, 58);
            modeLabel.Name = "modeLabel";
            modeLabel.Size = new Size(48, 20);
            modeLabel.TabIndex = 2;
            modeLabel.Text = "Mode";
            // 
            // resetSecondaryFiltersButton
            // 
            resetSecondaryFiltersButton.BackColor = Color.RosyBrown;
            resetSecondaryFiltersButton.FlatStyle = FlatStyle.Popup;
            resetSecondaryFiltersButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            resetSecondaryFiltersButton.Location = new Point(12, 258);
            resetSecondaryFiltersButton.Name = "resetSecondaryFiltersButton";
            resetSecondaryFiltersButton.Size = new Size(185, 38);
            resetSecondaryFiltersButton.TabIndex = 11;
            resetSecondaryFiltersButton.Text = "Clear Secondary filters";
            resetSecondaryFiltersButton.UseVisualStyleBackColor = false;
            resetSecondaryFiltersButton.Click += resetSecondaryFiltersButton_Click;
            // 
            // exportButton
            // 
            exportButton.BackColor = Color.DarkSeaGreen;
            exportButton.FlatStyle = FlatStyle.Popup;
            exportButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            exportButton.Location = new Point(213, 258);
            exportButton.Name = "exportButton";
            exportButton.Size = new Size(180, 38);
            exportButton.TabIndex = 12;
            exportButton.Text = "Export";
            exportButton.UseVisualStyleBackColor = false;
            exportButton.Click += exportButton_Click;
            // 
            // exportProgressBar
            // 
            exportProgressBar.Location = new Point(25, 307);
            exportProgressBar.Name = "exportProgressBar";
            exportProgressBar.Size = new Size(349, 19);
            exportProgressBar.TabIndex = 13;
            exportProgressBar.Visible = false;
            // 
            // QsoExportForm
            // 
            AccessibleRole = AccessibleRole.None;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(409, 387);
            Controls.Add(exportProgressBar);
            Controls.Add(exportButton);
            Controls.Add(resetSecondaryFiltersButton);
            Controls.Add(secondaryFiltersGroupBox);
            Controls.Add(amountsGroupBox);
            Controls.Add(mainFilterGroupBox);
            MaximizeBox = false;
            Name = "QsoExportForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Export to ADIF";
            mainFilterGroupBox.ResumeLayout(false);
            mainFilterGroupBox.PerformLayout();
            amountsGroupBox.ResumeLayout(false);
            amountsGroupBox.PerformLayout();
            secondaryFiltersGroupBox.ResumeLayout(false);
            secondaryFiltersGroupBox.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox mainFilterGroupBox;
        private RadioButton newQSOsRadioButton;
        private RadioButton byDateRadioButton;
        private RadioButton allQSOsRadioButton;
        private DateTimePicker dateFromTimePicker;
        private Label exportQsoDateFromLabel;
        private Label exportQsoDateToLabel;
        private DateTimePicker dateToTimePicker;
        private GroupBox amountsGroupBox;
        private Label totalLabel;
        private Label filteredLabel;
        private Label totalAmountLabel;
        private Label filteredAmountLabel;
        private GroupBox secondaryFiltersGroupBox;
        private ComboBox programIdComboBox;
        private Label programIdLabel;
        private ComboBox modeComboBox;
        private Label modeLabel;
        private Label bandLabel;
        private ComboBox bandComboBox;
        private ComboBox modeGroupComboBox;
        private Label modeGroupLabel;
        private Label OperatorLabel;
        private ComboBox operatorComboBox;
        private Label sourceIpLabel;
        private ComboBox sourceIpComboBox;
        private Button resetSecondaryFiltersButton;
        private Button exportButton;
        private ProgressBar exportProgressBar;
    }
}