using QSOCollector.Data;
using QSOCollector.Helpers;
using QSOCollector.Models;
using QSOCollector.Root;
using Serilog;
using System.Diagnostics;
using System.Text;

namespace QSOCollector
{
    public partial class QsoExportForm : Form
    {
        private readonly ILogger log = Log.ForContext<QsoExportForm>();

        private readonly IDbRepository dbRepository;
        private List<QsoExportExpectedAmounts> expectedAmounts;
        private readonly QsoExportFilters exportFilters = new();

        public QsoExportForm(IDbRepository dbRepository, List<QsoExportExpectedAmounts> expectedAmounts)
        {
            this.dbRepository = dbRepository;
            this.expectedAmounts = expectedAmounts;
            InitializeComponent();
            InitValues();
        }

        private void InitValues()
        {
            totalAmountLabel.Text = expectedAmounts.Sum(r => r.Count).ToString();
            newQSOsRadioButton.Checked = true;
            exportFilters.IsNewOnly = true;
            RadioButtonChanged();
            InitSecondaryFilters();
            RecalcFiltered();
        }

        private void ResetDateFromPicker()
        {
            dateFromTimePicker.Value = dateFromTimePicker.MinDate;
            exportFilters.DateFrom = dateFromTimePicker.Value.Date;
            ResetDatePicker(dateFromTimePicker);
        }

        private void ResetDateToPicker()
        {
            dateToTimePicker.Value = dateFromTimePicker.MaxDate;
            exportFilters.DateTo = dateToTimePicker.Value.Date;
            ResetDatePicker(dateToTimePicker);
        }
        private static void ResetDatePicker(DateTimePicker picker)
        {
            picker.Checked = false;
            picker.CustomFormat = " ";
            picker.Format = DateTimePickerFormat.Custom;
        }

        private void radioButtons_CheckedChanged(object sender, EventArgs e)
        {
            RadioButtonChanged();
            RecalcFiltered();
        }

        private void RadioButtonChanged()
        {
            exportFilters.IsNewOnly = newQSOsRadioButton.Checked;
            dateFromTimePicker.Enabled = byDateRadioButton.Checked;
            dateToTimePicker.Enabled = byDateRadioButton.Checked;
            exportButton.Text = "Export";
            if (byDateRadioButton.Checked)
            {
                SetDateFromPicker();
                SetDateToPicker();
            }
            else
            {
                ResetDateFromPicker();
                ResetDateToPicker();
            }
            ClearSecondaryFilters();
            InitSecondaryFilters();
        }

        private void ClearSecondaryFilters()
        {
            secondaryFiltersGroupBox.Controls.OfType<ComboBox>().ToList()
                .ForEach(c =>
                {
                    c.Items.Clear();
                    c.SelectedIndex = -1;
                    c.SelectedItem = null;
                    c.Text = null;
                });
        }

        private void InitSecondaryFilters()
        {
            exportFilters.ModeGroup = null;
            exportFilters.Mode = null;
            exportFilters.Band = null;
            exportFilters.SourceName = null;
            exportFilters.Operator = null;
            exportFilters.SourceIp = null;
            ResetSecondaryFilter(modeGroupComboBox, GetFilteredAmounts(exportFilters).Select(r => r.ModeGroup).Distinct().OrderBy(r => r));
            ResetSecondaryFilter(modeComboBox, GetFilteredAmounts(exportFilters).Select(r => r.Mode).Distinct().OrderBy(r => r));
            ResetSecondaryFilter(bandComboBox, GetFilteredAmounts(exportFilters).Select(r => r.Band).Distinct().OrderByDescending(r => Int32.Parse(r.TrimEnd('M'))));
            ResetSecondaryFilter(sourceNameComboBox, GetFilteredAmounts(exportFilters).Select(r => r.SourceName).Distinct().OrderBy(r => r == "<UNKNOWN>").ThenBy(r => r));
            ResetSecondaryFilter(operatorComboBox, GetFilteredAmounts(exportFilters).Select(r => r.Operator).Distinct().OrderBy(r => r == "<UNKNOWN>").ThenBy(r => r));
            ResetSecondaryFilter(sourceIpComboBox, GetFilteredAmounts(exportFilters).Select(r => r.SourceIp).Distinct().OrderBy(r => r == "<UNKNOWN>").ThenBy(r => r));
        }

