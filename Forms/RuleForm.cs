using Microsoft.Data.Sqlite;
using QSOCollector.Data;
using QSOCollector.Helpers;
using QSOCollector.Models;
using System.Globalization;

namespace QSOCollector.Forms
{
    public partial class RuleForm : Form
    {
        public bool ruleChanged = false;
        private readonly IDbRepository dbRepository;
        private SatRule? rule = null;
        private List<Band> bands = [];
        private bool isInit = false;
        private readonly bool isReadOnly;

        public RuleForm(IDbRepository dbRepository, SatRule? rule, bool isReadOnly = false)
        {
            this.dbRepository = dbRepository;
            InitializeComponent();
            this.rule = rule;
            this.isReadOnly = isReadOnly;
        }

        private void RuleForm_Load(object sender, EventArgs e)
        {
            List<string> satModes = dbRepository.GetSatModes();
            satModeTextBox.AutoCompleteCustomSource.AddRange([.. satModes]);
            bands = dbRepository.GetBands(true);

            var BandTxBindingSource = new BindingSource
            {
                DataSource = bands
            };
            bandTxComboBox.DataSource = BandTxBindingSource;
            bandTxComboBox.DisplayMember = "FullBandName";
            bandTxComboBox.ValueMember = "Id";
            bandTxComboBox.SelectedIndex = -1; // No selection by default

            var BandRxBindingSource = new BindingSource
            {
                DataSource = bands
            };
            bandRxComboBox.DataSource = BandRxBindingSource;
            bandRxComboBox.DisplayMember = "FullBandName";
            bandRxComboBox.ValueMember = "Id";
            bandRxComboBox.SelectedIndex = -1; // No selection by default

            if (rule != null)
            {
                PopulateFormFields();
            }
            isInit = true;
            if (isReadOnly)
            {
                this.Text += ": Readonly (Editable from Shared tab)";
                DisableEditableControls(this);
                saveButton.Visible = false;
                cancelButton.Visible = false;
            }
        }

        private static void DisableEditableControls(Control parentControl)
        {
            foreach (Control control in parentControl.Controls)
            {
                if (control is TextBox || control is ComboBox)
                {
                    control.BackColor = SystemColors.InactiveCaption;
                    control.Enabled = false;
                }
                DisableEditableControls(control);
            }
        }

        // Populate the form fields with the rule data
        private void PopulateFormFields()
        {
            propModeComboBox.Text = rule.PropagationMode;
            nameTextBox.Text = rule.Name;
            origFreqFrom.Text = rule.SourceFreqFrom.ToString(CultureInfo.InvariantCulture);
            origFreqTo.Text = rule.SourceFreqTo.ToString(CultureInfo.InvariantCulture);
            satNameTextBox.Text = rule.SatName;
            satModeTextBox.Text = rule.SatMode;

            if (rule.BandTx != null)
            {
                bandTxComboBox.SelectedValue = rule.BandTx.Id;
            }

            if (rule.BandRx != null)
            {
                bandRxComboBox.SelectedValue = rule.BandRx.Id;
            }

            freqRxTextBox.Text = rule.FreqRx?.ToString(CultureInfo.InvariantCulture);
            freqTxTextBox.Text = rule.FreqTx?.ToString(CultureInfo.InvariantCulture);
        }

        private void SatNameTextBox_TextChanged(object sender, EventArgs e)
        {
            SetRuleChanged();
            string satName = satNameTextBox.Text.ToUpper().Trim();
            if (satNameTextBox.Text.Length > 0 && satName == "QO-100")
            {
                satModeTextBox.Text = "SX";
            }
        }

        private void SatModeTextBox_TextChanged(object sender, EventArgs e)
        {
            SetRuleChanged();
            if (satModeTextBox.Text.Length == 0)
            {
                return;
            }

            string satMode = satModeTextBox.Text.ToUpper().Trim();

            if (satMode.Length != 2)
            {
                bandRxComboBox.SelectedIndex = -1;
                bandTxComboBox.SelectedIndex = -1;
                return;
            }

            char satBandTxDesignator = satMode[0];
            char satBandRxDesignator = satMode[1];
            if (!char.IsAsciiLetter(satBandTxDesignator) || !char.IsAsciiLetter(satBandRxDesignator))
            {
                return;
            }

            Band? selectedBandTx = bands
                .FirstOrDefault(b => !string.IsNullOrEmpty(b.Designator) && b.Designator.Length == 1 && b.Designator[0] == satBandTxDesignator);
            Band? selectedBandRx = bands
                .FirstOrDefault(b => !string.IsNullOrEmpty(b.Designator) && b.Designator.Length == 1 && b.Designator[0] == satMode[1]);
            if (selectedBandTx == null || selectedBandRx == null)
            {
                return;
            }

            bandTxComboBox.SelectedValue = selectedBandTx.Id;
            bandRxComboBox.SelectedValue = selectedBandRx.Id;
        }

