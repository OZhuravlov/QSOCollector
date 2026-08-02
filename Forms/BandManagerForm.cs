using Microsoft.Data.Sqlite;
using QSOCollector.Data;
using System.Data;
using System.Data.SQLite;
using System.Globalization;

namespace QSOCollector.Forms
{
    public partial class BandManagerForm : Form
    {
        private readonly IDbRepository dbRepository;

        public BandManagerForm(IDbRepository dbRepository)
        {
            this.dbRepository = dbRepository;
            InitializeComponent();
        }

        private void BandManagerForm_Load(object sender, EventArgs e)
        {
            // Bind the DataGridView to the BindingSource
            // and load the data from the database.
            bandManagerDataGridView.DataSource = bandMgrBindingSource;
            PopulateBandManagerDataGridViewData();
        }

        private void PopulateBandManagerDataGridViewData()
        {
            string connectionString = dbRepository.GetConnectionString();
            string selectCommand = "select id, name, alt_name, n1mm_name, freq_mhz_from, freq_mhz_to, designator, is_active from bands";
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
                bandMgrBindingSource.DataSource = table;
            }
            catch (SqliteException ex)
            {
                MessageBox.Show($"Can't retrieve data from DB: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
                return;
            }
        }

        private void BandManagerDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            DataGridViewColumn column = bandManagerDataGridView.Columns[e.ColumnIndex];
            e.Cancel = (column.Name.StartsWith("freq") && !HandleFreqValue(e.RowIndex, e.ColumnIndex, column.HeaderText)
                || column.Name == "designator" && !HandleDesignatorValue(e.RowIndex, e.ColumnIndex)
                );
        }

