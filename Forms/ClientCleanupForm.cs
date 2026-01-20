using QSOCollector.Data;

namespace QSOCollector
{
    public partial class ClientCleanupForm : Form
    {
        private readonly DbRepository dbRepository;

        public ClientCleanupForm(DbRepository dbRepository)
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

        private void HandleCheckBoxChanged(object sender, EventArgs e)
        {
            confirmCleanupButton.Enabled = cleanTemporarelySavedQsoCheckBox.Checked ||
                                                cleanUdpListenerConfigCheckBox.Checked ||
                                                cleanServerIpAndPortConfigCheckBox.Checked;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void confirmCleanupButton_Click(object sender, EventArgs e)
        {
            if (cleanTemporarelySavedQsoCheckBox.Checked)
            {
                dbRepository.CleanClientQsos();
            }

            if (cleanUdpListenerConfigCheckBox.Checked)
            {
                dbRepository.ReplaceListenerConfigs([]);
            }

            if (cleanServerIpAndPortConfigCheckBox.Checked)
            {
                dbRepository.SaveSetting("ClientServerNameIp", string.Empty);
                dbRepository.SaveSetting("ClientServerPort", string.Empty);
            }

            Close();
        }
    }
}
