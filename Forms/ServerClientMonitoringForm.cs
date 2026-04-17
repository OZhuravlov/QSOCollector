using QSOCollector.Models;
using System.Collections.Concurrent;

namespace QSOCollector
{
    public partial class ServerClientMonitoringForm : Form
    {
        private readonly ConcurrentDictionary<string, ClientMonitoringInfo> clientsMonitoring;
        private System.Windows.Forms.Timer refreshTimer;
        private DataGridView dataGridView;

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
            dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                BackgroundColor = SystemColors.Window,
                GridColor = SystemColors.Control
            };

            this.Controls.Add(dataGridView);
        }

        private void SetupDataGridView()
        {
            dataGridView.Columns.Clear();

            // IP Address Column
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "IpAddress",
                HeaderText = "IP Address",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleLeft }
            });

            // Status Column
            var statusColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Status",
                HeaderText = "Status",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };
            dataGridView.Columns.Add(statusColumn);

            // Connection Time Column
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ConnectionTime",
                HeaderText = "Connected At",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Format = "yyyy-MM-dd HH:mm:ss"
                }
            });

            // Last Activity Column
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "LastActivityTime",
                HeaderText = "Last Activity",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Format = "yyyy-MM-dd HH:mm:ss"
                }
            });

            // QSOs Received Column
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "QsosReceived",
                HeaderText = "QSOs Received",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dataGridView.AutoResizeColumns();
        }

        private void SetupTimer()
        {
            refreshTimer = new System.Windows.Forms.Timer
            {
                Interval = 10000 // 10 seconds
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

            var clientList = clientsMonitoring.Values
                .OrderByDescending(c => c.Status == ClientStatus.Connected)  // Connected clients first
                .ThenByDescending(c => c.LastActivityTime)  // Most recently active first
                .ToList();

            dataGridView.DataSource = null;
            dataGridView.DataSource = clientList;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            refreshTimer?.Stop();
            refreshTimer?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
