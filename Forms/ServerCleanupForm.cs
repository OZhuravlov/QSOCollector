using QSOCollector.Data;

namespace QSOCollector.Forms
{
    public partial class ServerCleanupForm : Form
    {

        private readonly DbRepository dbRepository;

        public ServerCleanupForm(DbRepository dbRepository)
        {
            InitializeComponent();
            this.dbRepository = dbRepository;
        }

        private void understandCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            confirmCleanupButton.Enabled = understandCheckBox.Checked;
        }


        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void confirmCleanupButton_Click(object sender, EventArgs e)
        {

            if (understandCheckBox.Checked)
            {
                dbRepository.CleanupServerQsoData();
            }

            Close();
        }
    }
}