        private void SatModeTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsAsciiLetterOrDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            e.KeyChar = char.ToUpper(e.KeyChar);
        }

        private void Freq_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != ',')
            {
                e.Handled = true;
            }

            if (e.KeyChar == ',')
            {
                e.KeyChar = '.';
            }

            // only allow one decimal point
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            bool isActive = rule == null || rule.IsActive;
            rule = new SatRule
            {
                Id = rule?.Id,
                IsActive = isActive
            };

            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                MessageBox.Show("Rule name cannot be empty.");
                return;
            }
            rule.Name = nameTextBox.Text.Trim();

            if (propModeComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Choose Propagation mode.");
                return;
            }
            rule.PropagationMode = propModeComboBox.Text.Trim();

            if (origFreqFrom.Text.Trim().Length == 0 || origFreqTo.Text.Trim().Length == 0)
            {
                MessageBox.Show("Source frequency 'From' and 'To' cannot be empty.");
                return;
            }

            rule.SourceFreqFrom = double.Parse(origFreqFrom.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture);
            if (rule.SourceFreqFrom <= 0)
            {
                MessageBox.Show("Source frequency 'From' must be greater than 0.");
                return;
            }
            rule.SourceFreqTo = double.Parse(origFreqTo.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture);
            if (rule.SourceFreqTo <= 0)
            {
                MessageBox.Show("Source frequency 'To' must be greater than 0.");
                return;
            }

            if (rule.SourceFreqFrom >= rule.SourceFreqTo)
            {
                MessageBox.Show("Source frequency 'From' must be less than 'To'.");
                return;
            }

            if (string.IsNullOrWhiteSpace(satNameTextBox.Text))
            {
                MessageBox.Show("Satellite name cannot be empty.");
                return;
            }
            rule.SatName = satNameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(satModeTextBox.Text))
            {
                rule.SatMode = null;
            }
            else
            {
                rule.SatMode = satModeTextBox.Text.Trim();
            }

            if (bandTxComboBox.SelectedIndex != -1)
            {
                rule.BandTx = bands.FirstOrDefault(b => b.Id == (int)bandTxComboBox.SelectedValue);
                rule.BandTxId = rule.BandTx?.Id;
            }

            if (bandRxComboBox.SelectedIndex != -1)
            {
                rule.BandRx = bands.FirstOrDefault(b => b.Id == (int)bandRxComboBox.SelectedValue);
                rule.BandRxId = rule.BandRx?.Id;
            }

            if (freqTxTextBox.Text.Trim().Length > 0)
            {
                rule.FreqTx = double.Parse(freqTxTextBox.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture);
                if (rule.BandTx != null && (rule.FreqTx < rule.BandTx.FreqFrom || rule.FreqTx > rule.BandTx.FreqTo))
                {
                    MessageBox.Show($"Transmit frequency must be within the selected transmit band ({rule.BandTx.FullBandName}) ({rule.BandTx.FreqFrom}-{rule.BandTx.FreqTo}).");
                    return;
                }
            }

            if (freqRxTextBox.Text.Trim().Length > 0)
            {
                rule.FreqRx = double.Parse(freqRxTextBox.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture);
                if (rule.BandRx != null && (rule.FreqRx < rule.BandRx.FreqFrom || rule.FreqRx > rule.BandRx.FreqTo))
                {
                    MessageBox.Show($"Receive frequency must be within the selected receive band ({rule.BandRx.FullBandName}) ({rule.BandRx.FreqFrom}-{rule.BandRx.FreqTo}).");
                    return;
                }
            }

            try
            {
                dbRepository.SaveSatRule(rule);
            }
            catch (Exception ex)
            {
                if (ex is SqliteException sqliteEx && sqliteEx.SqliteExtendedErrorCode == IDbRepository.SQLITE_CONSTRAINT_UNIQUE)
                {
                    MessageBox.Show("A rule with the same name already exists. Please choose a different name.");
                }
                else
                {
                    MessageBox.Show($"An error occurred while saving the rule: {ex.Message}");
                }
                return;
            }
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void SatNameTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = char.ToUpper(e.KeyChar);
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            SetRuleChanged();
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetRuleChanged();
        }

        private void SetRuleChanged()
        {
            if (!isInit) {
                return;
            }

            saveButton.Enabled = true;
            if (rule != null)
            {
                ruleChanged = true;
            }
        }
    }
}
