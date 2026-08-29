using Microsoft.Data.Sqlite;
using Newtonsoft.Json.Linq;
using QSOCollector.Data;
using QSOCollector.Forms;
using QSOCollector.Root;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.Text.Json;

namespace QSOCollector
{
    public partial class ListenersForm : Form
    {
        public Dictionary<int, bool> listnerRuleChanged = [];

        private readonly bool isLocalClientRunning;
        private readonly IDbRepository dbRepository;
        private readonly List<int> deletedListenereIds = [];

        public ListenersForm(IDbRepository dbRepository, bool isLocalClientRunning)
        {
            this.isLocalClientRunning = isLocalClientRunning;
            this.dbRepository = dbRepository;
            InitializeComponent();
        }

        private void ListenersForm_Load(object sender, EventArgs e)
        {
            // Bind the DataGridView to the BindingSource
            // and load the data from the database.
            ListenerDataGridView.DataSource = bindingSource1;
            Init();
            exportConfigButton.Enabled = ListenerDataGridView.Rows.Count > 0;
        }

        private void Init()
        {
            PopulateListenersConfigDataGridView();
            InitListnerRuleChanged();
            RefreshRuleButtons();
        }

        private void RefreshRuleButtons()
        {
            int rulesColumnIdx = ListenerDataGridView.Columns["rules"].Index;
            int idColumnIdx = ListenerDataGridView.Columns["id"].Index;
            foreach (DataGridViewRow row in ListenerDataGridView.Rows)
            {
                if (row.IsNewRow) continue;
                var id = int.Parse(row.Cells[idColumnIdx].Value.ToString());
                string? rules = dbRepository.GetListenerConcatRuleNames(id);
                DataGridViewButtonCell cell = (DataGridViewButtonCell)row.Cells[rulesColumnIdx];
                cell.UseColumnTextForButtonValue = false;
                cell.FlatStyle = FlatStyle.Popup;

                Font font;
                Color backColor;
                if (String.IsNullOrEmpty(rules))
                {
                    cell.ToolTipText = "No rules defined";
                    cell.Value = "Add Rules";
                    font = new Font(ListenerDataGridView.Font, FontStyle.Regular);
                    backColor = Color.White;
                }
                else
                {
                    cell.ToolTipText = rules;
                    cell.Value = "Edit Rules";
                    cell.FlatStyle = FlatStyle.Popup;
                    font = new Font(ListenerDataGridView.Font, FontStyle.Bold);
                    backColor = Color.DarkSeaGreen;
                }

                cell.Style.ApplyStyle(new()
                {
                    Font = font,
                    BackColor = backColor,
                });
            }
        }

        private void PopulateListenersConfigDataGridView()
        {
            string connectionString = dbRepository.GetConnectionString();
            string selectCommand = "select id, name, qso_port, forward_port, acknowledge_port, message_format, is_active from listeners";
            try
            {
                // Create a new data adapter based on the specified query.
                dataAdapter = new SQLiteDataAdapter(selectCommand, connectionString);

                // Create a command builder to generate SQL update, insert, and
                // delete commands based on selectCommand.
                SQLiteCommandBuilder commandBuilder = new(dataAdapter);

                // Populate a new data table and bind it to the BindingSource.
                DataTable table = new()
                {
                    Locale = CultureInfo.InvariantCulture
                };
                dataAdapter.Fill(table);
                bindingSource1.DataSource = table;
            }
            catch (SqliteException ex)
            {
                MessageBox.Show($"Can't retrieve data from DB: {ex.Message}");
            }
        }

        private void InitListnerRuleChanged()
        {
            foreach (DataGridViewRow row in ListenerDataGridView.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }
                int? listenerId = GetListenerId(row);
                if (listenerId == null)
                {
                    continue;
                }
                int id = listenerId.Value;
                if (listnerRuleChanged.ContainsKey(id))
                {
                    listnerRuleChanged[id] = true;
                }
                else
                {
                    listnerRuleChanged.Add(id, true);
                }
            }
        }

