using QSOCollector.Data;
using Serilog;

namespace QSOCollector.Forms
{
    public partial class ServerCleanupForm : Form
    {
        private readonly ILogger log = Log.ForContext<ServerCleanupForm>();

        private readonly DbRepository dbRepository;

        public ServerCleanupForm(DbRepository dbRepository)
        {
            InitializeComponent();
            this.dbRepository = dbRepository;
        }

        private void understandCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (understandCheckBox.Checked)
            {
                log.Warning("Server QSO data cleanup. User checked 'Understand ...' checkbox");
            }
            else
            {
                log.Warning("Server QSO data cleanup. User unchecked 'Understand ...' checkbox");
            }
            confirmCleanupButton.Enabled = understandCheckBox.Checked;
        }


        private void cancelButton_Click(object sender, EventArgs e)
        {
            log.Information("Server QSO data cleanup. User cancelled cleanup");
            Close();
        }

        private void confirmCleanupButton_Click(object sender, EventArgs e)
        {

            if (understandCheckBox.Checked)
            {
                log.Warning("Server QSO data cleanup. User confirmed and start cleanup");
                dbRepository.CleanupServerQsoData();
            }

            Close();
        }
    }
}
