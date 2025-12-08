using Newtonsoft.Json.Linq;
using QSOCollector.Data;
using QSOCollector.Root;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.Text.Json;

namespace QSOCollector
{
    public partial class ListenersForm : Form
    {
        private readonly string connectionString;
        private readonly bool isLocalClientRunning;
        private readonly DbRepository dbRepository;

        public ListenersForm(string connectionString, bool isLocalClientRunning)
        {
            this.connectionString = connectionString;
            this.isLocalClientRunning = isLocalClientRunning;
            dbRepository = new DbRepository(connectionString);
            InitializeComponent();
        }

        private void ListenersForm_Load(object sender, EventArgs e)
        {
            // Bind the DataGridView to the BindingSource
            // and load the data from the database.
            dataGridView1.DataSource = bindingSource1;
            GetListenersConfigDataForDataGridView1(connectionString, "select id, name, qso_port, forward_port, acknowledge_port, message_format, is_active from listeners");
            exportConfigButton.Enabled = dataGridView1.Rows.Count > 0;
        }

        private void GetListenersConfigDataForDataGridView1(string connectionString, string selectCommand)
        {
            try
            {
                // Create a new data adapter based on the specified query.
                dataAdapter = new SQLiteDataAdapter(selectCommand, connectionString);

                // Create a command builder to generate SQL update, insert, and
                // delete commands based on selectCommand.
                SQLiteCommandBuilder commandBuilder = new SQLiteCommandBuilder(dataAdapter);

                // Populate a new data table and bind it to the BindingSource.
                DataTable table = new()
                {
                    Locale = CultureInfo.InvariantCulture
                };
                dataAdapter.Fill(table);
                bindingSource1.DataSource = table;

                // Resize the DataGridView columns to fit the newly loaded content.
                // dataGridView1.AutoResizeColumns(
                //     DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show($"Can't retrieve data from DB: {ex.Message}");
            }
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            DataGridViewColumn column = dataGridView1.Columns[e.ColumnIndex];
            e.Cancel = column.Name.EndsWith("_port") && !handlePortValue(e.RowIndex, e.ColumnIndex, column.HeaderText);
        }

        private bool handlePortValue(int currentRowIndex, int currentColumnIndex, string currentColumnHeader)
        {
            DataGridViewRow currentRow = dataGridView1.Rows[currentRowIndex];
            DataGridViewCell currentCell = currentRow.Cells[currentColumnIndex];
            string? portValue = currentCell.EditedFormattedValue?.ToString();

            // Skip checking empty values
            if (string.IsNullOrEmpty(portValue)) return true;

            if (!int.TryParse(portValue, out int port) || port < 1 || port > 65535)
            {
                currentRow.ErrorText = $"The value {portValue} of '{currentColumnHeader}' must be a number between 1 and 65535";
                return false;
            }

            // Get port columns indexes
            List<DataGridViewColumn> portColumns = [
                dataGridView1.Columns["qso_port"],
                dataGridView1.Columns["acknowledge_port"],
                dataGridView1.Columns["forward_port"]
            ];

            int nameIndex = dataGridView1.Columns["name"].Index;

            // Check for uniqueness
            foreach (DataGridViewRow otherRow in dataGridView1.Rows)
            {
                // Skip empty rows
                if (otherRow.IsNewRow) continue;

                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    DataGridViewCell otherCell = otherRow.Cells[column.Index];
                    // Skip checking the current cell
                    if (otherCell == currentCell)
                        continue;
                    // Skipp non-port columns
                    if (!portColumns.Contains(column))
                        continue;
                    string? otherPort = otherCell.FormattedValue?.ToString();
                    // Skip checking other cell if empty
                    if (string.IsNullOrEmpty(otherPort))
                    {
                        continue;
                    }

                    if (otherPort == portValue)
                    {
                        dataGridView1.BeginEdit(true);
                        string errorMessage = $"Port must be unique accross all ports in config but {portValue} conflicts with '{dataGridView1.Columns[otherCell.ColumnIndex].HeaderText}' of '{otherRow.Cells[nameIndex].FormattedValue}' Listener";
                        MessageBox.Show(errorMessage, "Port Uniqueness Violation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        currentCell.ErrorText = errorMessage;
                        currentRow.ErrorText = errorMessage;
                        dataGridView1.EndEdit();
                        return false;
                    }
                }
            }
            dataGridView1.BeginEdit(true);
            currentCell.ErrorText = string.Empty;
            currentRow.ErrorText = string.Empty;
            dataGridView1.EndEdit();
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
                if (column.Name == "id" || column.Name == "forward_port" || column.Name == "acknowledge_port")
                    continue;
                DataGridViewCell cell = row.Cells[column.Index];
                if (cell.FormattedValue == null || string.IsNullOrWhiteSpace(cell.FormattedValue.ToString()))
                {
                    dataGridView1.BeginEdit(true);
                    string errorMessage = $"The value of '{column.HeaderText}' must not be empty";
                    cell.ErrorText = errorMessage;
                    row.ErrorText = errorMessage;
                    exportConfigButton.Enabled = false;
                    dataGridView1.EndEdit();
                    data.Cancel = true;
                    return;
                }
                row.ErrorText = string.Empty;
            }
            exportConfigButton.Enabled = true;
        }

