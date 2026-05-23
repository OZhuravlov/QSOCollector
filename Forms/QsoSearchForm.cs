using QSOCollector.Data;
using Serilog;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace QSOCollector.Forms
{
    public partial class QsoSearchForm : Form
    {
        private readonly ILogger log = Log.ForContext<QsoSearchForm>();
        private readonly IDbRepository dbRepository;
        private DataTable searchResultsDataTable;
        private string currentSortColumn = "qso_time";
        private bool isAscending = false; // false = DESC, true = ASC
        private List<Dictionary<string, object?>> lastSearchResults = new();

        public QsoSearchForm(IDbRepository dbRepository)
        {
            this.dbRepository = dbRepository ?? throw new ArgumentNullException(nameof(dbRepository));
            InitializeComponent();
            searchResultsDataTable = new()
            {
                Locale = CultureInfo.InvariantCulture
            };
            InitializeDataGridView();
            LoadFilterValues();
            callSearchTextBox.Focus();
        }

        private void InitializeDataGridView()
        {
            qsoSearchDataGridView.AutoGenerateColumns = false;
            qsoSearchDataGridView.AllowUserToAddRows = false;
            qsoSearchDataGridView.AllowUserToDeleteRows = false;
            qsoSearchDataGridView.ReadOnly = true;
            qsoSearchDataGridView.MultiSelect = false;
            qsoSearchDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            qsoSearchDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;

            // Create columns
            AddDataGridViewColumn("Callsign", "call", 200);
            AddDataGridViewColumn("QSO time", "qso_time", 200, "yyyy-MM-dd HH:mm:ss");
            AddDataGridViewColumn("Mode group", "mode_group", 100);
            AddDataGridViewColumn("Mode", "mode", 100);
            AddDataGridViewColumn("Band", "band", 60);
            AddDataGridViewColumn("Frequency", "freq", 120);
            AddDataGridViewColumn("Operator", "operator", 120);
            AddDataGridViewColumn("Source IP", "source_ip_address", 160);

            // Center align headers
            foreach (DataGridViewColumn column in qsoSearchDataGridView.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            qsoSearchDataGridView.DataSource = searchResultsDataTable;
            qsoSearchDataGridView.ColumnHeaderMouseClick += QsoSearchDataGridView_ColumnHeaderMouseClick;
        }

        private void AddDataGridViewColumn(string headerText, string dataPropertyName, int width, string? format = null)
        {
            var column = new DataGridViewTextBoxColumn
            {
                HeaderText = headerText,
                DataPropertyName = dataPropertyName,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Width = width,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };
            if (format != null)
            {
                column.DefaultCellStyle.Format = format;
            }
            qsoSearchDataGridView.Columns.Add(column);
        }

        private void LoadFilterValues()
        {
            try
            {
                var modeGroups = dbRepository.GetDistinctModeGroups();
                modeGroupComboBox.Items.Clear();
                modeGroupComboBox.Items.Add(""); // Empty option for all
                foreach (var modeGroup in modeGroups)
                {
                    modeGroupComboBox.Items.Add(modeGroup);
                }
                modeGroupComboBox.SelectedIndex = 0;

                var bands = dbRepository.GetDistinctBands();
                bandComboBox.Items.Clear();
                bandComboBox.Items.Add(""); // Empty option for all
                foreach (var band in bands)
                {
                    bandComboBox.Items.Add(band);
                }
                bandComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error loading filter values");
                MessageBox.Show("Error loading filter values", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void callSearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            // Allow search on Enter key
            if (e.KeyCode == Keys.Return && searchButton.Enabled)
            {
                searchButton_Click(sender, EventArgs.Empty);
            }
        }

        private void callSearchTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Validate and convert to uppercase
            if (!char.IsControl(e.KeyChar))
            {
                // Only allow A-Za-z0-9/_%$
                if (!Regex.IsMatch(e.KeyChar.ToString(), @"[A-Za-z0-9\/%_$]"))
                {
                    e.Handled = true;
                    return;
                }

                // Convert lowercase to uppercase
                if (char.IsLower(e.KeyChar))
                {
                    e.KeyChar = char.ToUpper(e.KeyChar);
                }
            }

            // Check if search button should be enabled (at least 3 [A-Z0-9] characters)
            UpdateSearchButtonState();
        }

        private void UpdateSearchButtonState()
        {
            // Count valid [A-Z0-9] characters
            int validCharCount = Regex.Count(callSearchTextBox.Text, @"[A-Z0-9]");
            searchButton.Enabled = validCharCount >= 3;
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            try
            {
                string searchPattern = callSearchTextBox.Text.Trim();

                // Validate input: only allow A-Za-z0-9/_%$ (should already be uppercase and valid)
                if (!Regex.IsMatch(searchPattern, @"^[A-Z0-9\/%_$]*$"))
                {
                    MessageBox.Show("Invalid characters. Only A-Z, 0-9, /, %, _, $ are allowed.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    callSearchTextBox.Focus();
                    return;
                }

                // Build LIKE pattern with % wildcards if not already specified
                if (!searchPattern.Contains('%') && !searchPattern.Contains('_'))
                {
                    searchPattern = $"%{searchPattern}%";
                }

                string? modeGroup = string.IsNullOrWhiteSpace(modeGroupComboBox.SelectedItem?.ToString()) ? null : modeGroupComboBox.SelectedItem.ToString();
                string? band = string.IsNullOrWhiteSpace(bandComboBox.SelectedItem?.ToString()) ? null : bandComboBox.SelectedItem.ToString();

                log.Debug("Searching QSOs: pattern={pattern}, modeGroup={modeGroup}, band={band}", searchPattern, modeGroup, band);

                var results = dbRepository.SearchQsosByCall(searchPattern, modeGroup, band, 200);

                // Store original results for filter dropdown population
                lastSearchResults = [.. results];

                // Populate DataTable
                searchResultsDataTable.Clear();
                searchResultsDataTable.Columns.Clear();

                // Define columns
                searchResultsDataTable.Columns.Add("call", typeof(string));
                searchResultsDataTable.Columns.Add("qso_time", typeof(DateTime));
                searchResultsDataTable.Columns.Add("mode_group", typeof(string));
                searchResultsDataTable.Columns.Add("mode", typeof(string));
                searchResultsDataTable.Columns.Add("band", typeof(string));
                searchResultsDataTable.Columns.Add("freq", typeof(string));
                searchResultsDataTable.Columns.Add("operator", typeof(string));
                searchResultsDataTable.Columns.Add("source_ip_address", typeof(string));

                // Add rows
                foreach (var result in results)
                {
                    var row = searchResultsDataTable.NewRow();
                    row["call"] = result["call"] ?? string.Empty;
                    row["qso_time"] = ParseDateTime(result["qso_time"]);
                    row["mode_group"] = result["mode_group"] ?? string.Empty;
                    row["mode"] = result["mode"] ?? string.Empty;
                    row["band"] = result["band"] ?? string.Empty;
                    row["freq"] = result["freq"] ?? string.Empty;
                    row["operator"] = result["operator"] ?? string.Empty;
                    row["source_ip_address"] = result["source_ip_address"] ?? string.Empty;
                    searchResultsDataTable.Rows.Add(row);
                }

                // Refresh DataGridView binding
                qsoSearchDataGridView.DataSource = null;
                qsoSearchDataGridView.DataSource = searchResultsDataTable;

                // Re-center align headers
                foreach (DataGridViewColumn column in qsoSearchDataGridView.Columns)
                {
                    column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                // Update filter dropdowns based on original search results
                UpdateFilterValues();

                log.Information("Search completed: {count} results found", results.Count);
                UpdateStatusLabel(results.Count);
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error during search");
                MessageBox.Show($"Error during search: {ex.Message}", "Search Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DateTime ParseDateTime(object? value)
        {
            if (value == null || value == DBNull.Value) return DateTime.MinValue;

            if (value is DateTime dt) return dt;
            if (value is string str && DateTime.TryParse(str, out dt)) return dt;

            return DateTime.MinValue;
        }

        private void UpdateFilterValues()
        {
            try
            {
                // Unsubscribe from events to prevent recursion during item updates
                modeGroupComboBox.SelectedIndexChanged -= modeGroupComboBox_SelectedIndexChanged;
                bandComboBox.SelectedIndexChanged -= bandComboBox_SelectedIndexChanged;

                try
                {
                    // Update Mode Group dropdown using original search results
                    var modeGroupValues = lastSearchResults
                        .Select(r => r["mode_group"]?.ToString())
                        .Where(v => !string.IsNullOrEmpty(v))
                        .Distinct()
                        .OrderBy(v => v)
                        .ToList();

                    string currentModeGroup = modeGroupComboBox.SelectedItem?.ToString() ?? "";
                    modeGroupComboBox.Items.Clear();
                    modeGroupComboBox.Items.Add("");
                    foreach (var mg in modeGroupValues)
                    {
                        modeGroupComboBox.Items.Add(mg);
                    }

                    // Restore previous selection if it still exists
                    int modeGroupIndex = modeGroupComboBox.Items.IndexOf(currentModeGroup);
                    modeGroupComboBox.SelectedIndex = modeGroupIndex >= 0 ? modeGroupIndex : 0;

                    // Update Band dropdown using original search results
                    var bandValues = lastSearchResults
                        .Select(r => r["band"]?.ToString())
                        .Where(v => !string.IsNullOrEmpty(v))
                        .Distinct()
                        .OrderBy(v => v)
                        .ToList();

                    string currentBand = bandComboBox.SelectedItem?.ToString() ?? "";
                    bandComboBox.Items.Clear();
                    bandComboBox.Items.Add("");
                    foreach (var b in bandValues)
                    {
                        bandComboBox.Items.Add(b);
                    }

                    // Restore previous selection if it still exists
                    int bandIndex = bandComboBox.Items.IndexOf(currentBand);
                    bandComboBox.SelectedIndex = bandIndex >= 0 ? bandIndex : 0;
                }
                finally
                {
                    // Re-subscribe to events
                    modeGroupComboBox.SelectedIndexChanged += modeGroupComboBox_SelectedIndexChanged;
                    bandComboBox.SelectedIndexChanged += bandComboBox_SelectedIndexChanged;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error updating filter values");
            }
        }

        private string BuildFilterConditionsDisplay()
        {
            var conditions = new List<string>();

            if (!string.IsNullOrWhiteSpace(callSearchTextBox.Text))
            {
                string callSearchOperator = callSearchTextBox.Text.Contains('%') || callSearchTextBox.Text.Contains('_') ? "like" : "contains";
                conditions.Add($"Callsign {callSearchOperator} '{callSearchTextBox.Text.Trim()}'");
            }

            string? modeGroup = string.IsNullOrWhiteSpace(modeGroupComboBox.SelectedItem?.ToString())
                ? null
                : modeGroupComboBox.SelectedItem.ToString();
            if (!string.IsNullOrWhiteSpace(modeGroup))
            {
                conditions.Add($"Mode group = '{modeGroup}'");
            }

            string? band = string.IsNullOrWhiteSpace(bandComboBox.SelectedItem?.ToString())
                ? null
                : bandComboBox.SelectedItem.ToString();
            if (!string.IsNullOrWhiteSpace(band))
            {
                conditions.Add($"Band = '{band}'");
            }

            string resultText = $"{searchResultsDataTable.Rows.Count} results found";
            if (conditions.Count > 0)
            {
                resultText += $" with condition(s): {string.Join(", ", conditions)}";
            }

            return resultText;
        }

        private void UpdateStatusLabel(int totalCount)
        {
            if (totalCount == 200)
            {
                statusLabel.Text = "200+ results (limited to 200)";
            }
            else
            {
                statusLabel.Text = BuildFilterConditionsDisplay();
            }
        }

        private void QsoSearchDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                string columnName = qsoSearchDataGridView.Columns[e.ColumnIndex].DataPropertyName;

                // Toggle sort direction if same column is clicked
                if (columnName == currentSortColumn)
                {
                    isAscending = !isAscending;
                }
                else
                {
                    currentSortColumn = columnName;
                    isAscending = false; // Default to descending for new column
                }

                // Sort the DataTable
                var view = searchResultsDataTable.DefaultView;
                view.Sort = $"{currentSortColumn} {(isAscending ? "ASC" : "DESC")}";
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error sorting data grid");
            }
        }

        private void modeGroupComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (searchResultsDataTable.Rows.Count > 0)
            {
                // Trigger new search with current filters
                if (searchButton.Enabled)
                {
                    searchButton_Click(this, EventArgs.Empty);
                }
                else
                {
                    UpdateStatusLabel(lastSearchResults.Count);
                }
            }
        }

        private void bandComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (searchResultsDataTable.Rows.Count > 0)
            {
                // Trigger new search with current filters
                if (searchButton.Enabled)
                {
                    searchButton_Click(this, EventArgs.Empty);
                }
                else
                {
                    UpdateStatusLabel(lastSearchResults.Count);
                }
            }
        }
    }
}