        private static void ResetSecondaryFilter(ComboBox comboBox, IOrderedEnumerable<string> newValues)
        {
            int defaultIndex = -1;
            int currIndex = comboBox.SelectedIndex;
            string? currValue = comboBox.SelectedItem?.ToString();
            comboBox.Items.Clear();
            comboBox.Items.AddRange([.. newValues]);

            comboBox.SelectedIndex = defaultIndex;
            comboBox.SelectedItem = null;
            comboBox.Text = null;
            if (currIndex != defaultIndex)
            {
                for (int i = 0; i < comboBox.Items.Count; i++)
                {
                    if (comboBox.Items[i].ToString() == currValue)
                    {
                        comboBox.SelectedIndex = i;
                        comboBox.SelectedItem = currValue;
                        comboBox.Text = currValue;
                    }
                }
            }
        }

        private void SetDateFromPicker()
        {
            DateTime dateFrom = expectedAmounts.Min(r => r.QsoDate);
            exportFilters.DateFrom = dateFrom;
            SetDateTimePicker(dateFromTimePicker, dateFrom);
        }

        private void SetDateToPicker()
        {
            DateTime dateTo = expectedAmounts.Max(r => r.QsoDate);
            exportFilters.DateTo = dateTo;
            SetDateTimePicker(dateToTimePicker, dateTo);
        }

        private static void SetDateTimePicker(DateTimePicker picker, DateTime value)
        {
            picker.Enabled = true;
            picker.Format = DateTimePickerFormat.Short;
            picker.Value = value;
        }

        private void dateFromTimePicker_EnabledChanged(object sender, EventArgs e)
        {
            if (dateFromTimePicker.Enabled)
            {
                SetDateFromPicker();
            }
            else
            {
                ResetDateFromPicker();
            }
            RecalcFiltered();
        }

        private void dateToTimePicker_EnabledChanged(object sender, EventArgs e)
        {
            if (dateToTimePicker.Enabled)
            {
                SetDateToPicker();
            }
            else
            {
                ResetDateToPicker();
            }
            RecalcFiltered();
        }

        private void dateFromTimePicker_ValueChanged(object sender, EventArgs e)
        {
            exportFilters.DateFrom = dateFromTimePicker.Value.Date;
            RecalcFiltered();
        }

        private void dateToTimePicker_ValueChanged(object sender, EventArgs e)
        {
            exportFilters.DateTo = dateToTimePicker.Value.Date;
            RecalcFiltered();
        }

        private List<QsoExportExpectedAmounts> GetFilteredAmounts(QsoExportFilters filters)
        {
            return [.. expectedAmounts
                .Where(r => !(filters.IsNewOnly ?? false) || (filters.IsNewOnly == true && !r.IsExported))
                .Where(r => r.QsoDate >= (filters.DateFrom ?? DateTime.MinValue) && r.QsoDate <= (filters.DateTo ?? DateTime.MaxValue))
                .Where(r => filters.ModeGroup == null || r.ModeGroup == filters.ModeGroup)
                .Where(r => filters.Mode == null || r.Mode == filters.Mode)
                .Where(r => filters.Band == null || r.Band == filters.Band)
                .Where(r => filters.Operator == null || r.Operator == filters.Operator)
                .Where(r => filters.SourceName == null || r.SourceName == filters.SourceName)
                .Where(r => filters.SourceIp == null || r.SourceIp == filters.SourceIp)];
        }

        private int GetCalcFiltered()
        {
            return GetFilteredAmounts(exportFilters).Sum(r => r.Count);
        }

        private void RecalcFiltered()
        {
            filteredAmountLabel.Text = GetCalcFiltered().ToString();
        }

        private void modeGroupComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            exportFilters.ModeGroup = GetComboBoxValue(modeGroupComboBox);
            var filters = (QsoExportFilters)exportFilters.Clone();
            filters.Mode = null;
            ResetSecondaryFilter(modeComboBox, GetFilteredAmounts(filters).Select(r => r.Mode).Distinct().OrderBy(r => r));
            RecalcFiltered();
        }

