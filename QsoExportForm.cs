using DbUp;

namespace QSOCollector
{
    public partial class QsoExportForm : Form
    {
        private readonly DbRepository dbRepository;
        private readonly List<QsoExportExpectedAmounts> expectedAmounts;
        private readonly QsoExportFilters exportFilters = new();

        public QsoExportForm(DbRepository dbRepository)
        {
            this.dbRepository = dbRepository;
            expectedAmounts = dbRepository.GetQsoAmountsForExport();
            InitializeComponent();
            InitValues();
        }

        private void InitValues()
        {
            totalAmountLabel.Text = expectedAmounts.Sum(r => r.Count).ToString();
            newQSOsRadioButton.Checked = true;
            exportFilters.IsNewOnly = true;
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
            ResetSecondaryFilter(modeGroupComboBox, GetFilteredAmounts(exportFilters).Select(r => r.ModeGroup).Distinct().OrderBy(r => r));
            ResetSecondaryFilter(modeComboBox, GetFilteredAmounts(exportFilters).Select(r => r.Mode).Distinct().OrderBy(r => r));
            ResetSecondaryFilter(programIdComboBox, GetFilteredAmounts(exportFilters).Select(r => r.ProgramId).Distinct().OrderBy(r => r == "<UNKNOWN>").ThenBy(r => r));
            ResetSecondaryFilter(bandComboBox, GetFilteredAmounts(exportFilters).Select(r => r.Band).Distinct().OrderByDescending(r => Int32.Parse(r.TrimEnd('M'))));
            ResetSecondaryFilter(operatorComboBox, GetFilteredAmounts(exportFilters).Select(r => r.Operator).Distinct().OrderBy(r => r == "<UNKNOWN>").ThenBy(r => r));
            ResetSecondaryFilter(sourceIpComboBox, GetFilteredAmounts(exportFilters).Select(r => r.Operator).Distinct().OrderBy(r => r == "<UNKNOWN>").ThenBy(r => r));
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
            if (currIndex != defaultIndex) {
                for (int i = 0; i < comboBox.Items.Count; i++)
                {
                    if (comboBox.Items[i].ToString() == currValue) { 
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

        private void dateFromTimePicker_CheckedChanged(object sender, EventArgs e)
        {
            if (dateFromTimePicker.Checked)
            {
                SetDateFromPicker();
            }
            else
            {
                ResetDateFromPicker();
            }
            RecalcFiltered();
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
        private void dateToTimePicker_CheckedChanged(object sender, EventArgs e)
        {
            if (dateToTimePicker.Checked)
            {
                SetDateToPicker();
            }
            else
            {
                ResetDateToPicker();
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
                .Where(r => filters.ProgramId == null || r.ProgramId == filters.ProgramId)
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

        private void programIdComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            exportFilters.ProgramId = GetComboBoxValue(programIdComboBox);
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

        private string? GetComboBoxValue(ComboBox comboBox) { 
            return comboBox.SelectedIndex == -1 || string.IsNullOrWhiteSpace(comboBox.Text) 
                ? null : comboBox.Text;
        }
    }
}
