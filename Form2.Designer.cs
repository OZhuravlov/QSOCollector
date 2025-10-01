using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Windows.Forms;
using System.Data.SQLite;

namespace QSOCollector
{
    partial class ListenersForm
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
            dataGridView1 = new DataGridView();
            id = new DataGridViewTextBoxColumn();
            protocol = new DataGridViewComboBoxColumn();
            qso_port = new DataGridViewTextBoxColumn();
            acknowledge_port = new DataGridViewTextBoxColumn();
            message_format = new DataGridViewComboBoxColumn();
            is_active = new DataGridViewCheckBoxColumn();
            description = new DataGridViewTextBoxColumn();
            deleteSelectedListenersButton = new Button();
            cancelEditListenersButton = new Button();
            saveListenersButton = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { id, protocol, qso_port, acknowledge_port, message_format, is_active, description });
            dataGridView1.Location = new Point(-1, 2);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView1.Size = new Size(805, 236);
            dataGridView1.TabIndex = 0;
            dataGridView1.CellBeginEdit += dataGridView1_CellBeginEdit;
            dataGridView1.CellValidating += dataGridView1_CellValidating;
            dataGridView1.DefaultValuesNeeded += dataGridView1_DefaultValuesNeeded;
            dataGridView1.EditingControlShowing += dataGridView1_EditingControlShowing;
            dataGridView1.RowValidating += dataGridView1_RowValidating;
            // 
            // id
            // 
            id.DataPropertyName = "id";
            id.HeaderText = "NN";
            id.MinimumWidth = 6;
            id.Name = "id";
            id.ReadOnly = true;
            id.Visible = false;
            id.Width = 10;
            // 
            // protocol
            // 
            protocol.DataPropertyName = "protocol";
            protocol.HeaderText = "*Protocol";
            protocol.Items.AddRange(new object[] { "UDP" });
            protocol.MinimumWidth = 6;
            protocol.Name = "protocol";
            protocol.Resizable = DataGridViewTriState.False;
            protocol.ToolTipText = "Check from QSO producer configuration";
            protocol.Width = 125;
            // 
            // qso_port
            // 
            qso_port.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            qso_port.DataPropertyName = "qso_port";
            qso_port.HeaderText = "*QSO Port";
            qso_port.MaxInputLength = 6;
            qso_port.MinimumWidth = 6;
            qso_port.Name = "qso_port";
            qso_port.Resizable = DataGridViewTriState.False;
            qso_port.SortMode = DataGridViewColumnSortMode.NotSortable;
            qso_port.Width = 70;
            // 
            // acknowledge_port
            // 
            acknowledge_port.DataPropertyName = "acknowledge_port";
            acknowledge_port.HeaderText = "Acknowledge Port (Optional)";
            acknowledge_port.MinimumWidth = 6;
            acknowledge_port.Name = "acknowledge_port";
            acknowledge_port.Width = 125;
            // 
            // message_format
            // 
            message_format.DataPropertyName = "message_format";
            message_format.HeaderText = "*Format";
            message_format.Items.AddRange(new object[] { "ADIF", "N1MM" });
            message_format.MinimumWidth = 6;
            message_format.Name = "message_format";
            message_format.Resizable = DataGridViewTriState.False;
            message_format.ToolTipText = "QSO message format. Check from QSO producer doc";
            message_format.Width = 150;
            // 
            // is_active
            // 
            is_active.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            is_active.DataPropertyName = "is_active";
            is_active.FlatStyle = FlatStyle.Popup;
            is_active.HeaderText = "Active?";
            is_active.MinimumWidth = 6;
            is_active.Name = "is_active";
            is_active.Resizable = DataGridViewTriState.False;
            is_active.Width = 60;
            // 
            // description
            // 
            description.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            description.DataPropertyName = "description";
            description.HeaderText = "Description (Optional)";
            description.MaxInputLength = 100;
            description.MinimumWidth = 6;
            description.Name = "description";
            description.Resizable = DataGridViewTriState.False;
            description.SortMode = DataGridViewColumnSortMode.NotSortable;
            description.Width = 300;
            // 
            // deleteSelectedListenersButton
            // 
            deleteSelectedListenersButton.Location = new Point(66, 244);
            deleteSelectedListenersButton.Name = "deleteSelectedListenersButton";
            deleteSelectedListenersButton.Size = new Size(180, 41);
            deleteSelectedListenersButton.TabIndex = 1;
            deleteSelectedListenersButton.Text = "Delete Selected Rows";
            deleteSelectedListenersButton.UseVisualStyleBackColor = true;
            deleteSelectedListenersButton.Click += deleteSelectedListenersButton_Click;
            // 
            // cancelEditListenersButton
            // 
            cancelEditListenersButton.Location = new Point(389, 244);
            cancelEditListenersButton.Name = "cancelEditListenersButton";
            cancelEditListenersButton.Size = new Size(118, 41);
            cancelEditListenersButton.TabIndex = 2;
            cancelEditListenersButton.Text = "Close";
            cancelEditListenersButton.UseVisualStyleBackColor = true;
            cancelEditListenersButton.Click += cancelEditListenersButton_Click;
            // 
            // saveListenersButton
            // 
            saveListenersButton.Enabled = false;
            saveListenersButton.Location = new Point(513, 244);
            saveListenersButton.Name = "saveListenersButton";
            saveListenersButton.Size = new Size(94, 41);
            saveListenersButton.TabIndex = 3;
            saveListenersButton.Text = "Save";
            saveListenersButton.UseVisualStyleBackColor = true;
            saveListenersButton.Click += saveListenersButton_Click;
            // 
            // ListenersForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(804, 284);
            Controls.Add(saveListenersButton);
            Controls.Add(cancelEditListenersButton);
            Controls.Add(deleteSelectedListenersButton);
            Controls.Add(dataGridView1);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            Name = "ListenersForm";
            Text = "Listeners";
            Load += ListenersForm_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private BindingSource bindingSource1 = new BindingSource();
        private SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter();
        private DataGridView dataGridView1;
        private Button deleteSelectedListenersButton;
        private Button cancelEditListenersButton;
        private Button saveListenersButton;
        private DataGridViewTextBoxColumn id;
        private DataGridViewComboBoxColumn protocol;
        private DataGridViewTextBoxColumn qso_port;
        private DataGridViewTextBoxColumn acknowledge_port;
        private DataGridViewComboBoxColumn message_format;
        private DataGridViewCheckBoxColumn is_active;
        private DataGridViewTextBoxColumn description;
    }
}