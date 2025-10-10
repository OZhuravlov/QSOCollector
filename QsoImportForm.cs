using System.ComponentModel;

namespace QSOCollector
{
    public partial class QsoImportForm : Form
    {
        private readonly DbRepository dbRepository;
        private string? filePath = null;
        private string? fileName = null;
        private string? folder = null;
        private string? fileContent = null;

        public QsoImportForm(DbRepository dbRepository)
        {
            this.dbRepository = dbRepository;
            InitializeComponent();
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(fileContent))
            {
                MessageBox.Show("The file is empty", "Invalid file content", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!fileContent.Contains(AdifToTableFieldsMapper.endOfRecord, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("The file doesn't contain valid ADIF records. Nothing to import", "Invalid file content", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            importProgressBar.Visible = true;
            importBackgroundWorker.RunWorkerAsync();
        }

        private void chooseFileButton_Click(object sender, EventArgs e)
        {
            importProgressStep.Text = string.Empty;
            using OpenFileDialog openFileDialog = new();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
            openFileDialog.Filter = "ADIF files (*.adi)|*.adi|All files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog.SafeFileName;
                folder = Path.GetDirectoryName(openFileDialog.FileName);
                filePath = openFileDialog.FileName;
                filePathLabel.Text = filePath;
                var fileStream = openFileDialog.OpenFile();
                using StreamReader reader = new(fileStream);
                fileContent = reader.ReadToEnd();
                int length = Math.Min(fileContent.Length, filePreviewTextBox.MaxLength);
                filePreviewTextBox.Text = fileContent[..length];
                handleButton(importButton, true, "Continue import");
                closeCancelButton.Text = "Cancel";
            }
            else
            {
                fileContent = null;
                filePath = null;
                fileContent = null;
                folder = null;
                fileName = null;
                filePathLabel.Text = "No file selected";
                filePreviewTextBox.Text = string.Empty;
                handleButton(importButton, false);
                closeCancelButton.Text = "Close";
                return;
            }
        }
        private void handleButton(Button button, bool enabled, string? newText = null)
        {
            button.Enabled = enabled;
            if (newText != null) button.Text = newText;
            ButtonStyleHandler.Update(button, enabled);
        }

        private void importBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            importProgressBar.Value = e.ProgressPercentage;
        }

        private void importBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            int count = (int)e.Result;
            importProgressBar.Visible = false;
            handleButton(importButton, false, $"Imported {count} QSOs");
            closeCancelButton.Text = "Close";
            MessageBox.Show($"{count} QSOs successfully imported", "QSOs imported", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void importBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            QsoMessage qsoMessage = new()
            {
                Source = "Import",
                OriginalFormat = "ADIF",
                OriginalQsoData = fileContent,
                AdifQsoData = fileContent
            };
            List<Dictionary<string, string?>> qsoRecords = AdifToTableFieldsMapper.Map(qsoMessage, progressUpdater: updateProgressText);
            List<Dictionary<string, string?>> dups = dbRepository.ImportQsoRecords(qsoRecords, folder, fileName, updateProgressText);
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
                    dbRepository.ForceImportQsoRecords(dups, folder, fileName, updateProgressText);
                }
            }
            e.Result = qsoRecords.Count;
        }

        private async Task updateProgressText(string text)
        {
            importProgressStep.Invoke((MethodInvoker)delegate
            {
                importProgressStep.Text = text;
            });
            importProgressStep.Text = text;
        }

    }
}
