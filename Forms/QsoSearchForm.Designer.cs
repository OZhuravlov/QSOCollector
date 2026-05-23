namespace QSOCollector.Forms
{
    partial class QsoSearchForm
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
        /// the contents of this method by the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            searchGroupBox = new GroupBox();
            searchButton = new Button();
            callSearchTextBox = new TextBox();
            callSearchLabel = new Label();
            filtersGroupBox = new GroupBox();
            bandComboBox = new ComboBox();
            bandLabel = new Label();
            modeGroupComboBox = new ComboBox();
            modeGroupLabel = new Label();
            qsoSearchDataGridView = new DataGridView();
            statusLabel = new Label();
            searchGroupBox.SuspendLayout();
            filtersGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)qsoSearchDataGridView).BeginInit();
            SuspendLayout();
            // 
            // searchGroupBox
            // 
            searchGroupBox.Controls.Add(searchButton);
            searchGroupBox.Controls.Add(callSearchTextBox);
            searchGroupBox.Controls.Add(callSearchLabel);
            searchGroupBox.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            searchGroupBox.Location = new Point(12, 12);
            searchGroupBox.Margin = new Padding(3, 2, 3, 2);
            searchGroupBox.Name = "searchGroupBox";
            searchGroupBox.Padding = new Padding(3, 2, 3, 2);
            searchGroupBox.Size = new Size(450, 60);
            searchGroupBox.TabIndex = 0;
            searchGroupBox.TabStop = false;
            searchGroupBox.Text = "Search";
            // 
            // searchButton
            // 
            searchButton.Enabled = false;
            searchButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            searchButton.Location = new Point(350, 25);
            searchButton.Margin = new Padding(3, 2, 3, 2);
            searchButton.Name = "searchButton";
            searchButton.Size = new Size(90, 26);
            searchButton.TabIndex = 2;
            searchButton.Text = "Search";
            searchButton.UseVisualStyleBackColor = true;
            searchButton.Click += searchButton_Click;
            // 
            // callSearchTextBox
            // 
            callSearchTextBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            callSearchTextBox.Location = new Point(120, 25);
            callSearchTextBox.Margin = new Padding(3, 2, 3, 2);
            callSearchTextBox.Name = "callSearchTextBox";
            callSearchTextBox.Size = new Size(220, 23);
            callSearchTextBox.TabIndex = 1;
            callSearchTextBox.KeyPress += callSearchTextBox_KeyPress;
            callSearchTextBox.KeyUp += callSearchTextBox_KeyUp;
            // 
            // callSearchLabel
            // 
            callSearchLabel.AutoSize = true;
            callSearchLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            callSearchLabel.Location = new Point(10, 28);
            callSearchLabel.Name = "callSearchLabel";
            callSearchLabel.Size = new Size(90, 15);
            callSearchLabel.TabIndex = 0;
            callSearchLabel.Text = "Search Callsign:";
            // 
            // filtersGroupBox
            // 
            filtersGroupBox.Controls.Add(bandComboBox);
            filtersGroupBox.Controls.Add(bandLabel);
            filtersGroupBox.Controls.Add(modeGroupComboBox);
            filtersGroupBox.Controls.Add(modeGroupLabel);
            filtersGroupBox.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            filtersGroupBox.Location = new Point(475, 12);
            filtersGroupBox.Margin = new Padding(3, 2, 3, 2);
            filtersGroupBox.Name = "filtersGroupBox";
            filtersGroupBox.Padding = new Padding(3, 2, 3, 2);
            filtersGroupBox.Size = new Size(310, 60);
            filtersGroupBox.TabIndex = 1;
            filtersGroupBox.TabStop = false;
            filtersGroupBox.Text = "Filters";
            // 
            // bandComboBox
            // 
            bandComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            bandComboBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            bandComboBox.FormattingEnabled = true;
            bandComboBox.Location = new Point(200, 25);
            bandComboBox.Margin = new Padding(3, 2, 3, 2);
            bandComboBox.Name = "bandComboBox";
            bandComboBox.Size = new Size(100, 23);
            bandComboBox.TabIndex = 3;
            bandComboBox.SelectedIndexChanged += bandComboBox_SelectedIndexChanged;
            // 
            // bandLabel
            // 
            bandLabel.AutoSize = true;
            bandLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            bandLabel.Location = new Point(168, 28);
            bandLabel.Name = "bandLabel";
            bandLabel.Size = new Size(37, 15);
            bandLabel.TabIndex = 2;
            bandLabel.Text = "Band:";
            // 
            // modeGroupComboBox
            // 
            modeGroupComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            modeGroupComboBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            modeGroupComboBox.FormattingEnabled = true;
            modeGroupComboBox.Location = new Point(60, 25);
            modeGroupComboBox.Margin = new Padding(3, 2, 3, 2);
            modeGroupComboBox.Name = "modeGroupComboBox";
            modeGroupComboBox.Size = new Size(100, 23);
            modeGroupComboBox.TabIndex = 1;
            modeGroupComboBox.SelectedIndexChanged += modeGroupComboBox_SelectedIndexChanged;
            // 
            // modeGroupLabel
            // 
            modeGroupLabel.AutoSize = true;
            modeGroupLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            modeGroupLabel.Location = new Point(10, 28);
            modeGroupLabel.Name = "modeGroupLabel";
            modeGroupLabel.Size = new Size(41, 15);
            modeGroupLabel.TabIndex = 0;
            modeGroupLabel.Text = "Mode:";
            // 
            // qsoSearchDataGridView
            // 
            qsoSearchDataGridView.AllowUserToAddRows = false;
            qsoSearchDataGridView.AllowUserToDeleteRows = false;
            qsoSearchDataGridView.AllowUserToResizeColumns = false;
            qsoSearchDataGridView.AllowUserToResizeRows = false;
            qsoSearchDataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            qsoSearchDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            qsoSearchDataGridView.Location = new Point(12, 85);
            qsoSearchDataGridView.Margin = new Padding(3, 2, 3, 2);
            qsoSearchDataGridView.Name = "qsoSearchDataGridView";
            qsoSearchDataGridView.ReadOnly = true;
            qsoSearchDataGridView.RowHeadersVisible = false;
            qsoSearchDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            qsoSearchDataGridView.Size = new Size(773, 480);
            qsoSearchDataGridView.TabIndex = 2;
            // 
            // statusLabel
            // 
            statusLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            statusLabel.AutoSize = true;
            statusLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            statusLabel.Location = new Point(12, 573);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(0, 15);
            statusLabel.TabIndex = 3;
            // 
            // QsoSearchForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(797, 598);
            Controls.Add(statusLabel);
            Controls.Add(qsoSearchDataGridView);
            Controls.Add(filtersGroupBox);
            Controls.Add(searchGroupBox);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Margin = new Padding(3, 2, 3, 2);
            Name = "QsoSearchForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Search QSOs";
            searchGroupBox.ResumeLayout(false);
            searchGroupBox.PerformLayout();
            filtersGroupBox.ResumeLayout(false);
            filtersGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)qsoSearchDataGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox searchGroupBox;
        private Button searchButton;
        private TextBox callSearchTextBox;
        private Label callSearchLabel;
        private GroupBox filtersGroupBox;
        private ComboBox bandComboBox;
        private Label bandLabel;
        private ComboBox modeGroupComboBox;
        private Label modeGroupLabel;
        private DataGridView qsoSearchDataGridView;
        private Label statusLabel;
    }
}
