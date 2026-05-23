using QSOCollector.Models;
using System.Collections.Concurrent;

namespace QSOCollector
{
    public partial class ServerClientMonitoringForm : Form
    {
        private const int InactivityBeforeUnknownInMinutes = 1;
        private const int InactivityBeforeDisconnectedInMinutes = 5;
        private const int InactivityBeforeRemoveFromListInMinutes = 10;

        private readonly ConcurrentDictionary<string, ClientMonitoringInfo> clientsMonitoring;
        private System.Windows.Forms.Timer refreshTimer;
        private Label statusLabel;
        private DataGridView clientDataGridView;

        public ServerClientMonitoringForm(ConcurrentDictionary<string, ClientMonitoringInfo> clientsMonitoring)
        {
            this.clientsMonitoring = clientsMonitoring ?? throw new ArgumentNullException(nameof(clientsMonitoring));
            InitializeComponent();
            SetupDataGridView();
            SetupTimer();
        }

        private void InitializeComponent()
        {
            clientDataGridView = new DataGridView();
            statusLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)clientDataGridView).BeginInit();
            SuspendLayout();
            // 
            // clientDataGridView
            // 
            clientDataGridView.AllowUserToAddRows = false;
            clientDataGridView.AllowUserToDeleteRows = false;
            clientDataGridView.AllowUserToResizeColumns = false;
            clientDataGridView.AllowUserToResizeRows = false;
            clientDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            clientDataGridView.Location = new Point(4, 4);
            clientDataGridView.Name = "clientDataGridView";
            clientDataGridView.ReadOnly = true;
            clientDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            clientDataGridView.Size = new Size(975, 394);
            clientDataGridView.TabIndex = 0;
            // 
            // statusLabel
            // 
            statusLabel.AutoSize = true;
            statusLabel.Location = new Point(12, 417);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(0, 15);
            statusLabel.TabIndex = 1;
            // 
            // ServerClientMonitoringForm
            // 
            ClientSize = new Size(980, 456);
            Controls.Add(statusLabel);
            Controls.Add(clientDataGridView);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Name = "ServerClientMonitoringForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Server Client Monitoring";
            ((System.ComponentModel.ISupportInitialize)clientDataGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private void SetupDataGridView()
        {
            clientDataGridView.Columns.Clear();
            // IP Address Column
            AddDataGridViewColumn("IP Address", "IpAddress", 180);
            // Status Column
            AddDataGridViewColumn("Status", "Status", 140);
            // Connection Time Column
            AddDataGridViewColumn("Connected At", "ConnectionTime", 200, "yyyy-MM-dd HH:mm:ss");
            // Last Activity Column
            AddDataGridViewColumn("Last Activity", "LastActivityTime", 200, "yyyy-MM-dd HH:mm:ss");
            // QSOs Received Column
            AddDataGridViewColumn("QSOs Received", "QsosReceived", 120);

            // Center align headers
            foreach (DataGridViewColumn column in clientDataGridView.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
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
            clientDataGridView.Columns.Add(column);
        }

        private void SetupTimer()
        {
            refreshTimer = new System.Windows.Forms.Timer
            {
                Interval = 10000
            };
            refreshTimer.Tick += (s, e) => RefreshClientList();
            refreshTimer.Start();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            RefreshClientList();
        }

        private void RefreshClientList()
        {
            if (InvokeRequired)
            {
                BeginInvoke(RefreshClientList);
                return;
            }

            UpdateClientStatusByActivityTimeout();

            var clientList = clientsMonitoring.Values
                .OrderByDescending(c => c.IpAddress)
                .ToList();

            int totalCount = clientList.Count;
            int connectedCount = clientList.Count(c => c.Status == ClientStatus.Connected);
            int disconnectedCount = clientList.Count(c => c.Status == ClientStatus.Disconnected);
            int unknownCount = clientList.Count(c => c.Status == ClientStatus.Unknown);

            statusLabel.Text = $"Total Clients: {totalCount} | Connected: {connectedCount} | Disconnected: {disconnectedCount} | Unknown: {unknownCount}. Last refreshed at {DateTime.UtcNow:dd-MMM HH:mm:ss}Z";

            clientDataGridView.DataSource = null;
            SetupDataGridView();
            clientDataGridView.DataSource = clientList;
        }

        private void UpdateClientStatusByActivityTimeout()
        {
            var now = DateTime.UtcNow;
            
            var inactivityRemoveThresholdTime = now.AddMinutes(-InactivityBeforeRemoveFromListInMinutes);
            clientsMonitoring.Where(kvp => kvp.Value.LastActivityTime < inactivityRemoveThresholdTime)
                .Select(kvp => kvp.Key)
                .ToList()
                .ForEach(ip => clientsMonitoring.TryRemove(ip, out _));

            var inactivityDisconnectedThresholdTime = now.AddMinutes(-InactivityBeforeDisconnectedInMinutes);
            var inactivityUnknownThresholdTime = now.AddMinutes(-InactivityBeforeUnknownInMinutes);

            foreach (var clientInfo in clientsMonitoring.Values)
            {
                var lastActivityTime = clientInfo.LastActivityTime;
                if (clientInfo.Status != ClientStatus.Disconnected && lastActivityTime < inactivityDisconnectedThresholdTime)
                {
                    clientInfo.Status = ClientStatus.Disconnected;
                    continue;
                }

                if (clientInfo.Status == ClientStatus.Connected && 
                    clientInfo.LastActivityTime < inactivityUnknownThresholdTime)
                {
                    clientInfo.Status = ClientStatus.Unknown;
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            refreshTimer?.Stop();
            refreshTimer?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
