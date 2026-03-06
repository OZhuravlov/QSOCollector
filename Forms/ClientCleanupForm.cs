using QSOCollector.Data;
using Serilog;

namespace QSOCollector
{
    public partial class ClientCleanupForm : Form
    {
        private readonly ILogger log = Log.ForContext<ClientCleanupForm>();

        private readonly IDbRepository dbRepository;

        public ClientCleanupForm(IDbRepository dbRepository)
        {
            InitializeComponent();
            this.dbRepository = dbRepository;
        }

        private void cleanTemporarelySavedQsoCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            HandleCheckBoxChanged(sender, e);
        }
        private void cleanUdpListenerConfigCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            HandleCheckBoxChanged(sender, e);
        }

        private void cleanServerIpAndPortConfigCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            HandleCheckBoxChanged(sender, e);
        }

        private void understandCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (understandCheckBox.Checked)
            {
                log.Warning("Client data cleanup. User checked 'Understand ...' checkbox");
            }
            else
            {
                log.Warning("Client data cleanup. User unchecked 'Understand ...' checkbox");
            }
            HandleCheckBoxChanged(sender, e);
        }

        private void HandleCheckBoxChanged(object sender, EventArgs e)
        {
            confirmCleanupButton.Enabled = (cleanTemporarelySavedQsoCheckBox.Checked ||
                                                cleanUdpListenerConfigCheckBox.Checked ||
                                                cleanServerIpAndPortConfigCheckBox.Checked)
                                                && understandCheckBox.Checked;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            log.Information("Client cleanup. User cancelled cleanup");
            Close();
        }

        private void confirmCleanupButton_Click(object sender, EventArgs e)
        {
            if (!confirmCleanupButton.Enabled) {
                log.Error("Client cleanup. User clicked confirm button but it's not enabled. This should not happen.");
                confirmCleanupButton.Enabled = false;
                return;
            }

            if (cleanTemporarelySavedQsoCheckBox.Checked)
            {
                log.Warning("Client cleanup. User confirmed to clean QSO data");
                dbRepository.CleanClientQsos();
                dbRepository.CleanRawQsoData();
            }

            if (cleanUdpListenerConfigCheckBox.Checked)
            {
                log.Warning("Client cleanup. User confirmed to clean UDP listener config");
                dbRepository.ReplaceListenerConfigs([]);
            }

            if (cleanServerIpAndPortConfigCheckBox.Checked)
            {
                log.Warning("Client cleanup. User confirmed to clean server IP and port config");
                dbRepository.SaveSetting("ClientServerNameIp", string.Empty);
                dbRepository.SaveSetting("ClientServerPort", string.Empty);
            }

            Close();
        }
    }
}
