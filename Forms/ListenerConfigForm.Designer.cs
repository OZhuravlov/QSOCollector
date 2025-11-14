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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            dataGridView1 = new DataGridView();
            deleteSelectedListenersButton = new Button();
            cancelEditListenersButton = new Button();
            saveListenersButton = new Button();
            exportConfigButton = new Button();
            importConfigButton = new Button();
            id = new DataGridViewTextBoxColumn();
            name = new DataGridViewTextBoxColumn();
            qso_port = new DataGridViewTextBoxColumn();
            forward_port = new DataGridViewTextBoxColumn();
            acknowledge_port = new DataGridViewTextBoxColumn();
            message_format = new DataGridViewComboBoxColumn();
            is_active = new DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.SelectionBackColor = Color.Gray;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView1.BackgroundColor = SystemColors.Window;
            dataGridView1.BorderStyle = BorderStyle.Fixed3D;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = SystemColors.Control;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = Color.LightGray;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.WindowText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { id, name, qso_port, forward_port, acknowledge_port, message_format, is_active });
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = SystemColors.Window;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = Color.LightGray;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.WindowText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dataGridView1.DefaultCellStyle = dataGridViewCellStyle3;
            dataGridView1.Location = new Point(-1, 2);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 20;
            dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView1.Size = new Size(585, 257);
            dataGridView1.TabIndex = 0;
            dataGridView1.CellBeginEdit += dataGridView1_CellBeginEdit;
            dataGridView1.CellValidating += dataGridView1_CellValidating;
            dataGridView1.DefaultValuesNeeded += dataGridView1_DefaultValuesNeeded;
            dataGridView1.EditingControlShowing += dataGridView1_EditingControlShowing;
            dataGridView1.RowValidating += dataGridView1_RowValidating;
            // 
            // deleteSelectedListenersButton
            // 
            deleteSelectedListenersButton.Location = new Point(7, 266);
            deleteSelectedListenersButton.Name = "deleteSelectedListenersButton";
            deleteSelectedListenersButton.Size = new Size(118, 51);
            deleteSelectedListenersButton.TabIndex = 1;
            deleteSelectedListenersButton.Text = "Delete Selected Rows";
            deleteSelectedListenersButton.UseVisualStyleBackColor = true;
            deleteSelectedListenersButton.Click += deleteSelectedListenersButton_Click;
            // 
            // cancelEditListenersButton
            // 
            cancelEditListenersButton.Location = new Point(402, 272);
            cancelEditListenersButton.Name = "cancelEditListenersButton";
            cancelEditListenersButton.Size = new Size(88, 41);
            cancelEditListenersButton.TabIndex = 2;
            cancelEditListenersButton.Text = "Close";
            cancelEditListenersButton.UseVisualStyleBackColor = true;
            cancelEditListenersButton.Click += cancelEditListenersButton_Click;
            // 
            // saveListenersButton
            // 
            saveListenersButton.Enabled = false;
            saveListenersButton.Location = new Point(496, 272);
            saveListenersButton.Name = "saveListenersButton";
            saveListenersButton.Size = new Size(81, 41);
            saveListenersButton.TabIndex = 3;
            saveListenersButton.Text = "Save";
            saveListenersButton.UseVisualStyleBackColor = true;
            saveListenersButton.Click += saveListenersButton_Click;
            // 
            // exportConfigButton
            // 
            exportConfigButton.Enabled = false;
            exportConfigButton.Location = new Point(268, 268);
            exportConfigButton.Name = "exportConfigButton";
            exportConfigButton.Size = new Size(102, 49);
            exportConfigButton.TabIndex = 4;
            exportConfigButton.Text = "Export config";
            exportConfigButton.UseVisualStyleBackColor = true;
            exportConfigButton.Click += exportConfigButton_Click;
            // 
            // importConfigButton
            // 
            importConfigButton.Location = new Point(159, 267);
            importConfigButton.Name = "importConfigButton";
            importConfigButton.Size = new Size(105, 50);
            importConfigButton.TabIndex = 5;
            importConfigButton.Text = "Import config";
            importConfigButton.UseVisualStyleBackColor = true;
            importConfigButton.Click += importConfigButton_Click;
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
            // name
            // 
            name.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            name.DataPropertyName = "name";
            name.HeaderText = "*Listener name";
            name.MaxInputLength = 100;
            name.MinimumWidth = 6;
            name.Name = "name";
            name.Resizable = DataGridViewTriState.False;
            name.SortMode = DataGridViewColumnSortMode.NotSortable;
            name.ToolTipText = "Describe sender. Like MSHV, N1MM, WSJTX";
            name.Width = 120;
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
            qso_port.ToolTipText = "main Port to get All QSOs";
            qso_port.Width = 90;
            // 
            // forward_port
            // 
            forward_port.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            forward_port.DataPropertyName = "forward_port";
            forward_port.HeaderText = "Forward Port";
            forward_port.MaxInputLength = 6;
            forward_port.MinimumWidth = 6;
            forward_port.Name = "forward_port";
            forward_port.ToolTipText = "(Optional) Port to forward all messages to";
            forward_port.Width = 80;
            // 
            // acknowledge_port
            // 
            acknowledge_port.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            acknowledge_port.DataPropertyName = "acknowledge_port";
            acknowledge_port.HeaderText = "Application port";
            acknowledge_port.MaxInputLength = 6;
            acknowledge_port.MinimumWidth = 6;
            acknowledge_port.Name = "acknowledge_port";
            acknowledge_port.ToolTipText = "Port used by Application to send information of itself. Used to monitor App is active";
            acknowledge_port.Width = 90;
            // 
            // message_format
            // 
            message_format.DataPropertyName = "message_format";
            message_format.HeaderText = "*Format";
            message_format.Items.AddRange(new object[] { "ADIF", "N1MM" });
            message_format.MinimumWidth = 6;
            message_format.Name = "message_format";
            message_format.Resizable = DataGridViewTriState.False;
            message_format.ToolTipText = "QSO message format like ADIF, N1MM";
            message_format.Width = 110;
            // 
            // is_active
            // 
            is_active.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            is_active.DataPropertyName = "is_active";
            is_active.FlatStyle = FlatStyle.Popup;
            is_active.HeaderText = "*Active?";
            is_active.MinimumWidth = 6;
            is_active.Name = "is_active";
            is_active.Resizable = DataGridViewTriState.False;
            is_active.Width = 70;
            // 
            // ListenersForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(585, 322);
            Controls.Add(importConfigButton);
            Controls.Add(exportConfigButton);
            Controls.Add(saveListenersButton);
            Controls.Add(cancelEditListenersButton);
            Controls.Add(deleteSelectedListenersButton);
            Controls.Add(dataGridView1);
            ForeColor = SystemColors.ControlText;
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            Name = "ListenersForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "UDP Listeners";
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
        private Button exportConfigButton;
        private Button importConfigButton;
        private DataGridViewTextBoxColumn id;
        private DataGridViewTextBoxColumn name;
        private DataGridViewTextBoxColumn qso_port;
        private DataGridViewTextBoxColumn forward_port;
        private DataGridViewTextBoxColumn acknowledge_port;
        private DataGridViewComboBoxColumn message_format;
        private DataGridViewCheckBoxColumn is_active;
    }
}