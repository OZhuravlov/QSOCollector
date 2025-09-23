using System.Data;
using System.Data.SQLite;
using System.Globalization;

namespace QSOCollector
{
    public partial class ListenersForm : Form
    {
        private string connectionString;

        public ListenersForm(string connectionString)
        {
            this.connectionString = connectionString;
            InitializeComponent();
        }

        private void ListenersForm_Load(object sender, EventArgs e)
        {
            // Bind the DataGridView to the BindingSource
            // and load the data from the database.
            dataGridView1.DataSource = bindingSource1;
            GetData(connectionString, "select id, protocol, qso_port, acknowledge_port, message_format, is_active, description from listeners");
        }

        private void GetData(string connectionString, string selectCommand)
        {
            try
            {
                // Create a new data adapter based on the specified query.
                dataAdapter = new SQLiteDataAdapter(selectCommand, connectionString);

                // Create a command builder to generate SQL update, insert, and
                // delete commands based on selectCommand.
                SQLiteCommandBuilder commandBuilder = new SQLiteCommandBuilder(dataAdapter);

                // Populate a new data table and bind it to the BindingSource.
                DataTable table = new DataTable
                {
                    Locale = CultureInfo.InvariantCulture
                };
                dataAdapter.Fill(table);
                bindingSource1.DataSource = table;

                // Resize the DataGridView columns to fit the newly loaded content.
                // dataGridView1.AutoResizeColumns(
                //     DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
            }
            catch (SQLiteException)
            {
                MessageBox.Show("To run this example, replace the value of the " +
                    "connectionString variable with a connection string that is " +
                    "valid for your system.");
            }
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            DataGridViewColumn column = dataGridView1.Columns[e.ColumnIndex];
            e.Cancel = (column.Name == "qso_port" || column.Name == "acknowledge_port")
                && !handlePortValue(e.RowIndex, e.ColumnIndex, column.HeaderText);
        }

        private bool handlePortValue(int rowIndex, int columnIndex, string columnHeader)
        {
            DataGridViewRow row = dataGridView1.Rows[rowIndex];
            DataGridViewCell cell = row.Cells[columnIndex];
            if (!string.IsNullOrEmpty((string?)cell.EditedFormattedValue)
                && (!int.TryParse(cell.EditedFormattedValue.ToString(), out int port) || port < 1 || port > 65535))
            {
                row.ErrorText = $"The value of '{columnHeader}' must be a number between 1 and 65535";
                return false;
            }
            // Check for uniqueness
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (i == rowIndex) continue;
                DataGridViewRow otherRow = dataGridView1.Rows[i];
                if (otherRow.IsNewRow) continue;
                if (!string.IsNullOrEmpty((string?)otherRow.Cells[columnIndex].EditedFormattedValue)
                    && !string.IsNullOrEmpty((string?)cell.EditedFormattedValue)
                    && otherRow.Cells[columnIndex].EditedFormattedValue.ToString() == cell.EditedFormattedValue.ToString())
                {
                    row.ErrorText = $"The value of '{columnHeader}' must be unique";
                    return false;
                }
            }

            row.ErrorText = string.Empty;
            return true;
        }

        private void dataGridView1_RowValidating(object sender, DataGridViewCellCancelEventArgs data)
        {
            DataGridViewRow row = dataGridView1.Rows[data.RowIndex];
            // skip checking new row
            if (row.IsNewRow) return;
            // Validate all columns except id and description
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                if (column.Name == "id" || column.Name == "description" || column.Name == "acknowledge_port")
                    continue;
                DataGridViewCell cell = row.Cells[column.Index];
                if (cell.FormattedValue == null || string.IsNullOrWhiteSpace(cell.FormattedValue.ToString()))
                {
                    row.ErrorText = $"The value of '{column.HeaderText}' must not be empty";
                    return;
                }
                row.ErrorText = string.Empty;
            }
        }

        private void dataGridView1_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["is_active"].Value = true;
            e.Row.Cells["protocol"].Value = "UDP";
            e.Row.Cells["message_format"].Value = "ADIF";
        }

        private void cancelEditListenersButton_Click(object sender, EventArgs e)
        {
            if (saveListenersButton.Enabled)
            {
                DialogResult result = MessageBox.Show("All unsaved changes will be lost. Do you want to continue?", "Cancel changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes)
                {
                    return;
                }
            }
            this.Close();
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            cancelEditListenersButton.Text = "Cancel";
            saveListenersButton.Enabled = true;
        }

        private void saveListenersButton_Click(object sender, EventArgs e)
        {
            // Validate rows before saving
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;
                if (!string.IsNullOrEmpty(row.ErrorText))
                {
                    MessageBox.Show("Please fix the errors before saving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Ensure the current edit is committed.
            dataGridView1.EndEdit();
            // Save the data from the DataGridView to the database.
            dataAdapter.Update((DataTable)bindingSource1.DataSource);
            cancelEditListenersButton.Text = "Close";
            cancelEditListenersButton.Focus();
            saveListenersButton.Enabled = false;
        }

        private void deleteSelectedListenersButton_Click(object sender, EventArgs e)
        {
            List<DataGridViewRow> rowsToDelete = dataGridView1.SelectedRows
                .OfType<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .ToList();

            if (rowsToDelete.Count == 0)
            {
                MessageBox.Show("No rows selected to delete.", "Delete Listeners", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            rowsToDelete.ForEach(r => dataGridView1.Rows.Remove(r));
            cancelEditListenersButton.Text = "Cancel";
            saveListenersButton.Enabled = true;
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int currentColumnIndex = dataGridView1.CurrentCell.ColumnIndex;
            int qsoPortColumnIndex = dataGridView1.Columns["qso_port"].Index;
            int acknowledgePortColumnIndex = dataGridView1.Columns["acknowledge_port"].Index;
            if (currentColumnIndex == qsoPortColumnIndex || currentColumnIndex == acknowledgePortColumnIndex)
            {
                e.Control.KeyPress += new KeyPressEventHandler(dataGridView1Port_KeyPress);
            }
        }

        private void dataGridView1Port_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