        private void ListenerDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            DataGridViewColumn column = ListenerDataGridView.Columns[e.ColumnIndex];
            e.Cancel = column.Name.EndsWith("_port") && !HandlePortValue(e.RowIndex, e.ColumnIndex, column.HeaderText);
        }

        private bool HandlePortValue(int currentRowIndex, int currentColumnIndex, string currentColumnHeader)
        {
            DataGridViewRow currentRow = ListenerDataGridView.Rows[currentRowIndex];
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
                ListenerDataGridView.Columns["qso_port"],
                ListenerDataGridView.Columns["acknowledge_port"],
                ListenerDataGridView.Columns["forward_port"]
            ];

            int nameIndex = ListenerDataGridView.Columns["name"].Index;
            string currentColumnName = ListenerDataGridView.Columns[currentColumnIndex].Name;

            // Check for uniqueness
            foreach (DataGridViewRow otherRow in ListenerDataGridView.Rows)
            {
                // Skip empty rows
                if (otherRow.IsNewRow) continue;

                foreach (DataGridViewColumn otherColumn in ListenerDataGridView.Columns)
                {
                    DataGridViewCell otherCell = otherRow.Cells[otherColumn.Index];

                    // Skip checking the current cell
                    if (otherCell == currentCell)
                        continue;

                    // Skip non-port columns
                    if (!portColumns.Contains(otherColumn))
                        continue;

                    string? otherPort = otherCell.FormattedValue?.ToString();
                    // Skip checking other cell if empty
                    if (string.IsNullOrEmpty(otherPort))
                    {
                        continue;
                    }

                    // Forward port can be non-unique
                    if (currentColumnName == otherColumn.Name && currentColumnName == "forward_port")
                        continue;

                    // Acknowledge port could be the same as qso port but only within the same row
                    if (otherRow.Index == currentRowIndex &&
                        (currentColumnName == "qso_port" && currentColumnName == "acknowledge_port"
                        || currentColumnName == "acknowledge_port" && currentColumnName == "qso_port"
                        )
                       )
                    {
                        continue;
                    }

                    if (otherPort == portValue)
                    {
                        ListenerDataGridView.BeginEdit(true);
                        string errorMessage = $"Port must be unique accross all ports in config but {portValue} conflicts with '{ListenerDataGridView.Columns[otherCell.ColumnIndex].HeaderText}' of '{otherRow.Cells[nameIndex].FormattedValue}' Listener";
                        MessageBox.Show(errorMessage, "Port Uniqueness Violation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        currentCell.ErrorText = errorMessage;
                        currentRow.ErrorText = errorMessage;
                        ListenerDataGridView.EndEdit();
                        return false;
                    }
                }
            }
            ListenerDataGridView.BeginEdit(true);
            currentCell.ErrorText = string.Empty;
            currentRow.ErrorText = string.Empty;
            ListenerDataGridView.EndEdit();
            return true;
        }

        private void ListenerDataGridView_RowValidating(object sender, DataGridViewCellCancelEventArgs data)
        {
            DataGridViewRow row = ListenerDataGridView.Rows[data.RowIndex];
            // skip checking new row
            if (row.IsNewRow) return;
            // Validate all columns except id forward_port and acknowledge_port
            foreach (DataGridViewColumn column in ListenerDataGridView.Columns)
            {
                if (column.Name == "id" || column.Name == "forward_port" || column.Name == "acknowledge_port")
                    continue;
                DataGridViewCell cell = row.Cells[column.Index];
                if (cell.FormattedValue == null || string.IsNullOrWhiteSpace(cell.FormattedValue.ToString()))
                {
                    ListenerDataGridView.BeginEdit(true);
                    string errorMessage = $"The value of '{column.HeaderText}' must not be empty";
                    cell.ErrorText = errorMessage;
                    row.ErrorText = errorMessage;
                    exportConfigButton.Enabled = false;
                    ListenerDataGridView.EndEdit();
                    data.Cancel = true;
                    return;
                }
                row.ErrorText = string.Empty;
            }
            exportConfigButton.Enabled = true;
        }

        private void ListenerDataGridView_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["is_active"].Value = true;
            e.Row.Cells["message_format"].Value = "N1MM";
        }

        private void CancelEditListenersButton_Click(object sender, EventArgs e)
        {
            if (saveListenersButton.Enabled)
            {
                DialogResult result = MessageBox.Show("All unsaved changes will be lost. Do you want to continue?", "Cancel changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes)
                {
                    return;
                }
            }
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ListenerDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            cancelEditListenersButton.Text = "Cancel";
            saveListenersButton.Enabled = true;
        }

        private void SaveListenersButton_Click(object sender, EventArgs e)
        {
            // Validate rows before saving
            foreach (DataGridViewRow row in ListenerDataGridView.Rows)
            {
                if (row.IsNewRow) continue;
                if (!string.IsNullOrEmpty(row.ErrorText))
                {
                    MessageBox.Show("Please fix the errors before saving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Ensure the current edit is committed.
            ListenerDataGridView.EndEdit();
            // Save the data from the DataGridView to the database.
            try
            {
                deletedListenereIds.ForEach(listinerId =>
                {
                    List<int> satRuleIds = dbRepository.GetListenerSatRules(listinerId).Select(rule => (int)rule.Id).ToList();
                    dbRepository.RemoveRulesFromListener(listinerId, satRuleIds);
                });
                deletedListenereIds.Clear();
                dataAdapter.Update((DataTable)bindingSource1.DataSource);
            }
            catch (DBConcurrencyException)
            {
            }

            cancelEditListenersButton.Focus();
            cancelEditListenersButton.Text = "Close";
            saveListenersButton.Enabled = false;
            if (isLocalClientRunning)
            {
                MessageBox.Show("New config will be applied only after restarting Client", "Restart required", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            Init();
        }

        private void DeleteSelectedListenersButton_Click(object sender, EventArgs e)
        {
            List<DataGridViewRow> rowsToDelete = [.. ListenerDataGridView.SelectedRows
                .OfType<DataGridViewRow>()
                .Where(r => !r.IsNewRow)];

            if (rowsToDelete.Count == 0)
            {
                MessageBox.Show("No rows selected to delete.", "Delete Listeners", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            rowsToDelete.ForEach(r => {
                int? listenerId = GetListenerId(r);
                ListenerDataGridView.Rows.Remove(r);
                if (listenerId.HasValue)
                {
                    deletedListenereIds.Add(listenerId.Value);
                }
            });
            cancelEditListenersButton.Text = "Cancel";
            saveListenersButton.Enabled = true;
            exportConfigButton.Enabled = ListenerDataGridView.Rows.Count > 0;
            RefreshRuleButtons();
        }

        private void ListenerDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress += new KeyPressEventHandler(ListenerDataGridView_PortKeyPress);
        }

        private void ListenerDataGridView_PortKeyPress(object sender, KeyPressEventArgs e)
        {
            int currentColumnIndex = ListenerDataGridView.CurrentCell.ColumnIndex;
            int qsoPortColumnIndex = ListenerDataGridView.Columns["qso_port"].Index;
            int acknowledgePortColumnIndex = ListenerDataGridView.Columns["acknowledge_port"].Index;
            int forwardPortColumnIndex = ListenerDataGridView.Columns["forward_port"].Index;

            if (currentColumnIndex != qsoPortColumnIndex && currentColumnIndex != acknowledgePortColumnIndex && currentColumnIndex != forwardPortColumnIndex)
            {
                return;
            }

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void ExportConfigButton_Click(object sender, EventArgs e)
        {
            if (saveListenersButton.Enabled)
            {
                DialogResult result = MessageBox.Show("All changes must be saved first.\nDo you want to save them now and continue?", "Saving changes before export", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                {
                    return;
                }
                SaveListenersButton_Click(saveListenersButton, EventArgs.Empty);
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
                List<Models.ListenerConfig> listenerConfigs = dbRepository.GetListenerConfigs() ?? [];
                string jsonListenerConfigs = JToken.Parse(JsonSerializer.Serialize(listenerConfigs)).ToString();
                string filePath = saveFileDialog.FileName;
                File.WriteAllText(filePath, jsonListenerConfigs);
            }
            else
            {
                return;
            }
        }

        private void ImportConfigButton_Click(object sender, EventArgs e)
        {
            if (ListenerDataGridView.Rows.Count > 0 || dbRepository.GetListenerConfigs()?.Count > 0)
            {
                DialogResult result = MessageBox.Show("Existing configs will be replaced by imported. SAT Rules needs to be reassigned manually if needed. Do you want to continue?", "Existing config replacement", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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
                var listenerConfigs = JsonSerializer.Deserialize<List<Models.ListenerConfig>>(jsonListenerConfigs);
                dbRepository.ReplaceListenerConfigs(listenerConfigs);
                ListenersForm_Load(this, EventArgs.Empty);
            }
        }

        private void ListenerDataGridView_Sorted(object sender, EventArgs e)
        {
            RefreshRuleButtons();
        }

        private void ListenerDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                int? listenerId = GetListenerId(senderGrid.Rows[e.RowIndex]);
                if (saveListenersButton.Enabled || !listenerId.HasValue)
                {
                    MessageBox.Show("Please Save Listener Configs before dealing with Rules", "Listener Config saving required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ListenerRulesForm listenerRulesForm = new(dbRepository, listenerId.Value);
                listenerRulesForm.ShowDialog();
                if (listenerRulesForm.assignChanged)
                {
                    if (listnerRuleChanged.ContainsKey(listenerId.Value))
                    {
                        listnerRuleChanged[listenerId.Value] = true;
                    }
                    else
                    {
                        listnerRuleChanged.Add(listenerId.Value, true);
                    }
                    RefreshRuleButtons();
                }
            }
        }

        private int? GetListenerId(DataGridViewRow row)
        {
            string? id = row.Cells["id"].Value?.ToString();
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            return Convert.ToInt32(id);
        }
    }
}
