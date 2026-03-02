using QSOCollector.Data;
using QSOCollector.Helpers;

namespace QSOCollector.Forms
{
    public partial class QsoAutoExportForm : Form
    {
        private readonly DbRepository dbRepository;
        private List<String> savedHours;
        private List<String> hours;

        public QsoAutoExportForm(DbRepository dbRepository)
        {
            this.dbRepository = dbRepository;
            savedHours = dbRepository.GetExportHours();
            hours = [.. savedHours];
            InitializeComponent();
            enableExportSchedulerCheckBox.Checked = hours.Count > 0;
            HandleHourLabels();
            enableExportSchedulerCheckBox_CheckedChanged(enableExportSchedulerCheckBox, EventArgs.Empty);
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
            string hoursText = hours.Count > 0 ? String.Join(", ", hours) : "No hours selected";
            selectedHoursTextBox.Text = String.Join(", ", hours);
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
            if (enableExportSchedulerCheckBox.Checked && hours.Count == 0) {
                MessageBox.Show("Since Scheduler is enabled then at least one export execution hour must be selected. Please select or disable Scheduler", "Cannot save export scheduler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!enableExportSchedulerCheckBox.Checked)
            {
                hours.Clear();
            }

            dbRepository.SaveExportHours(hours);
            savedHours = [.. hours];
            this.Close();
        }

        private void QsoAutoExportForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (HandleCancelEditExportScheduler()) {
                return; 
            }
            e.Cancel = true;
        }


        private void cancelEditExportScheduler_Click(object sender, EventArgs e)
        {
            if (HandleCancelEditExportScheduler()) {
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
            if (everyHourRadioButton.Checked) {
                hours.Clear();
                for (int i = 0; i < 24; i++) {
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
        }

        private static void HandleCheckBoxForChildControls(CheckBox checkbox, Control parentControl, bool enabled)
        {
            foreach (Control control in parentControl.Controls)
            {
                if (control == checkbox) continue;

                if (control is Button button)
                {
                    if (enabled) {
                        ButtonStyleHandler.Update(button, enabled, Color.White);
                    } else {
                        ButtonStyleHandler.Update(button, enabled);
                    }
                    continue;
                }

                control.Enabled = enabled;
                HandleCheckBoxForChildControls(checkbox, control, enabled);
            }
        }

    }
}
