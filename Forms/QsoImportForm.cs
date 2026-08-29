using Microsoft.Data.Sqlite;
using QSOCollector.Data;
using QSOCollector.Forms;
using QSOCollector.Helpers;
using QSOCollector.Models;
using QSOCollector.Parsers;
using QSOCollector.Root;
using Serilog;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;

namespace QSOCollector
{
    public partial class QsoImportForm : Form
    {
        private readonly ILogger log = Log.ForContext<QsoImportForm>();
        private readonly IDbRepository dbRepository;
        private string? filePath = null;
        private string? fileName = null;
        private string? folder = null;
        private string? fileContent = null;
        private readonly DataTable ruleDataTable = new();

        public QsoImportForm(IDbRepository dbRepository)
        {
            this.dbRepository = dbRepository;
            InitializeComponent();
        }

        private void QsoImportForm_Load(object sender, EventArgs e)
        {
            PopulateRuleDataGridView();
        }

        private void PopulateRuleDataGridView()
        {
            string selectCommand = "select sr.id, sr.name, sr.is_apply_for_import from sat_rules sr where sr.is_active = 1";
            try
            {
                Log.Debug("Populate Listener Rules");
                ruleDataAdapter = new SQLiteDataAdapter(selectCommand, dbRepository.GetConnectionString());
                SQLiteCommandBuilder commandBuilder = new(ruleDataAdapter);
                ruleDataAdapter.Fill(ruleDataTable);
                ruleDataTable.PrimaryKey = [ruleDataTable.Columns["id"]];
                applyRuleDataGridView.DataSource = ruleBindingSource;
                ruleBindingSource.DataSource = ruleDataTable;
            }
            catch (SqliteException ex)
            {
                string message = "Can't retrieve data from DB";
                log.Error(ex, message);
                MessageBox.Show($"{message}: {ex.Message}");
            }
        }

        private void ImportButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(fileContent))
            {
                log.Warning("Import button clicked but file content is empty");
                MessageBox.Show("File is empty", "Invalid file content", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!fileContent.Contains(AdifToTableFieldsMapper.endOfRecord, StringComparison.OrdinalIgnoreCase))
            {
                log.Warning("Import button clicked but file doesn't contain valid ADIF records");
                MessageBox.Show("File doesn't contain valid ADIF records. Nothing to import", "Invalid file content", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            importProgressBar.Visible = true;
            importBackgroundWorker.RunWorkerAsync();
        }

        private void ChooseFileButton_Click(object sender, EventArgs e)
        {
            importProgressStep.Text = string.Empty;
            using OpenFileDialog openFileDialog = new();
            openFileDialog.InitialDirectory = Program.importFolder;
            openFileDialog.Filter = "ADIF files (*.adi)|*.adi|All files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                log.Information("File selected for import: {FilePath}", openFileDialog.FileName);
                fileName = openFileDialog.SafeFileName;
                folder = Path.GetDirectoryName(openFileDialog.FileName);
                filePath = openFileDialog.FileName;
                filePathLabel.Text = filePath;
                var fileStream = openFileDialog.OpenFile();
                using StreamReader reader = new(fileStream);
                fileContent = reader.ReadToEnd();
                int length = Math.Min(fileContent.Length, filePreviewTextBox.MaxLength);
                filePreviewTextBox.Text = fileContent[..length];
                HandleButton(importButton, true, "Continue import");
                closeCancelButton.Text = "Cancel";
            }
            else
            {
                log.Information("File selection cancelled");
                fileContent = null;
                filePath = null;
                fileContent = null;
                folder = null;
                fileName = null;
                filePathLabel.Text = "No file selected";
                filePreviewTextBox.Text = string.Empty;
                HandleButton(importButton, false);
                closeCancelButton.Text = "Close";
                return;
            }
        }
        private static void HandleButton(Button button, bool enabled, string? newText = null)
        {
            button.Enabled = enabled;
            if (newText != null) button.Text = newText;
            ButtonStyleHandler.Update(button, enabled);
        }

        private void ImportBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            importProgressBar.Value = e.ProgressPercentage;
        }

