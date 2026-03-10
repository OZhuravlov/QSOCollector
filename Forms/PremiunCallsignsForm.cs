using Castle.Core.Resource;
using FileHelpers;
using Microsoft.VisualBasic.FileIO;
using QSOCollector.Data;
using QSOCollector.Models;
using QSOCollector.Root;
using QSOCollector.Service;
using Serilog;
using System.Data;
using System.Data.SQLite;
using System.Globalization;

namespace QSOCollector.Forms
{
    public partial class PremiunCallsignsForm : Form
    {
        private readonly ILogger log = Log.ForContext<AutoExportTaskService>();

        private readonly IDbRepository dbRepository;
        private DataTable dataTable;

        public PremiunCallsignsForm(IDbRepository dbRepository)
        {
            this.dbRepository = dbRepository;
            InitializeComponent();
        }

        private void PremiunCallsignsForm_Load(object sender, EventArgs e)
        {
            // Bind the DataGridView to the BindingSource
            // and load the data from the database.
            premiumCallsignsDataGridView.DataSource = premiumCallsignsBindingSource;
            GetDataForPremiumCallsignsDataGridView(dbRepository.GetConnectionString(), "select id, callsign, club, donated_amount_usd, comment from premium_callsign order by callsign");
        }

        private void GetDataForPremiumCallsignsDataGridView(string connectionString, string selectCommand)
        {
            try
            {
                dataAdapter = new SQLiteDataAdapter(selectCommand, connectionString);
                SQLiteCommandBuilder commandBuilder = new(dataAdapter);
                dataTable = new()
                {
                    Locale = CultureInfo.InvariantCulture
                };
                dataAdapter.Fill(dataTable);
                premiumCallsignsBindingSource.DataSource = dataTable;
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show($"Can't retrieve data from DB: {ex.Message}");
            }
        }

        private void premiumCallsignsDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress += new KeyPressEventHandler(premiumCallsignsDataGridView_KeyPress);
        }

        private void premiumCallsignsDataGridView_KeyPress(object sender, KeyPressEventArgs e)
        {
            int currentColumnIndex = premiumCallsignsDataGridView.CurrentCell.ColumnIndex;

            if (currentColumnIndex == premiumCallsignsDataGridView.Columns["callsign"].Index)
            {
                if (char.IsLetter(e.KeyChar))
                {
                    e.KeyChar = e.KeyChar.ToString().ToUpper()[0];
                    return;
                }

                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '/')
                {
                    e.Handled = true;
                }
                return;
            }

            if (currentColumnIndex == premiumCallsignsDataGridView.Columns["donated_amount_usd"].Index)
            {
                if (e.KeyChar == ',')
                {
                    e.KeyChar = '.';
                    return;
                }

                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }
                return;
            }
        }

        private void premiumCallsignsDataGridView_RowValidating(object sender, DataGridViewCellCancelEventArgs data)
        {
            DataGridViewRow row = premiumCallsignsDataGridView.Rows[data.RowIndex];
            // skip checking new row
            if (row.IsNewRow) return;
            // Validate all columns except id and description
            foreach (DataGridViewColumn column in premiumCallsignsDataGridView.Columns)
            {
                if (column.Name == "Id" || column.Name == "comment")
                    continue;
                DataGridViewCell cell = row.Cells[column.Index];
                if (cell.FormattedValue == null || string.IsNullOrWhiteSpace(cell.FormattedValue.ToString()))
                {
                    premiumCallsignsDataGridView.BeginEdit(true);
                    string errorMessage = $"The value of '{column.HeaderText}' must not be empty";
                    cell.ErrorText = errorMessage;
                    row.ErrorText = errorMessage;
                    premiumCallsignsDataGridView.EndEdit();
                    data.Cancel = true;
                    return;
                }
                row.ErrorText = string.Empty;
            }
        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            dataTable.DefaultView.RowFilter = $"callsign LIKE '%{searchTextBox.Text}%' OR club LIKE '%{searchTextBox.Text}%' OR comment LIKE '%{searchTextBox.Text}%'";
        }

        private void deleteSelectedRowsButton_Click(object sender, EventArgs e)
        {
            List<DataGridViewRow> rowsToDelete = premiumCallsignsDataGridView.SelectedRows
                .OfType<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .ToList();

            if (rowsToDelete.Count == 0)
            {
                MessageBox.Show("No rows selected to delete.", "Delete premium callsigns", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            rowsToDelete.ForEach(r => premiumCallsignsDataGridView.Rows.Remove(r));
            cancelEditButton.Text = "Cancel";
            saveButton.Enabled = true;
        }

        private void cancelEditButton_Click(object sender, EventArgs e)
        {
            if (saveButton.Enabled)
            {
                DialogResult result = MessageBox.Show("All unsaved changes will be lost. Do you want to continue?", "Cancel changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes)
                {
                    return;
                }
            }
            this.Close();
        }

        private void premiumCallsignsDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            DataGridViewRow currentRow = premiumCallsignsDataGridView.Rows[e.RowIndex];
            DataGridViewCell currentCell = currentRow.Cells[e.ColumnIndex];

            premiumCallsignsDataGridView.BeginEdit(true);
            currentCell.ErrorText = string.Empty;
            currentRow.ErrorText = string.Empty;
            premiumCallsignsDataGridView.EndEdit();
        }

        private void premiumCallsignsDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            cancelEditButton.Text = "Cancel";
            saveButton.Enabled = true;
        }

        private void premiumCallsignsDataGridView_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["club"].Value = "N/A";
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            // Validate rows before saving
            foreach (DataGridViewRow row in premiumCallsignsDataGridView.Rows)
            {
                if (row.IsNewRow) continue;
                if (!string.IsNullOrEmpty(row.ErrorText))
                {
                    MessageBox.Show("Please fix the errors before saving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Ensure the current edit is committed.
            premiumCallsignsDataGridView.EndEdit();
            // Save the data from the DataGridView to the database.
            dataAdapter.Update((DataTable)premiumCallsignsBindingSource.DataSource);

            cancelEditButton.Text = "Close";
            cancelEditButton.Focus();
            saveButton.Enabled = false;
        }

        private void uploadPremiumCallsignsButton_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new();
            openFileDialog.InitialDirectory = Program.configFolder;
            openFileDialog.Filter = "CSV files (*.csv)|*.csv";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var filename = openFileDialog.FileName;
            var engine = new FileHelperEngine<PremiumCallsign>();

            // To Read Use:
            PremiumCallsign[] result = engine.ReadFile(filename);
            foreach (var item in result)
            {
                log.Information("{item}", item);
            }
        }
    }
}
