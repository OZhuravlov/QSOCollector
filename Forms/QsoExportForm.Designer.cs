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
            sourceNameComboBox = new ComboBox();
            sourceNameLabel = new Label();
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
            mainFilterGroupBox.Location = new Point(4, 7);
            mainFilterGroupBox.Margin = new Padding(3, 2, 3, 2);
            mainFilterGroupBox.Name = "mainFilterGroupBox";
            mainFilterGroupBox.Padding = new Padding(3, 2, 3, 2);
            mainFilterGroupBox.Size = new Size(349, 86);
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
            dateToTimePicker.Location = new Point(191, 47);
            dateToTimePicker.Margin = new Padding(3, 2, 3, 2);
            dateToTimePicker.Name = "dateToTimePicker";
            dateToTimePicker.Size = new Size(121, 23);
            dateToTimePicker.TabIndex = 4;
            dateToTimePicker.ValueChanged += dateToTimePicker_ValueChanged;
            dateToTimePicker.EnabledChanged += dateToTimePicker_EnabledChanged;
            // 
            // exportQsoDateToLabel
            // 
            exportQsoDateToLabel.AutoSize = true;
            exportQsoDateToLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            exportQsoDateToLabel.Location = new Point(115, 49);
            exportQsoDateToLabel.Name = "exportQsoDateToLabel";
            exportQsoDateToLabel.Size = new Size(46, 15);
            exportQsoDateToLabel.TabIndex = 3;
            exportQsoDateToLabel.Text = "Date To";
            // 
            // exportQsoDateFromLabel
            // 
            exportQsoDateFromLabel.AutoSize = true;
            exportQsoDateFromLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            exportQsoDateFromLabel.Location = new Point(115, 26);
            exportQsoDateFromLabel.Name = "exportQsoDateFromLabel";
            exportQsoDateFromLabel.Size = new Size(62, 15);
            exportQsoDateFromLabel.TabIndex = 2;
            exportQsoDateFromLabel.Text = "Date From";
            // 
            // allQSOsRadioButton
            // 
            allQSOsRadioButton.AutoSize = true;
            allQSOsRadioButton.Font = new Font("Segoe UI", 9F);
            allQSOsRadioButton.Location = new Point(5, 63);
            allQSOsRadioButton.Margin = new Padding(3, 2, 3, 2);
            allQSOsRadioButton.Name = "allQSOsRadioButton";
            allQSOsRadioButton.Size = new Size(71, 19);
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
            dateFromTimePicker.Location = new Point(191, 24);
            dateFromTimePicker.Margin = new Padding(3, 2, 3, 2);
            dateFromTimePicker.Name = "dateFromTimePicker";
            dateFromTimePicker.Size = new Size(121, 23);
            dateFromTimePicker.TabIndex = 1;
            dateFromTimePicker.ValueChanged += dateFromTimePicker_ValueChanged;
            dateFromTimePicker.EnabledChanged += dateFromTimePicker_EnabledChanged;
            // 
            // byDateRadioButton
            // 
            byDateRadioButton.AutoSize = true;
            byDateRadioButton.Font = new Font("Segoe UI", 9F);
            byDateRadioButton.Location = new Point(6, 41);
            byDateRadioButton.Margin = new Padding(3, 2, 3, 2);
            byDateRadioButton.Name = "byDateRadioButton";
            byDateRadioButton.Size = new Size(64, 19);
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
            newQSOsRadioButton.Location = new Point(5, 19);
            newQSOsRadioButton.Margin = new Padding(3, 2, 3, 2);
            newQSOsRadioButton.Name = "newQSOsRadioButton";
            newQSOsRadioButton.Size = new Size(81, 19);
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
            amountsGroupBox.Location = new Point(8, 249);
            amountsGroupBox.Margin = new Padding(3, 2, 3, 2);
            amountsGroupBox.Name = "amountsGroupBox";
            amountsGroupBox.Padding = new Padding(3, 2, 3, 2);
            amountsGroupBox.Size = new Size(349, 40);
            amountsGroupBox.TabIndex = 1;
            amountsGroupBox.TabStop = false;
            amountsGroupBox.Text = "QSO Amount";
            // 
            // filteredAmountLabel
            // 
            filteredAmountLabel.AutoSize = true;
            filteredAmountLabel.Location = new Point(79, 17);
            filteredAmountLabel.Name = "filteredAmountLabel";
            filteredAmountLabel.Size = new Size(13, 15);
            filteredAmountLabel.TabIndex = 3;
            filteredAmountLabel.Text = "0";
            // 
            // totalLabel
            // 
            totalLabel.AutoSize = true;
            totalLabel.Location = new Point(182, 17);
            totalLabel.Name = "totalLabel";
            totalLabel.RightToLeft = RightToLeft.No;
            totalLabel.Size = new Size(46, 15);
            totalLabel.TabIndex = 2;
            totalLabel.Text = "of Total";
            // 
            // filteredLabel
            // 
            filteredLabel.AutoSize = true;
            filteredLabel.Location = new Point(5, 17);
            filteredLabel.Name = "filteredLabel";
            filteredLabel.Size = new Size(46, 15);
            filteredLabel.TabIndex = 1;
            filteredLabel.Text = "Filtered";
            // 
            // totalAmountLabel
            // 
            totalAmountLabel.AutoSize = true;
            totalAmountLabel.Location = new Point(252, 17);
            totalAmountLabel.Name = "totalAmountLabel";
            totalAmountLabel.Size = new Size(13, 15);
            totalAmountLabel.TabIndex = 0;
            totalAmountLabel.Text = "0";
            // 
            // secondaryFiltersGroupBox
            // 
            secondaryFiltersGroupBox.Controls.Add(sourceIpComboBox);
            secondaryFiltersGroupBox.Controls.Add(sourceIpLabel);
            secondaryFiltersGroupBox.Controls.Add(sourceNameComboBox);
            secondaryFiltersGroupBox.Controls.Add(sourceNameLabel);
            secondaryFiltersGroupBox.Controls.Add(operatorComboBox);
            secondaryFiltersGroupBox.Controls.Add(OperatorLabel);
            secondaryFiltersGroupBox.Controls.Add(modeGroupComboBox);
            secondaryFiltersGroupBox.Controls.Add(modeGroupLabel);
            secondaryFiltersGroupBox.Controls.Add(bandComboBox);
            secondaryFiltersGroupBox.Controls.Add(bandLabel);
            secondaryFiltersGroupBox.Controls.Add(modeComboBox);
            secondaryFiltersGroupBox.Controls.Add(modeLabel);
            secondaryFiltersGroupBox.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            secondaryFiltersGroupBox.Location = new Point(4, 97);
            secondaryFiltersGroupBox.Margin = new Padding(3, 2, 3, 2);
            secondaryFiltersGroupBox.Name = "secondaryFiltersGroupBox";
            secondaryFiltersGroupBox.Padding = new Padding(3, 2, 3, 2);
            secondaryFiltersGroupBox.Size = new Size(349, 92);
            secondaryFiltersGroupBox.TabIndex = 2;
            secondaryFiltersGroupBox.TabStop = false;
            secondaryFiltersGroupBox.Text = "Secondary filters";
            // 
            // sourceIpComboBox
            // 
            sourceIpComboBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            sourceIpComboBox.FormattingEnabled = true;
            sourceIpComboBox.Location = new Point(248, 64);
            sourceIpComboBox.Margin = new Padding(3, 2, 3, 2);
            sourceIpComboBox.Name = "sourceIpComboBox";
            sourceIpComboBox.Size = new Size(92, 23);
            sourceIpComboBox.TabIndex = 10;
            sourceIpComboBox.SelectedValueChanged += sourceIpComboBox_SelectedValueChanged;
            // 
            // sourceIpLabel
            // 
            sourceIpLabel.AutoSize = true;
            sourceIpLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            sourceIpLabel.Location = new Point(182, 68);
            sourceIpLabel.Name = "sourceIpLabel";
            sourceIpLabel.Size = new Size(56, 15);
            sourceIpLabel.TabIndex = 9;
            sourceIpLabel.Text = "Source IP";
            // 
            // sourceNameComboBox
            // 
            sourceNameComboBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            sourceNameComboBox.FormattingEnabled = true;
            sourceNameComboBox.ItemHeight = 15;
            sourceNameComboBox.Location = new Point(248, 19);
            sourceNameComboBox.Margin = new Padding(3, 2, 3, 2);
            sourceNameComboBox.Name = "sourceNameComboBox";
            sourceNameComboBox.Size = new Size(92, 23);
            sourceNameComboBox.TabIndex = 3;
            sourceNameComboBox.SelectedValueChanged += sourceNameComboBox_SelectedValueChanged;
            // 
            // sourceNameLabel
            // 
            sourceNameLabel.AutoSize = true;
            sourceNameLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            sourceNameLabel.Location = new Point(186, 21);
            sourceNameLabel.Name = "sourceNameLabel";
            sourceNameLabel.Size = new Size(43, 15);
            sourceNameLabel.TabIndex = 1;
            sourceNameLabel.Text = "Source";
            // 
            // operatorComboBox
            // 
            operatorComboBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            operatorComboBox.FormattingEnabled = true;
            operatorComboBox.Location = new Point(248, 41);
            operatorComboBox.Margin = new Padding(3, 2, 3, 2);
            operatorComboBox.Name = "operatorComboBox";
            operatorComboBox.Size = new Size(92, 23);
            operatorComboBox.TabIndex = 8;
            operatorComboBox.SelectedValueChanged += operatorComboBox_SelectedValueChanged;
            // 
            // OperatorLabel
            // 
            OperatorLabel.AutoSize = true;
            OperatorLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            OperatorLabel.Location = new Point(183, 44);
            OperatorLabel.Name = "OperatorLabel";
            OperatorLabel.Size = new Size(54, 15);
            OperatorLabel.TabIndex = 7;
            OperatorLabel.Text = "Operator";
            // 
            // modeGroupComboBox
            // 
            modeGroupComboBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            modeGroupComboBox.FormattingEnabled = true;
            modeGroupComboBox.Location = new Point(84, 18);
            modeGroupComboBox.Margin = new Padding(3, 2, 3, 2);
            modeGroupComboBox.Name = "modeGroupComboBox";
            modeGroupComboBox.Size = new Size(93, 23);
            modeGroupComboBox.TabIndex = 0;
            modeGroupComboBox.SelectedValueChanged += modeGroupComboBox_SelectedValueChanged;
            // 
            // modeGroupLabel
            // 
            modeGroupLabel.AutoSize = true;
            modeGroupLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            modeGroupLabel.Location = new Point(3, 21);
            modeGroupLabel.Name = "modeGroupLabel";
            modeGroupLabel.Size = new Size(73, 15);
            modeGroupLabel.TabIndex = 6;
            modeGroupLabel.Text = "Mode group";
            // 
            // bandComboBox
            // 
            bandComboBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            bandComboBox.FormattingEnabled = true;
            bandComboBox.Location = new Point(84, 63);
            bandComboBox.Margin = new Padding(3, 2, 3, 2);
            bandComboBox.Name = "bandComboBox";
            bandComboBox.Size = new Size(92, 23);
            bandComboBox.TabIndex = 2;
            bandComboBox.SelectedValueChanged += bandComboBox_SelectedValueChanged;
            // 
            // bandLabel
            // 
            bandLabel.AutoSize = true;
            bandLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            bandLabel.Location = new Point(4, 65);
            bandLabel.Name = "bandLabel";
            bandLabel.Size = new Size(34, 15);
            bandLabel.TabIndex = 4;
            bandLabel.Text = "Band";
            // 
            // modeComboBox
            // 
            modeComboBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            modeComboBox.FormattingEnabled = true;
            modeComboBox.Location = new Point(84, 40);
            modeComboBox.Margin = new Padding(3, 2, 3, 2);
            modeComboBox.Name = "modeComboBox";
            modeComboBox.Size = new Size(93, 23);
            modeComboBox.TabIndex = 1;
            modeComboBox.SelectedValueChanged += modeComboBox_SelectedValueChanged;
            // 
            // modeLabel
            // 
            modeLabel.AutoSize = true;
            modeLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            modeLabel.Location = new Point(4, 44);
            modeLabel.Name = "modeLabel";
            modeLabel.Size = new Size(38, 15);
            modeLabel.TabIndex = 2;
            modeLabel.Text = "Mode";
            // 
            // resetSecondaryFiltersButton
            // 
            resetSecondaryFiltersButton.BackColor = Color.RosyBrown;
            resetSecondaryFiltersButton.FlatStyle = FlatStyle.Popup;
            resetSecondaryFiltersButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            resetSecondaryFiltersButton.Location = new Point(10, 194);
            resetSecondaryFiltersButton.Margin = new Padding(3, 2, 3, 2);
            resetSecondaryFiltersButton.Name = "resetSecondaryFiltersButton";
            resetSecondaryFiltersButton.Size = new Size(162, 28);
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
            exportButton.Location = new Point(186, 194);
            exportButton.Margin = new Padding(3, 2, 3, 2);
            exportButton.Name = "exportButton";
            exportButton.Size = new Size(158, 28);
            exportButton.TabIndex = 12;
            exportButton.Text = "Export";
            exportButton.UseVisualStyleBackColor = false;
            exportButton.Click += exportButton_Click;
            // 
            // exportProgressBar
            // 
            exportProgressBar.Location = new Point(22, 230);
            exportProgressBar.Margin = new Padding(3, 2, 3, 2);
            exportProgressBar.Name = "exportProgressBar";
            exportProgressBar.Size = new Size(305, 14);
            exportProgressBar.TabIndex = 13;
            exportProgressBar.Visible = false;
            // 
            // QsoExportForm
            // 
            AccessibleRole = AccessibleRole.None;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(358, 290);
            Controls.Add(exportProgressBar);
            Controls.Add(exportButton);
            Controls.Add(resetSecondaryFiltersButton);
            Controls.Add(secondaryFiltersGroupBox);
            Controls.Add(amountsGroupBox);
            Controls.Add(mainFilterGroupBox);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Margin = new Padding(3, 2, 3, 2);
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
        private ComboBox sourceNameComboBox;
        private Label sourceNameLabel;
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