        private bool HandleFreqValue(int currentRowIndex, int currentColumnIndex, string currentColumnHeader)
        {
            DataGridViewRow currentRow = bandManagerDataGridView.Rows[currentRowIndex];
            DataGridViewCell currentCell = currentRow.Cells[currentColumnIndex];
            string? freqValue = currentCell.EditedFormattedValue?.ToString();

            // Skip checking empty values
            if (string.IsNullOrEmpty(freqValue)) return true;

            if (!double.TryParse(freqValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double freq) || freq <= 0)
            {
                currentRow.ErrorText = $"The value {freqValue} of '{currentColumnHeader}' must be a positive number";
                return false;
            }

            int nameIndex = bandManagerDataGridView.Columns["bandName"].Index;
            int freqFromIndex = bandManagerDataGridView.Columns["freqFrom"].Index;
            int freqToIndex = bandManagerDataGridView.Columns["freqTo"].Index;
            if (currentColumnIndex == freqFromIndex)
            {
                string? freqToValue = currentRow.Cells[freqToIndex].FormattedValue?.ToString();
                if (!string.IsNullOrEmpty(freqToValue)
                    && double.TryParse(freqToValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double freqTo) && freq >= freqTo)
                {
                    currentRow.ErrorText = $"The value {freqValue} of '{currentColumnHeader}' must be less than 'Freq To' ({freqTo})";
                    return false;
                }
            }
            else if (currentColumnIndex == freqToIndex)
            {
                string? freqFromValue = currentRow.Cells[freqFromIndex].FormattedValue?.ToString();
                if (!string.IsNullOrEmpty(freqFromValue)
                    && double.TryParse(freqFromValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double freqFrom) && freq <= freqFrom)
                {
                    currentRow.ErrorText = $"The value {freqValue} of '{currentColumnHeader}' must be greater than 'Freq From' ({freqFrom})";
                    return false;
                }
            }

            // Check for uniqueness
            foreach (DataGridViewRow otherRow in bandManagerDataGridView.Rows)
            {
                // Skip same or empty rows
                if (otherRow.IsNewRow || otherRow == currentRow) continue;
                bool otherFreqFromParsed = double.TryParse(otherRow.Cells[freqFromIndex].FormattedValue?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double otherFreqFrom);
                bool otherFreqToParsed = double.TryParse(otherRow.Cells[freqToIndex].FormattedValue?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double otherFreqTo);

                if (otherFreqFromParsed && otherFreqToParsed && freq > otherFreqFrom && freq < otherFreqTo)
                {
                    bandManagerDataGridView.BeginEdit(true);
                    string errorMessage = $"Frequency range must be unique across all bands in config but {freqValue} conflicts with '{otherRow.Cells[nameIndex].FormattedValue}' Band";
                    MessageBox.Show(errorMessage, "Frequency Uniqueness Violation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    currentCell.ErrorText = errorMessage;
                    currentRow.ErrorText = errorMessage;
                    bandManagerDataGridView.EndEdit();
                    return false;
                }
            }
            if (currentCell.ErrorText != string.Empty || currentRow.ErrorText != string.Empty)
            {
                bandManagerDataGridView.BeginEdit(true);
                currentCell.ErrorText = string.Empty;
                currentRow.ErrorText = string.Empty;
                bandManagerDataGridView.EndEdit();
            }
            return true;
        }

        private bool HandleDesignatorValue(int currentRowIndex, int currentColumnIndex)
        {
            DataGridViewRow currentRow = bandManagerDataGridView.Rows[currentRowIndex];
            DataGridViewCell currentCell = currentRow.Cells[currentColumnIndex];
            string? designatorValue = currentCell.EditedFormattedValue?.ToString();

            // Skip checking empty values
            if (string.IsNullOrEmpty(designatorValue)) return true;

            int nameIndex = bandManagerDataGridView.Columns["bandName"].Index;
            // Check for uniqueness
            foreach (DataGridViewRow otherRow in bandManagerDataGridView.Rows)
            {
                // Skip same or empty rows
                if (otherRow.IsNewRow || otherRow == currentRow) continue;
                string? otherDesignatorValue = otherRow.Cells[currentColumnIndex].FormattedValue?.ToString();
                if (otherDesignatorValue == designatorValue)
                {
                    bandManagerDataGridView.BeginEdit(true);
                    string errorMessage = $"Band abbr must be unique across all bands in config but {designatorValue} conflicts with '{otherRow.Cells[nameIndex].FormattedValue}' Band";
                    MessageBox.Show(errorMessage, "Designator Uniqueness Violation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    currentCell.ErrorText = errorMessage;
                    currentRow.ErrorText = errorMessage;
                    bandManagerDataGridView.EndEdit();
                    return false;
                }
            }
            if (currentCell.ErrorText != string.Empty || currentRow.ErrorText != string.Empty)
            {
                bandManagerDataGridView.BeginEdit(true);
                currentCell.ErrorText = string.Empty;
                currentRow.ErrorText = string.Empty;
                bandManagerDataGridView.EndEdit();
            }
            return true;
        }

        private void BandManagerDataGridView_RowValidating(object sender, DataGridViewCellCancelEventArgs data)
        {
            DataGridViewRow currentRow = bandManagerDataGridView.Rows[data.RowIndex];
            // skip checking new row
            if (currentRow.IsNewRow) return;
            // Validate all columns except id and designator
            foreach (DataGridViewColumn column in bandManagerDataGridView.Columns)
            {
                if (column.Name == "id" || column.Name == "designator")
                    continue;
                DataGridViewCell cell = currentRow.Cells[column.Index];
                if (cell.FormattedValue == null || string.IsNullOrWhiteSpace(cell.FormattedValue.ToString()))
                {
                    bandManagerDataGridView.BeginEdit(true);
                    string errorMessage = $"The value of '{column.HeaderText}' must not be empty";
                    cell.ErrorText = errorMessage;
                    currentRow.ErrorText = errorMessage;
                    bandManagerDataGridView.EndEdit();
                    data.Cancel = true;
                    return;
                }
                currentRow.ErrorText = string.Empty;
            }

            int nameIndex = bandManagerDataGridView.Columns["bandName"].Index;
            int freqFromIndex = bandManagerDataGridView.Columns["freqFrom"].Index;
            int freqToIndex = bandManagerDataGridView.Columns["freqTo"].Index;

            bool freqFromParsed = double.TryParse(currentRow.Cells[freqFromIndex].FormattedValue.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double freqFromValue);
            bool freqToParsed = double.TryParse(currentRow.Cells[freqToIndex].FormattedValue.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double freqToValue);
            if (!freqFromParsed)
            {
                bandManagerDataGridView.BeginEdit(true);
                string errorMessage = $"The value of 'Freq from, mHz' is not a valid number";
                currentRow.Cells[freqFromIndex].ErrorText = errorMessage;
                currentRow.ErrorText = errorMessage;
                bandManagerDataGridView.EndEdit();
                data.Cancel = true;
                return;
            }
            if (!freqToParsed)
            {
                bandManagerDataGridView.BeginEdit(true);
                string errorMessage = $"The value of 'Freq to, mHz' is not a valid number";
                currentRow.Cells[freqToIndex].ErrorText = errorMessage;
                currentRow.ErrorText = errorMessage;
                bandManagerDataGridView.EndEdit();
                data.Cancel = true;
                return;
            }

            // Check for uniqueness
            foreach (DataGridViewRow otherRow in bandManagerDataGridView.Rows)
            {
                // Skip same or empty rows
                if (otherRow.IsNewRow || otherRow == currentRow) continue;
                bool otherFreqFromParsed = double.TryParse(otherRow.Cells[freqFromIndex].FormattedValue?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double otherFreqFrom);
                bool otherFreqToParsed = double.TryParse(otherRow.Cells[freqToIndex].FormattedValue?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double otherFreqTo);

                if (!otherFreqFromParsed || !otherFreqToParsed) continue;

                if (freqFromValue >= otherFreqFrom && freqFromValue <= otherFreqTo
                    || freqToValue >= otherFreqFrom && freqToValue <= otherFreqTo
                    || freqFromValue <= otherFreqFrom && freqToValue >= otherFreqTo)
                {
                    bandManagerDataGridView.BeginEdit(true);
                    string errorMessage = $"Frequency range {freqFromValue}-{freqToValue} conflicts with '{otherRow.Cells[nameIndex].FormattedValue}' ({otherFreqFrom}-{otherFreqTo}) Band";
                    MessageBox.Show(errorMessage, "Frequency Uniqueness Violation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    currentRow.ErrorText = errorMessage;
                    bandManagerDataGridView.EndEdit();
                    data.Cancel = true;
                    return;
                }
            }
        }

        private void BandCancelEditButton_Click(object sender, EventArgs e)
        {
            if (bandSaveButton.Enabled)
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

        private void BandManagerDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            bandCancelEditButton.Text = "Cancel";
            bandSaveButton.Enabled = true;
        }

        private void BandSaveButton_Click(object sender, EventArgs e)
        {
            // Validate rows before saving
            foreach (DataGridViewRow row in bandManagerDataGridView.Rows)
            {
                if (row.IsNewRow) continue;
                if (!string.IsNullOrEmpty(row.ErrorText))
                {
                    MessageBox.Show("Please fix the errors before saving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Ensure the current edit is committed.
            bandManagerDataGridView.EndEdit();
            // Save the data from the DataGridView to the database.
            try
            {
                dataAdapter.Update((DataTable)bandMgrBindingSource.DataSource);
            }
            catch (SqliteException ex)
            {
                string errorMessage = ex.SqliteExtendedErrorCode switch
                {
                    IDbRepository.SQLITE_CONSTRAINT_FOREIGN_KEY => "The band(s) you are trying to save are referenced by SAT Rules. Revoke the references and try again.",
                    IDbRepository.SQLITE_CONSTRAINT_UNIQUE => "The band(s) you are trying to save have duplicate names. Ensure all bands have unique names and try again.",
                    _ => $"An error occurred while saving the data: {ex.Message}"
                };
                MessageBox.Show(errorMessage, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (DBConcurrencyException)
            {
                MessageBox.Show("A concurrency error occurred while saving the data.", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            PopulateBandManagerDataGridViewData();
            bandCancelEditButton.Text = "Close";
            bandCancelEditButton.Focus();
            bandSaveButton.Enabled = false;
        }


        private void BandDeleteButton_Click(object sender, EventArgs e)
        {
            List<DataGridViewRow> rowsToDelete = [.. bandManagerDataGridView.SelectedRows
                .OfType<DataGridViewRow>()
                .Where(r => !r.IsNewRow)];

            if (rowsToDelete.Count == 0)
            {
                MessageBox.Show("No rows selected to delete.", "Delete Listeners", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            rowsToDelete.ForEach(r => bandManagerDataGridView.Rows.Remove(r));
            bandCancelEditButton.Text = "Cancel";
            bandSaveButton.Enabled = true;
        }

        private void BandManagerDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress += new KeyPressEventHandler(BandManagerDataGridView_KeyPress);
        }

        private void BandManagerDataGridView_KeyPress(object sender, KeyPressEventArgs e)
        {
            int currentColumnIndex = bandManagerDataGridView.CurrentCell.ColumnIndex;
            int freqFromColumnIndex = bandManagerDataGridView.Columns["freqFrom"].Index;
            int freqToColumnIndex = bandManagerDataGridView.Columns["freqTo"].Index;

            if (currentColumnIndex != freqFromColumnIndex && currentColumnIndex != freqToColumnIndex)
            {
                return;
            }

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != ',')
            {
                e.Handled = true;
            }

            if (e.KeyChar == ',') {
                e.KeyChar = '.';
            }

            // only allow one decimal point
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void BandManagerDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            BandDeleteButton.Enabled = bandManagerDataGridView.SelectedRows.Count > 0;
        }
    }
}