        private void ImportBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            int count = (int)e.Result;
            importProgressBar.Visible = false;
            HandleButton(importButton, false, $"Imported {count} QSOs");
            closeCancelButton.Text = "Close";
            MessageBox.Show($"{count} QSOs successfully imported", "QSOs imported", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ImportBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            QsoMessage qsoMessage = new()
            {
                Source = "Import",
                OriginalFormat = "ADIF",
                OriginalQsoData = fileContent,
                AdifQsoData = fileContent
            };
            List<Dictionary<string, string?>> qsoRecords = AdifToTableFieldsMapper.Map(qsoMessage, progressUpdater: UpdateProgressText);
            List<Dictionary<string, string?>> dups = dbRepository.ImportQsoRecords(qsoRecords, folder, fileName, UpdateProgressText);
            if (dups.Count > 0)
            {
                if (DialogResult.Yes == MessageBox.Show(
                    $"{dups.Count} duplicate QSOs found\nDo you want to Re-Upload them? (Not Recommended) ",
                    "Duplicate QSOs",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2
                ))
                {
                    dbRepository.ForceImportQsoRecords(dups, folder, fileName, UpdateProgressText);
                }
            }
            e.Result = qsoRecords.Count;
        }

        public List<Dictionary<string, string>> GetQsoRecords(QsoMessage qsoMessage)
        {
            List<Band> bands = dbRepository.GetBands();
            List<SatRule> rules = [..dbRepository.GetSatRules().Where(r => r.IsApplyForImport)];
            List<Dictionary<string, string?>> qsos = AdifToTableFieldsMapper.Map(qsoMessage, progressUpdater: UpdateProgressText);
            bool isOrigChanged = false;
            var newQsos = qsos;
            foreach (var rule in rules)
            {
                bool ruleApplied = SatRuleApplier.ApplyRuleToQsos(qsos, rule, bands, out var updatedQsos, out bool updatedIsOrigChanged);
                if (ruleApplied)
                {
                    log.Debug("Rule {RuleName} applied to QSO message from {Format}", rule.Name, qsoMessage.OriginalFormat);
                    newQsos = updatedQsos;
                    isOrigChanged |= updatedIsOrigChanged;
                }
            }
            if (isOrigChanged) {
                log.Debug("QSO message from {Format} has been modified by applied rules", qsoMessage.OriginalFormat);
                qsoMessage.AdifQsoData = AdifToTableFieldsMapper.Map(newQsos, withHeader: true);
            }
            return newQsos;
        }

        private async Task UpdateProgressText(string text)
        {
            importProgressStep.BeginInvoke((MethodInvoker)delegate
            {
                importProgressStep.Text = text;
            });
            importProgressStep.Text = text;
        }

        private void ApplyRuleDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (e.RowIndex < 0)
            {
                return;
            }

            DataGridViewColumn dataGridViewColumn = senderGrid.Columns[e.ColumnIndex];

            if (dataGridViewColumn is DataGridViewButtonColumn)
            {
                int ruleId = Convert.ToInt32(senderGrid.Rows[e.RowIndex].Cells["id"].Value);
                SatRule satRule = dbRepository.GetSatRule(ruleId);
                RuleForm ruleForm = new(dbRepository, satRule, true);
                ruleForm.ShowDialog();
            }
            else if (dataGridViewColumn is DataGridViewCheckBoxColumn)
            {
                int ruleId = Convert.ToInt32(senderGrid.Rows[e.RowIndex].Cells["id"].Value);
                DataGridViewCell applyRuleCheckBoxCell = senderGrid.Rows[e.RowIndex].Cells["isApplyForImport"];
                bool shouldBeChecked = !Convert.ToBoolean(applyRuleCheckBoxCell.Value);
                dbRepository.UpdateSatRuleApplyForImport(ruleId, shouldBeChecked);
                applyRuleCheckBoxCell.Value = shouldBeChecked;
            }
        }
    }
}
