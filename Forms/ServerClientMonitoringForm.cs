using QSOCollector.Models;
using System.Collections.Concurrent;

namespace QSOCollector
{
    public partial class ServerClientMonitoringForm : Form
    {
        private const int InactivityTimeoutMinutes = 5;

        private readonly ConcurrentDictionary<string, ClientMonitoringInfo> clientsMonitoring;
        private System.Windows.Forms.Timer refreshTimer;
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
            this.Text = "Server Client Monitoring";
            this.Size = new Size(900, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;

            // Create DataGridView
            clientDataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AllowUserToOrderColumns = true,
                RowHeadersVisible = false,
                BackgroundColor = SystemColors.Window,
                GridColor = SystemColors.Control
            };

            this.Controls.Add(clientDataGridView);
        }

        private void SetupDataGridView()
        {
            clientDataGridView.Columns.Clear();

            // IP Address Column
            clientDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "IpAddress",
                HeaderText = "IP Address",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Width = 140,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            // Status Column
            var statusColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Status",
                HeaderText = "Status",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };
            clientDataGridView.Columns.Add(statusColumn);

            // Connection Time Column
            clientDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ConnectionTime",
                HeaderText = "Connected At",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Width = 170,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Format = "yyyy-MM-dd HH:mm:ss"
                }
            });

            // Last Activity Column
            clientDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "LastActivityTime",
                HeaderText = "Last Activity",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Width = 170,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Format = "yyyy-MM-dd HH:mm:ss"
                }
            });

            // QSOs Received Column
            clientDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "QsosReceived",
                HeaderText = "QSOs Received",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            // Center align all column headers
            foreach (DataGridViewColumn column in clientDataGridView.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
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
                .OrderByDescending(c => c.Status == ClientStatus.Connected)
                .ThenByDescending(c => c.LastActivityTime)
                .ToList();

            clientDataGridView.DataSource = null;
            clientDataGridView.DataSource = clientList;
        }

        private void UpdateClientStatusByActivityTimeout()
        {
            var timeoutThreshold = DateTime.UtcNow.AddMinutes(-InactivityTimeoutMinutes);

            foreach (var clientInfo in clientsMonitoring.Values)
            {
                if (clientInfo.Status == ClientStatus.Connected && 
                    clientInfo.LastActivityTime < timeoutThreshold)
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