        private void dataGridView1_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["is_active"].Value = true;
            e.Row.Cells["message_format"].Value = "N1MM";
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
            try
            {
                dataAdapter.Update((DataTable)bindingSource1.DataSource);
            }
            catch (DBConcurrencyException)
            {
            }

            cancelEditListenersButton.Text = "Close";
            cancelEditListenersButton.Focus();
            saveListenersButton.Enabled = false;
            if (isLocalClientRunning)
            {
                MessageBox.Show("New config will be applied only after restarting Client", "Restart required", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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
            exportConfigButton.Enabled = dataGridView1.Rows.Count > 0;
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress += new KeyPressEventHandler(dataGridView1Port_KeyPress);
        }

        private void dataGridView1Port_KeyPress(object sender, KeyPressEventArgs e)
        {
            int currentColumnIndex = dataGridView1.CurrentCell.ColumnIndex;
            int qsoPortColumnIndex = dataGridView1.Columns["qso_port"].Index;
            int acknowledgePortColumnIndex = dataGridView1.Columns["acknowledge_port"].Index;
            int forwardPortColumnIndex = dataGridView1.Columns["forward_port"].Index;

            if (currentColumnIndex != qsoPortColumnIndex && currentColumnIndex != acknowledgePortColumnIndex && currentColumnIndex != forwardPortColumnIndex)
            {
                return;
            }

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void exportConfigButton_Click(object sender, EventArgs e)
        {
            if (saveListenersButton.Enabled)
            {
                DialogResult result = MessageBox.Show("All changes must be saved first.\nDo you want to save them now and continue?", "Saving changes before export", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                {
                    return;
                }
                saveListenersButton_Click(saveListenersButton, EventArgs.Empty);
            }

            using SaveFileDialog saveFileDialog = new()
            {
                InitialDirectory = Program.configFolder,
                FileName = "listeners-config.json",
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                DefaultExt = "json",
                AddExtension = true,
                RestoreDirectory = true
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                List<Models.ListenerConfig> listenerConfigs = dbRepository.GetListenerConfigs();
                string jsonListenerConfigs = JToken.Parse(
                    JsonSerializer.Serialize<List<Models.ListenerConfig>>(listenerConfigs)
                    ).ToString();
                string filePath = saveFileDialog.FileName;
                File.WriteAllText(filePath, jsonListenerConfigs);
            }
            else
            {
                return;
            }
        }

        private void importConfigButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0 || dbRepository.GetListenerConfigs().Count > 0)
            {
                DialogResult result = MessageBox.Show("Existing configs will be replaced by imported. Do you want to continue?", "Existing config replacement", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                {
                    return;
                }
                using OpenFileDialog openFileDialog = new();
                openFileDialog.InitialDirectory = Program.configFolder;
                openFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                var fileStream = openFileDialog.OpenFile();
                using StreamReader reader = new(fileStream);
                string jsonListenerConfigs = reader.ReadToEnd();
                List<Models.ListenerConfig> listenerConfigs = JsonSerializer.Deserialize<List<Models.ListenerConfig>>(jsonListenerConfigs);
                dbRepository.ReplaceListenerConfigs(listenerConfigs);
                ListenersForm_Load(this, EventArgs.Empty);
            }
        }
    }
}
