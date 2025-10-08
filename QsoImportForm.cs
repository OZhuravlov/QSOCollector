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
            if (!fileContent.Contains(AdifToTableFieldsMapper.endOfRecord))
            {
                MessageBox.Show("The file doesn't contain valid ADIF records. Nothing to import", "Invalid file content", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            QsoMessage qsoMessage = new()
            {
                Source = "Import",
                OriginalFormat = "ADIF",
                OriginalQsoData = fileContent,
                AdifQsoData = fileContent
            };
            List<Dictionary<string, string?>> qsoRecords = AdifToTableFieldsMapper.Map(qsoMessage);
            dbRepository.ImportQsoRecords(qsoRecords, folder, fileName);
            importButton.Text = $"Imported {qsoRecords.Count} QSOs";
            importButton.Enabled = false;
            closeCancelButton.Text = "Close";
            MessageBox.Show($"{qsoRecords.Count} QSOs successfully imported", "QSOs imported", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void chooseFileButton_Click(object sender, EventArgs e)
        {
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
                importButton.Enabled = true;
                importButton.Text = "Continue import";
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
                importButton.Enabled = false;
                closeCancelButton.Text = "Close";
                return;
            }
        }
    }
}
