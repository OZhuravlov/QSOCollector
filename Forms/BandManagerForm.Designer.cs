using System.Data.SQLite;

namespace QSOCollector.Forms
{
    partial class BandManagerForm
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            bandManagerDataGridView = new DataGridView();
            bandCancelEditButton = new Button();
            bandSaveButton = new Button();
            bandMgrBindingSource = new BindingSource(components);
            BandDeleteButton = new Button();
            id = new DataGridViewTextBoxColumn();
            band = new DataGridViewTextBoxColumn();
            bandName = new DataGridViewTextBoxColumn();
            n1mmName = new DataGridViewTextBoxColumn();
            freqFrom = new DataGridViewTextBoxColumn();
            freqTo = new DataGridViewTextBoxColumn();
            designator = new DataGridViewTextBoxColumn();
            isActive = new DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)bandManagerDataGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bandMgrBindingSource).BeginInit();
            SuspendLayout();
            // 
            // bandManagerDataGridView
            // 
            bandManagerDataGridView.BackgroundColor = SystemColors.Control;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = SystemColors.Window;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = Color.LightGray;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            bandManagerDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            bandManagerDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            bandManagerDataGridView.Columns.AddRange(new DataGridViewColumn[] { id, band, bandName, n1mmName, freqFrom, freqTo, designator, isActive });
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = SystemColors.Window;
            dataGridViewCellStyle4.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle4.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = Color.LightGray;
            dataGridViewCellStyle4.SelectionForeColor = SystemColors.WindowText;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.False;
            bandManagerDataGridView.DefaultCellStyle = dataGridViewCellStyle4;
            bandManagerDataGridView.Location = new Point(5, 5);
            bandManagerDataGridView.Name = "bandManagerDataGridView";
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = SystemColors.Window;
            dataGridViewCellStyle5.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle5.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = Color.LightGray;
            dataGridViewCellStyle5.SelectionForeColor = SystemColors.WindowText;
            dataGridViewCellStyle5.WrapMode = DataGridViewTriState.True;
            bandManagerDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            bandManagerDataGridView.Size = new Size(629, 366);
            bandManagerDataGridView.TabIndex = 0;
            bandManagerDataGridView.CellBeginEdit += BandManagerDataGridView_CellBeginEdit;
            bandManagerDataGridView.CellValidating += BandManagerDataGridView_CellValidating;
            bandManagerDataGridView.EditingControlShowing += BandManagerDataGridView_EditingControlShowing;
            bandManagerDataGridView.RowValidating += BandManagerDataGridView_RowValidating;
            bandManagerDataGridView.SelectionChanged += BandManagerDataGridView_SelectionChanged;
            // 
            // bandCancelEditButton
            // 
            bandCancelEditButton.Location = new Point(267, 386);
            bandCancelEditButton.Name = "bandCancelEditButton";
            bandCancelEditButton.Size = new Size(98, 33);
            bandCancelEditButton.TabIndex = 1;
            bandCancelEditButton.Text = "Cancel";
            bandCancelEditButton.UseVisualStyleBackColor = true;
            bandCancelEditButton.Click += BandCancelEditButton_Click;
            // 
            // bandSaveButton
            // 
            bandSaveButton.Enabled = false;
            bandSaveButton.Location = new Point(481, 386);
            bandSaveButton.Name = "bandSaveButton";
            bandSaveButton.Size = new Size(98, 33);
            bandSaveButton.TabIndex = 2;
            bandSaveButton.Text = "Save";
            bandSaveButton.UseVisualStyleBackColor = true;
            bandSaveButton.Click += BandSaveButton_Click;
            // 
            // BandDeleteButton
            // 
            BandDeleteButton.Location = new Point(36, 381);
            BandDeleteButton.Name = "BandDeleteButton";
            BandDeleteButton.Size = new Size(98, 42);
            BandDeleteButton.TabIndex = 3;
            BandDeleteButton.Text = "Delete selected";
            BandDeleteButton.UseVisualStyleBackColor = true;
            BandDeleteButton.Click += BandDeleteButton_Click;
            // 
            // id
            // 
            id.DataPropertyName = "id";
            id.HeaderText = "Id";
            id.MaxInputLength = 2;
            id.Name = "id";
            id.Visible = false;
            // 
            // band
            // 
            band.DataPropertyName = "name";
            band.HeaderText = "Name";
            band.MaxInputLength = 10;
            band.Name = "band";
            band.Width = 70;
            // 
            // bandName
            // 
            bandName.DataPropertyName = "alt_name";
            bandName.HeaderText = "Alternative Name";
            bandName.MaxInputLength = 10;
            bandName.Name = "bandName";
            bandName.Width = 90;
            // 
            // n1mmName
            // 
            n1mmName.DataPropertyName = "n1mm_name";
            n1mmName.HeaderText = "N1MM name";
            n1mmName.MaxInputLength = 10;
            n1mmName.Name = "n1mmName";
            n1mmName.Width = 75;
            // 
            // freqFrom
            // 
            freqFrom.DataPropertyName = "freq_mhz_from";
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.Format = "N3";
            dataGridViewCellStyle2.FormatProvider = new System.Globalization.CultureInfo("");
            dataGridViewCellStyle2.NullValue = null;
            freqFrom.DefaultCellStyle = dataGridViewCellStyle2;
            freqFrom.HeaderText = "Freq from, mHz";
            freqFrom.MaxInputLength = 10;
            freqFrom.Name = "freqFrom";
            freqFrom.Width = 120;
            // 
            // freqTo
            // 
            freqTo.DataPropertyName = "freq_mhz_to";
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.Format = "N3";
            dataGridViewCellStyle3.FormatProvider = new System.Globalization.CultureInfo("");
            dataGridViewCellStyle3.NullValue = null;
            freqTo.DefaultCellStyle = dataGridViewCellStyle3;
            freqTo.HeaderText = "Freq to, mHz";
            freqTo.MaxInputLength = 10;
            freqTo.Name = "freqTo";
            // 
            // designator
            // 
            designator.DataPropertyName = "designator";
            designator.HeaderText = "Abbr";
            designator.MaxInputLength = 2;
            designator.Name = "designator";
            designator.Width = 50;
            // 
            // isActive
            // 
            isActive.DataPropertyName = "is_active";
            isActive.HeaderText = "Is Active";
            isActive.Name = "isActive";
            isActive.Width = 70;
            // 
            // BandManagerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = bandCancelEditButton;
            ClientSize = new Size(633, 431);
            Controls.Add(BandDeleteButton);
            Controls.Add(bandSaveButton);
            Controls.Add(bandCancelEditButton);
            Controls.Add(bandManagerDataGridView);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "BandManagerForm";
            Text = "Band Manager";
            Load += BandManagerForm_Load;
            ((System.ComponentModel.ISupportInitialize)bandManagerDataGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)bandMgrBindingSource).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView bandManagerDataGridView;
        private Button bandCancelEditButton;
        private Button bandSaveButton;
        private BindingSource bandMgrBindingSource;
        private SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter();
        private Button BandDeleteButton;
        private DataGridViewTextBoxColumn id;
        private DataGridViewTextBoxColumn band;
        private DataGridViewTextBoxColumn bandName;
        private DataGridViewTextBoxColumn n1mmName;
        private DataGridViewTextBoxColumn freqFrom;
        private DataGridViewTextBoxColumn freqTo;
        private DataGridViewTextBoxColumn designator;
        private DataGridViewCheckBoxColumn isActive;
    }
}