using System.Data.SQLite;

namespace QSOCollector.Forms
{
    partial class PremiunCallsignsForm
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
            premiumCallsignsBindingSource = new BindingSource(components);
            premiumCallsignsDataGridView = new DataGridView();
            Id = new DataGridViewTextBoxColumn();
            callsign = new DataGridViewTextBoxColumn();
            club = new DataGridViewTextBoxColumn();
            donated_amount_usd = new DataGridViewTextBoxColumn();
            comment = new DataGridViewTextBoxColumn();
            searchLabel = new Label();
            searchTextBox = new TextBox();
            saveButton = new Button();
            cancelEditButton = new Button();
            deleteSelectedRowsButton = new Button();
            uploadPremiumCallsignsButton = new Button();
            ((System.ComponentModel.ISupportInitialize)premiumCallsignsBindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)premiumCallsignsDataGridView).BeginInit();
            SuspendLayout();
            // 
            // premiumCallsignsDataGridView
            // 
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            premiumCallsignsDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            premiumCallsignsDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            premiumCallsignsDataGridView.Columns.AddRange(new DataGridViewColumn[] { Id, callsign, club, donated_amount_usd, comment });
            premiumCallsignsDataGridView.Location = new Point(12, 47);
            premiumCallsignsDataGridView.Name = "premiumCallsignsDataGridView";
            premiumCallsignsDataGridView.Size = new Size(746, 393);
            premiumCallsignsDataGridView.StandardTab = true;
            premiumCallsignsDataGridView.TabIndex = 0;
            premiumCallsignsDataGridView.CellBeginEdit += premiumCallsignsDataGridView_CellBeginEdit;
            premiumCallsignsDataGridView.CellValidating += premiumCallsignsDataGridView_CellValidating;
            premiumCallsignsDataGridView.EditingControlShowing += premiumCallsignsDataGridView_EditingControlShowing;
            premiumCallsignsDataGridView.RowValidating += premiumCallsignsDataGridView_RowValidating;
            // 
            // Id
            // 
            Id.DataPropertyName = "id";
            Id.HeaderText = "Id";
            Id.Name = "Id";
            Id.Visible = false;
            // 
            // callsign
            // 
            callsign.DataPropertyName = "callsign";
            callsign.HeaderText = "Callsign";
            callsign.MaxInputLength = 20;
            callsign.Name = "callsign";
            callsign.Width = 120;
            // 
            // club
            // 
            club.DataPropertyName = "club";
            club.HeaderText = "Club";
            club.MaxInputLength = 100;
            club.Name = "club";
            club.Width = 200;
            // 
            // donated_amount_usd
            // 
            donated_amount_usd.DataPropertyName = "donated_amount_usd";
            donated_amount_usd.HeaderText = "Donated, USD";
            donated_amount_usd.MaxInputLength = 10;
            donated_amount_usd.Name = "donated_amount_usd";
            // 
            // comment
            // 
            comment.DataPropertyName = "comment";
            comment.HeaderText = "Comments";
            comment.MaxInputLength = 500;
            comment.Name = "comment";
            comment.Width = 250;
            // 
            // searchLabel
            // 
            searchLabel.AutoSize = true;
            searchLabel.Location = new Point(16, 20);
            searchLabel.Name = "searchLabel";
            searchLabel.Size = new Size(42, 15);
            searchLabel.TabIndex = 1;
            searchLabel.Text = "Search";
            // 
            // searchTextBox
            // 
            searchTextBox.Location = new Point(62, 18);
            searchTextBox.Name = "searchTextBox";
            searchTextBox.Size = new Size(214, 23);
            searchTextBox.TabIndex = 2;
            searchTextBox.TextChanged += searchTextBox_TextChanged;
            // 
            // saveButton
            // 
            saveButton.Location = new Point(588, 463);
            saveButton.Margin = new Padding(3, 2, 3, 2);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(71, 31);
            saveButton.TabIndex = 6;
            saveButton.Text = "Save";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += saveButton_Click;
            // 
            // cancelEditButton
            // 
            cancelEditButton.Location = new Point(506, 463);
            cancelEditButton.Margin = new Padding(3, 2, 3, 2);
            cancelEditButton.Name = "cancelEditButton";
            cancelEditButton.Size = new Size(77, 31);
            cancelEditButton.TabIndex = 5;
            cancelEditButton.Text = "Close";
            cancelEditButton.UseVisualStyleBackColor = true;
            cancelEditButton.Click += cancelEditButton_Click;
            // 
            // deleteSelectedRowsButton
            // 
            deleteSelectedRowsButton.Location = new Point(72, 458);
            deleteSelectedRowsButton.Margin = new Padding(3, 2, 3, 2);
            deleteSelectedRowsButton.Name = "deleteSelectedRowsButton";
            deleteSelectedRowsButton.Size = new Size(103, 38);
            deleteSelectedRowsButton.TabIndex = 4;
            deleteSelectedRowsButton.Text = "Delete Selected Rows";
            deleteSelectedRowsButton.UseVisualStyleBackColor = true;
            deleteSelectedRowsButton.Click += deleteSelectedRowsButton_Click;
            // 
            // uploadPremiumCallsignsButton
            // 
            uploadPremiumCallsignsButton.Location = new Point(470, 12);
            uploadPremiumCallsignsButton.Name = "uploadPremiumCallsignsButton";
            uploadPremiumCallsignsButton.Size = new Size(113, 31);
            uploadPremiumCallsignsButton.TabIndex = 7;
            uploadPremiumCallsignsButton.Text = "Upload from file";
            uploadPremiumCallsignsButton.UseVisualStyleBackColor = true;
            uploadPremiumCallsignsButton.Click += uploadPremiumCallsignsButton_Click;
            // 
            // PremiunCallsignsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(770, 507);
            Controls.Add(uploadPremiumCallsignsButton);
            Controls.Add(saveButton);
            Controls.Add(cancelEditButton);
            Controls.Add(deleteSelectedRowsButton);
            Controls.Add(searchTextBox);
            Controls.Add(searchLabel);
            Controls.Add(premiumCallsignsDataGridView);
            Name = "PremiunCallsignsForm";
            Text = "Premium Callsigns";
            Load += PremiunCallsignsForm_Load;
            ((System.ComponentModel.ISupportInitialize)premiumCallsignsBindingSource).EndInit();
            ((System.ComponentModel.ISupportInitialize)premiumCallsignsDataGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private BindingSource premiumCallsignsBindingSource;
        private SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter();
        private DataGridView premiumCallsignsDataGridView;
        private Label searchLabel;
        private TextBox searchTextBox;
        private Button saveButton;
        private Button cancelEditButton;
        private Button deleteSelectedRowsButton;
        private DataGridViewTextBoxColumn Id;
        private DataGridViewTextBoxColumn callsign;
        private DataGridViewTextBoxColumn club;
        private DataGridViewTextBoxColumn donated_amount_usd;
        private DataGridViewTextBoxColumn comment;
        private Button uploadPremiumCallsignsButton;
    }
}