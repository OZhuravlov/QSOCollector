using QSOCollector.Data;
using QSOCollector.Helpers;
using QSOCollector.Network.Server;
using QSOCollector.Root;
using Serilog;
using System.Runtime.InteropServices;
using System.Text;

namespace QSOCollector.Forms
{
    public partial class QsoAutoExportForm : Form
    {
        private readonly ILogger log = Log.ForContext<TcpServer>();

        private readonly IDbRepository dbRepository;
        private List<String> savedHours;
        private readonly List<String> hours;
        private string mainFolder = Program.defaultAutoExportFolder;

        public QsoAutoExportForm(IDbRepository dbRepository)
        {
            this.dbRepository = dbRepository;
            savedHours = dbRepository.GetExportHours();
            hours = [.. savedHours];
            InitializeComponent();
            enableExportSchedulerCheckBox.Checked = hours.Count > 0;
            HandleHourLabels();
            enableExportSchedulerCheckBox_CheckedChanged(enableExportSchedulerCheckBox, EventArgs.Empty);
            dbRepository.LoadSettings().TryGetValue("AutoExportFolder", out string? mainFolder);
            if (!string.IsNullOrEmpty(mainFolder))
            {
                this.mainFolder = mainFolder;
            }
            folderTextBox.Text = PathShortener(this.mainFolder, 140);
        }

        private void HandleHourLabels()
        {
            hours.Sort();
            foreach (Control control in hoursGroupBox.Controls)
            {
                if (control is not Label) continue;
                string hour = GetHourFromLabelName(control.Name);
                if (hours.Contains(hour))
                {
                    control.ForeColor = Color.Red;
                    control.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
                }
                else
                {
                    control.ForeColor = Color.Black;
                    control.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
                }
            }
            selectedHoursTextBox.Clear();
            selectedHoursTextBox.Text = hours.Count > 0 ? String.Join(", ", hours) : "No hours selected";
            HandleSaveExportSchedulerButton();
        }

        private void HandleSaveExportSchedulerButton()
        {
            saveExportScheduler.Text = (hours.Count > 0 && enableExportSchedulerCheckBox.Checked) ? "Start" : "Save";
            saveExportScheduler.Enabled = !enableExportSchedulerCheckBox.Checked || hours.Count > 0;
        }

        private void hourLabel_Click(object sender, EventArgs e)
        {
            if (everyHourRadioButton.Checked) return;

            Label label = sender as Label;
            if (label == null) return;
            string hour = GetHourFromLabelName(label.Name);
            if (hours.Contains(hour))
            {
                hours.Remove(hour);
            }
            else
            {
                hours.Add(hour);
            }
            HandleHourLabels();
        }

        private static string GetHourFromLabelName(String labelName)
        {
            return labelName.Replace("hour", "").Replace("Label", "");
        }

        private void saveExportScheduler_Click(object sender, EventArgs e)
        {
            if (enableExportSchedulerCheckBox.Checked && hours.Count == 0)
            {
                MessageBox.Show("Since Scheduler is enabled then at least one export execution hour must be selected. Please select or disable Scheduler", "Cannot save export scheduler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!enableExportSchedulerCheckBox.Checked)
            {
                hours.Clear();
            }

            dbRepository.SaveExportHours(hours);
            savedHours = [.. hours];
            dbRepository.SaveSetting("AutoExportFolder", mainFolder);
            this.Close();
        }

        private void QsoAutoExportForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (HandleCancelEditExportScheduler())
            {
                return;
            }
            e.Cancel = true;
        }


        private void cancelEditExportScheduler_Click(object sender, EventArgs e)
        {
            if (HandleCancelEditExportScheduler())
            {
                this.Close();
            }
        }

        private bool HandleCancelEditExportScheduler()
        {
            bool isChanged = hours.Any(hour => !savedHours.Contains(hour)) || savedHours.Any(hour => !hours.Contains(hour));
            if (isChanged)
            {
                DialogResult result = MessageBox.Show("Scheduler changed but not saved. All unsaved changes will be lost. Do you want to continue?", "Cancel changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes)
                {
                    return false;
                }
            }
            return true;
        }

        private void radioButtons_CheckedChanged(object sender, EventArgs e)
        {
            RadioButtonChanged();
        }

        private void RadioButtonChanged()
        {
            hours.Clear();
            if (everyHourRadioButton.Checked)
            {
                hours.Clear();
                for (int i = 0; i < 24; i++)
                {
                    hours.Add(i.ToString("D2"));
                }
            }
            HandleHourLabels();
        }

        private void enableExportSchedulerCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkbox = sender as CheckBox;
            Control? parent = checkbox.Parent;
            if (parent == null)
            {
                return;
            }
            HandleCheckBoxForChildControls(checkbox, parent, checkbox.Checked);
            HandleSaveExportSchedulerButton();
        }

        private static void HandleCheckBoxForChildControls(CheckBox checkbox, Control parentControl, bool enabled)
        {
            foreach (Control control in parentControl.Controls)
            {
                if (control == checkbox) continue;

                if (control is Button button)
                {
                    if (enabled)
                    {
                        ButtonStyleHandler.Update(button, enabled, Color.White);
                    }
                    else
                    {
                        ButtonStyleHandler.Update(button, enabled);
                    }
                    continue;
                }

                control.Enabled = enabled;
                HandleCheckBoxForChildControls(checkbox, control, enabled);
            }
        }

        private void chooseFolderButton_Click(object sender, EventArgs e)
        {
            using FolderBrowserDialog folderDialog = new();
            folderDialog.InitialDirectory = mainFolder;
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                mainFolder = folderDialog.SelectedPath;
                log.Information("Folder selected for auto export: {SelectedPath}", mainFolder);
                folderTextBox.Text = PathShortener(mainFolder, 140);
            }
            else
            {
                log.Information("File selection cancelled");
            }
        }

        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        static extern bool PathCompactPathEx(
               [Out] StringBuilder pszOut,
               string szPath,
               int cchMax,
               int dwFlags);

        static string PathShortener(string path, int length)
        {
            StringBuilder sb = new(length + 1);
            PathCompactPathEx(sb, path, length, 0);
            return sb.ToString();
        }
    }
}