        private void modeComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            exportFilters.Mode = GetComboBoxValue(modeComboBox);
            RecalcFiltered();
        }

        private void sourceNameComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            exportFilters.SourceName = GetComboBoxValue(sourceNameComboBox);
            RecalcFiltered();
        }

        private void bandComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            exportFilters.Band = GetComboBoxValue(bandComboBox);
            RecalcFiltered();
        }

        private void operatorComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            exportFilters.Operator = GetComboBoxValue(operatorComboBox);
            RecalcFiltered();
        }

        private void sourceIpComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            exportFilters.SourceIp = GetComboBoxValue(sourceIpComboBox);
            RecalcFiltered();
        }

        private string? GetComboBoxValue(ComboBox comboBox)
        {
            return comboBox.SelectedIndex == -1 || string.IsNullOrWhiteSpace(comboBox.Text) ? null : comboBox.Text;
        }

        private void resetSecondaryFiltersButton_Click(object sender, EventArgs e)
        {
            ClearSecondaryFilters();
            InitSecondaryFilters();
            RecalcFiltered();
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            if (GetCalcFiltered() == 0)
            {
                DialogResult dialogResultStartExport = MessageBox.Show("It looks like no QSO expected to be exported. " +
                    "Do you want to try fetching from Database?", "Nothing to export",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);

                if (dialogResultStartExport == DialogResult.No)
                {
                    return;
                }
            }

            Dictionary<int, string> adifEntries = dbRepository.GetAdif(exportFilters);

            if (adifEntries.Count == 0)
            {
                log.Information("No QSO found to export with current filters: {@Filters}", exportFilters);
                MessageBox.Show("No QSO found to export with current filters", "Nothing to export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime now = DateTime.UtcNow;

            var sb = new StringBuilder($"# UR8UQ DXpedition QSO Collector v.{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}\r\n#   Created:  {now:yyyy-MM-dd HH:mm:ss}\r\n#\r\n<ADIF_VER:5>3.1.6\r\n<EOH>\r\n");
            foreach (var item in adifEntries.Values)
            {
                sb.AppendLine(item);
            }

            var fileContent = sb.ToString();
            var filePath = string.Empty;
            var fileName = string.Empty;
            var folder = string.Empty;

            using (SaveFileDialog saveFileDialog = new()
            {
                InitialDirectory = Program.exportFolder,
                FileName = $"export_{now:yyyyMMdd_HHmmss}.adi",
                Filter = "ADIF files (*.adi)|*.adi|All files (*.*)|*.*",
                DefaultExt = "adi",
                AddExtension = true,
                RestoreDirectory = true
            })
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = saveFileDialog.FileName;
                    folder = Path.GetDirectoryName(filePath);
                    fileName = Path.GetFileName(filePath);
                    log.Information("Exporting {Count} QSO(s) to file {FilePath}", adifEntries.Count, filePath);
                    File.WriteAllText(filePath, fileContent);
                }
                else
                {
                    log.Information("Export cancelled by user");
                    return;
                }
            }

            DialogResult dialogResultSetExported = MessageBox.Show(
                $"ADIF file with {adifEntries.Count} QSO(s) has been saved to file {filePath}\nWould you like to set these QSO(s) in database as exported (Recommended)\n(This could help do not export duplicates next time)",
                "ADIF exported",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1
            );

            if (dialogResultSetExported == DialogResult.Yes)
            {
                log.Information("Setting {Count} QSO(s) as exported in database", adifEntries.Count);
                dbRepository.SetQSOsExported([.. adifEntries.Keys], folder, fileName, exportFilters, true);
                expectedAmounts = dbRepository.GetQsoAmountsForExport();
                handleButton(exportButton, false, $"Exported ({adifEntries.Count})");
                ButtonStyleHandler.Update(exportButton, false);
                RecalcFiltered();
            }
            else
            {
                log.Information("User chose not to set QSO(s) as exported in database");
                dbRepository.SetQSOsExported([.. adifEntries.Keys], folder, fileName, exportFilters, false);
            }

            string argument = "/select, \"" + filePath + "\"";
            Process.Start("explorer.exe", argument);
        }

        private void handleButton(Button button, bool enabled, string? newText = null)
        {
            button.Enabled = enabled;
            if (newText != null) button.Text = newText;
            ButtonStyleHandler.Update(button, enabled);
        }
    }
}
