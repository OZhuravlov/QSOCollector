using Microsoft.Data.Sqlite;
using QSOCollector.Data;
using QSOCollector.Models;
using Serilog;
using System.Data;
using System.Data.SQLite;
using System.Globalization;

namespace QSOCollector.Forms
{
    public partial class AssignRuleForm : Form
    {
        public bool ruleChanged = false;

        private readonly ILogger log = Log.ForContext<QsoCollectorForm>();

        private readonly IDbRepository dbRepository;
        private readonly int listenerId;
        private readonly DataTable ruleDataTable = new();

        public AssignRuleForm(IDbRepository dbRepository, int listenerId)
        {
            InitializeComponent();
            this.dbRepository = dbRepository;
            this.listenerId = listenerId;
        }

        private void AssignRuleForm_Load(object sender, EventArgs e)
        {
            PopulateRuleDataGridView();
        }

        private void PopulateRuleDataGridView()
        {
            string selectCommand = "select sr.id, sr.name, sr.orig_freq_mhz_from, sr.orig_freq_mhz_to, sr.propagation_mode, sr.sat_name, sr.sat_mode, bt.name band, br.name band_rx, sr.freq_mhz_rx, sr.freq_mhz, sr.is_active from sat_rules sr left join bands bt on sr.band_id = bt.id left join bands br on sr.band_rx_id = br.id where sr.id not in (select lsr.rule_id from listener_sat_rules lsr where lsr.listener_id = @listenerId)";
            try
            {
                Log.Debug("Populate Listener Rules");
                ruleDataAdapter = new SQLiteDataAdapter(selectCommand, dbRepository.GetConnectionString());
                ruleDataAdapter.SelectCommand.Parameters.AddWithValue("@listenerId", listenerId);
                SQLiteCommandBuilder commandBuilder = new(ruleDataAdapter);
                ruleDataAdapter.Fill(ruleDataTable);
                ruleDataTable.PrimaryKey = [ruleDataTable.Columns["id"]];
                assignRuleDataGridView.DataSource = ruleBindingSource;
                ruleBindingSource.DataSource = ruleDataTable;
            }
            catch (SqliteException ex)
            {
                string message = "Can't retrieve data from DB";
                log.Error(ex, message);
                MessageBox.Show($"{message}: {ex.Message}");
            }
        }

        private void AssignRuleDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            AssignRuleButton.Enabled = assignRuleDataGridView.SelectedRows.Count == 1;
            if (!AssignRuleButton.Enabled)
            {
                assignRuleHintLabel.Text = "(Select one Rule row to assign)";
            }
            else
            {
                assignRuleHintLabel.Text = "";
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void AssignRuleButton_Click(object sender, EventArgs e)
        {
            DataGridViewRow selectedRow = assignRuleDataGridView.SelectedRows[0];
            if (!Convert.ToBoolean(selectedRow.Cells["isActive"].Value))
            {
                DialogResult result = MessageBox.Show("Selected rule is not active. You can enable it later in 'Shared' section. Do you want to assign it anyway?", "Confirm Assignment", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    return;
                }
                ruleChanged = true;
            }

            int ruleId = Convert.ToInt32(selectedRow.Cells["id"].Value);
            double origFreqFrom = double.Parse(selectedRow.Cells["origFreqFrom"].Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture);
            double origFreqTo = double.Parse(selectedRow.Cells["origFreqTo"].Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture);

            SatRule? overLappingRule = dbRepository.GetListenerSatRules(listenerId)
                .FirstOrDefault(rule => origFreqFrom <= rule.SourceFreqTo && origFreqTo >= rule.SourceFreqFrom);
            if (overLappingRule != null)
            {
                MessageBox.Show($"Selected rule frequency range ({origFreqFrom} - {origFreqTo}) overlaps with existing assigned rule '{overLappingRule.Name}' frequency range ({overLappingRule.SourceFreqFrom} - {overLappingRule.SourceFreqTo}). Please select a different rule.", "Frequency Overlap", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            dbRepository.AssignRuleToListener(listenerId, ruleId);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
