using Microsoft.Data.Sqlite;
using QSOCollector.Data;
using Serilog;
using System.Data;
using System.Data.SQLite;
using System.Globalization;

namespace QSOCollector.Forms
{
    public partial class ListenerRulesForm : Form
    {
        public bool assignChanged = false;

        private readonly ILogger log = Log.ForContext<QsoCollectorForm>();

        private readonly IDbRepository dbRepository;
        private readonly int listenerId;

        public ListenerRulesForm(IDbRepository dbRepository, int listenerId)
        {
            InitializeComponent();
            this.dbRepository = dbRepository;
            this.listenerId = listenerId;
        }

        private void ListenerRulesForm_Load(object sender, EventArgs e)
        {
            listenerRuleDataGridView.DataSource = listenerRuleBindingSource;
            PopulateListenerRuleDataGridView();
        }

        private void PopulateListenerRuleDataGridView()
        {
            string selectCommand = "select sr.id, sr.name, sr.orig_freq_mhz_from, sr.orig_freq_mhz_to, sr.propagation_mode, sr.sat_name, sr.sat_mode, bt.name band, br.name band_rx, sr.freq_mhz_rx, sr.freq_mhz, sr.is_active from listener_sat_rules lsr join sat_rules sr on lsr.rule_id = sr.id left join bands bt on sr.band_id = bt.id left join bands br on sr.band_rx_id = br.id where lsr.listener_id = @listenerId";
            try
            {
                Log.Debug("Populate Listener Rules");
                listenerRuleDataAdapter = new SQLiteDataAdapter(selectCommand, dbRepository.GetConnectionString());
                listenerRuleDataAdapter.SelectCommand.Parameters.AddWithValue("@listenerId", listenerId);
                // Populate a new data table and bind it to the BindingSource.
                DataTable listenerRuleDataTable = new()
                {
                    Locale = CultureInfo.InvariantCulture
                };
                listenerRuleDataAdapter.Fill(listenerRuleDataTable);
                listenerRuleBindingSource.DataSource = listenerRuleDataTable;
            }
            catch (SqliteException ex)
            {
                string message = "Can't retrieve data from DB";
                log.Error(ex, message);
                MessageBox.Show($"{message}: {ex.Message}");
            }
        }

        private void ListenerRuleDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            unassignRulesButton.Enabled = listenerRuleDataGridView.SelectedRows.Count > 0;
            unassignRuleHintLabel.Text = unassignRulesButton.Enabled ? "" : "(Select one Rule row to unassign)";
        }

        private void UnassignRulesButton_Click(object sender, EventArgs e)
        {
            List<int> selectedRuleIds = [.. listenerRuleDataGridView.SelectedRows.Cast<DataGridViewRow>()
                .Select(row => Convert.ToInt32(row.Cells["id"].Value))];
            dbRepository.RemoveRulesFromListener(listenerId, selectedRuleIds);
            assignChanged = true;
            PopulateListenerRuleDataGridView();
        }

        private void AssignRuleButton_Click(object sender, EventArgs e)
        {
            var result = new AssignRuleForm(dbRepository, listenerId).ShowDialog();
            if (result == DialogResult.OK)
            {
                assignChanged = true;
                PopulateListenerRuleDataGridView();
            }
        }
    }
}
