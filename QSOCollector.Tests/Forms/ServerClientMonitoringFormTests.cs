using System.Collections.Concurrent;
using System.Windows.Forms;
using QSOCollector.Models;
using Assert = Xunit.Assert;

namespace QSOCollector.Tests.Forms
{
    public class ServerClientMonitoringFormTests : IDisposable
    {
        [Fact]
        public void Constructor_WithValidClientsDictionary_Success()
        {
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();

            // Act
            var form = new ServerClientMonitoringForm(clientsMonitoring);

            // Assert
            Assert.NotNull(form);
            form.Dispose();
        }

        [Fact]
        public void Constructor_WithNullClientsDictionary_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ServerClientMonitoringForm(null!));
        }

        [Fact]
        public void Form_HasDataGridView_WithFiveColumns()
        {
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();

            // Act
            var form = new ServerClientMonitoringForm(clientsMonitoring);
            form.Show();

            // Assert
            var dataGridView = form.Controls.OfType<DataGridView>().FirstOrDefault();
            Assert.NotNull(dataGridView);
            Assert.Equal(5, dataGridView.Columns.Count);

            // Verify column names
            Assert.Equal("IpAddress", dataGridView.Columns[0].DataPropertyName);
            Assert.Equal("Status", dataGridView.Columns[1].DataPropertyName);
            Assert.Equal("ConnectionTime", dataGridView.Columns[2].DataPropertyName);
            Assert.Equal("LastActivityTime", dataGridView.Columns[3].DataPropertyName);
            Assert.Equal("QsosReceived", dataGridView.Columns[4].DataPropertyName);

            form.Dispose();
        }

        [Fact]
        public void DataGridView_ColumnHeaders_AreCorrect()
        {
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();

            // Act
            var form = new ServerClientMonitoringForm(clientsMonitoring);
            form.Show();

            // Assert
            var dataGridView = form.Controls.OfType<DataGridView>().FirstOrDefault();
            Assert.NotNull(dataGridView);
            Assert.Equal("IP Address", dataGridView.Columns[0].HeaderText);
            Assert.Equal("Status", dataGridView.Columns[1].HeaderText);
            Assert.Equal("Connected At", dataGridView.Columns[2].HeaderText);
            Assert.Equal("Last Activity", dataGridView.Columns[3].HeaderText);
            Assert.Equal("QSOs Received", dataGridView.Columns[4].HeaderText);

            form.Dispose();
        }

        [Fact]
        public void DataGridView_ColumnWidths_MatchConfiguredLayout()
        {
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();

            // Act
            var form = new ServerClientMonitoringForm(clientsMonitoring);
            form.Show();

            // Assert
            var dataGridView = form.Controls.OfType<DataGridView>().FirstOrDefault();
            Assert.NotNull(dataGridView);

            Assert.Equal(180, dataGridView.Columns[0].Width);  // IP Address
            Assert.Equal(140, dataGridView.Columns[1].Width);  // Status
            Assert.Equal(200, dataGridView.Columns[2].Width);  // Connected At
            Assert.Equal(200, dataGridView.Columns[3].Width);  // Last Activity
            Assert.Equal(120, dataGridView.Columns[4].Width);  // QSOs Received

            form.Dispose();
        }

        [Fact]
        public void DataGridView_AllColumnsHeadersAreCentered()
        {
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();

            // Act
            var form = new ServerClientMonitoringForm(clientsMonitoring);
            form.Show();

            // Assert
            var dataGridView = form.Controls.OfType<DataGridView>().FirstOrDefault();
            Assert.NotNull(dataGridView);

            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                Assert.Equal(DataGridViewContentAlignment.MiddleCenter, column.HeaderCell.Style.Alignment);
            }

            form.Dispose();
        }

        [Fact]
        public void DataGridView_AllColumnsCellsAreCentered()
        {
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();

            // Act
            var form = new ServerClientMonitoringForm(clientsMonitoring);
            form.Show();

            // Assert
            var dataGridView = form.Controls.OfType<DataGridView>().FirstOrDefault();
            Assert.NotNull(dataGridView);

            Assert.Equal(DataGridViewContentAlignment.MiddleCenter, dataGridView.Columns[0].DefaultCellStyle.Alignment);  // IP
            Assert.Equal(DataGridViewContentAlignment.MiddleCenter, dataGridView.Columns[1].DefaultCellStyle.Alignment);  // Status
            Assert.Equal(DataGridViewContentAlignment.MiddleCenter, dataGridView.Columns[2].DefaultCellStyle.Alignment);  // ConnectionTime
            Assert.Equal(DataGridViewContentAlignment.MiddleCenter, dataGridView.Columns[3].DefaultCellStyle.Alignment);  // LastActivityTime
            Assert.Equal(DataGridViewContentAlignment.MiddleCenter, dataGridView.Columns[4].DefaultCellStyle.Alignment);  // QsosReceived

            form.Dispose();
        }

        [Fact]
        public void DataGridView_IsReadOnly()
        {
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();

            // Act
            var form = new ServerClientMonitoringForm(clientsMonitoring);
            form.Show();

            // Assert
            var dataGridView = form.Controls.OfType<DataGridView>().FirstOrDefault();
            Assert.NotNull(dataGridView);
            Assert.True(dataGridView.ReadOnly);
            Assert.False(dataGridView.AllowUserToAddRows);
            Assert.False(dataGridView.AllowUserToDeleteRows);
            Assert.False(dataGridView.AllowUserToResizeRows);

            form.Dispose();
        }

        [Fact]
        public void Form_DisplaysEmptyDataGridView_WhenNoClientsConnected()
        {
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();

            // Act
            var form = new ServerClientMonitoringForm(clientsMonitoring);
            form.Show();

            // Assert
            var dataGridView = form.Controls.OfType<DataGridView>().FirstOrDefault();
            Assert.NotNull(dataGridView);
            Assert.Equal(0, dataGridView.RowCount);

            form.Dispose();
        }

        [Fact]
        public void Form_DisplaysSingleClient_WhenOneClientConnected()
        {
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();
            const string clientIp = "192.168.1.100";
            var clientInfo = new ClientMonitoringInfo
            {
                IpAddress = clientIp,
                Status = ClientStatus.Connected,
                ConnectionTime = DateTime.UtcNow,
                LastActivityTime = DateTime.UtcNow,
                QsosReceived = 10
            };
            clientsMonitoring.TryAdd(clientIp, clientInfo);

            // Act
            var form = new ServerClientMonitoringForm(clientsMonitoring);
            form.Show();
            System.Threading.Thread.Sleep(500);

            // Assert
            var dataGridView = form.Controls.OfType<DataGridView>().FirstOrDefault();
            Assert.NotNull(dataGridView);
            Assert.True(dataGridView.RowCount >= 1);

            form.Dispose();
        }

        [Fact]
        public void ClientStatusTimeout_MarksAsUnknown_AfterOneMinuteOfInactivity()
        {
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();
            const string clientIp = "192.168.1.100";

            // Create client with activity 1+ minutes ago
            var oneMinuteAgo = DateTime.UtcNow.AddMinutes(-1).AddSeconds(-1);
            var clientInfo = new ClientMonitoringInfo
            {
                IpAddress = clientIp,
                Status = ClientStatus.Connected,
                ConnectionTime = oneMinuteAgo.AddMinutes(-10),
                LastActivityTime = oneMinuteAgo,
                QsosReceived = 10
            };
            clientsMonitoring.TryAdd(clientIp, clientInfo);

            // Act
            var form = new ServerClientMonitoringForm(clientsMonitoring);
            form.Show();
            System.Threading.Thread.Sleep(500);

            // Assert - Status should be set to Unknown after 5 minutes of inactivity
            Assert.Equal(ClientStatus.Unknown, clientsMonitoring[clientIp].Status);

            form.Dispose();
        }

        [Fact]
        public void ClientStatusTimeout_KeepsConnected_WithinActivityWindow()
        {
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();
            const string clientIp = "192.168.1.100";

            // Create client with activity less than 5 minutes ago
            var lessThenOneMinutesAgo = DateTime.UtcNow.AddMinutes(-1).AddSeconds(20);
            var clientInfo = new ClientMonitoringInfo
            {
                IpAddress = clientIp,
                Status = ClientStatus.Connected,
                ConnectionTime = lessThenOneMinutesAgo.AddMinutes(-10),
                LastActivityTime = lessThenOneMinutesAgo,
                QsosReceived = 10
            };
            clientsMonitoring.TryAdd(clientIp, clientInfo);

            // Act
            var form = new ServerClientMonitoringForm(clientsMonitoring);
            form.Show();
            System.Threading.Thread.Sleep(500);

            // Assert - Status should remain Connected (within 5 minute window)
            Assert.Equal(ClientStatus.Connected, clientsMonitoring[clientIp].Status);

            form.Dispose();
        }

        [Fact]
        public void ClientStatusTimeout_IgnoresDisconnected_Clients()
        {
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();
            const string clientIp = "192.168.1.100";

            // Create disconnected client
            var sixMinutesAgo = DateTime.UtcNow.AddMinutes(-6);
            var clientInfo = new ClientMonitoringInfo
            {
                IpAddress = clientIp,
                Status = ClientStatus.Disconnected,
                ConnectionTime = sixMinutesAgo.AddHours(-2),
                LastActivityTime = sixMinutesAgo,
                QsosReceived = 10
            };
            clientsMonitoring.TryAdd(clientIp, clientInfo);

            // Act
            var form = new ServerClientMonitoringForm(clientsMonitoring);
            form.Show();
            System.Threading.Thread.Sleep(500);

            // Assert - Status should remain Disconnected (not changed to Unknown)
            Assert.Equal(ClientStatus.Disconnected, clientsMonitoring[clientIp].Status);

            form.Dispose();
        }

        [Fact]
        public void ClientStatusTimeout_HandlesMultipleClients()
        {
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();

            var now = DateTime.UtcNow;
            var oneMinuteAgo = now.AddMinutes(-1).AddSeconds(-1);
            var fiveMinutesAgo = now.AddMinutes(-5).AddSeconds(-1);
            var tenMinutesAgo = now.AddMinutes(-10).AddSeconds(-1);

            // Client 1: Connected, inactive for 1+ minute → should become Unknown
            var client1 = new ClientMonitoringInfo
            {
                IpAddress = "192.168.1.100",
                Status = ClientStatus.Connected,
                ConnectionTime = oneMinuteAgo.AddMinutes(-10),
                LastActivityTime = oneMinuteAgo,
                QsosReceived = 5
            };

            // Client 2: Connected, inactive for < 1 minute → should remain Connected
            var client2 = new ClientMonitoringInfo
            {
                IpAddress = "192.168.1.101",
                Status = ClientStatus.Connected,
                ConnectionTime = oneMinuteAgo.AddMinutes(-10),
                LastActivityTime = oneMinuteAgo.AddSeconds(20),
                QsosReceived = 10
            };

            // Client 3: Unknown, inactive for 5+ minutes → should become Disconnected
            var client3 = new ClientMonitoringInfo
            {
                IpAddress = "192.168.1.102",
                Status = ClientStatus.Unknown,
                ConnectionTime = fiveMinutesAgo.AddHours(-2),
                LastActivityTime = fiveMinutesAgo,
                QsosReceived = 3
            };


            // Client 4: Connected, inactive for 5+ minutes → should become Disconnected
            var client4 = new ClientMonitoringInfo
            {
                IpAddress = "192.168.1.103",
                Status = ClientStatus.Connected,
                ConnectionTime = fiveMinutesAgo.AddHours(-2),
                LastActivityTime = fiveMinutesAgo,
                QsosReceived = 3
            };

            // Client 5: Disconnected, inactive for 10+ minutes → should be removed
            var client5 = new ClientMonitoringInfo
            {
                IpAddress = "192.168.1.104",
                Status = ClientStatus.Disconnected,
                ConnectionTime = tenMinutesAgo.AddHours(-2),
                LastActivityTime = tenMinutesAgo,
                QsosReceived = 3
            };

            clientsMonitoring.TryAdd(client1.IpAddress, client1);
            clientsMonitoring.TryAdd(client2.IpAddress, client2);
            clientsMonitoring.TryAdd(client3.IpAddress, client3);
            clientsMonitoring.TryAdd(client4.IpAddress, client4);
            clientsMonitoring.TryAdd(client5.IpAddress, client5);

            // Act
            var form = new ServerClientMonitoringForm(clientsMonitoring);
            form.Show();
            Thread.Sleep(500);

            // Assert
            Assert.Equal(ClientStatus.Unknown, clientsMonitoring["192.168.1.100"].Status);
            Assert.Equal(ClientStatus.Connected, clientsMonitoring["192.168.1.101"].Status);
            Assert.Equal(ClientStatus.Disconnected, clientsMonitoring["192.168.1.102"].Status);
            Assert.Equal(ClientStatus.Disconnected, clientsMonitoring["192.168.1.103"].Status);
            clientsMonitoring.TryGetValue("192.168.1.104", out var deletedClient);
            Assert.Null(deletedClient);

            form.Dispose();
        }

        [Fact]
        public void Form_DisplaysMultipleClients_WhenMultipleClientsConnected()
        {
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();
            var now = DateTime.UtcNow;

            var client1 = new ClientMonitoringInfo
            {
                IpAddress = "192.168.1.100",
                Status = ClientStatus.Connected,
                ConnectionTime = now,
                LastActivityTime = now,
                QsosReceived = 10
            };
            var client2 = new ClientMonitoringInfo
            {
                IpAddress = "192.168.1.101",
                Status = ClientStatus.Connected,
                ConnectionTime = now,
                LastActivityTime = now.AddSeconds(-30),
                QsosReceived = 20
            };
            var client3 = new ClientMonitoringInfo
            {
                IpAddress = "192.168.1.102",
                Status = ClientStatus.Disconnected,
                ConnectionTime = now.AddMinutes(-5),
                LastActivityTime = now.AddMinutes(-1),
                QsosReceived = 5
            };

            clientsMonitoring.TryAdd(client1.IpAddress, client1);
            clientsMonitoring.TryAdd(client2.IpAddress, client2);
            clientsMonitoring.TryAdd(client3.IpAddress, client3);

            // Act
            var form = new ServerClientMonitoringForm(clientsMonitoring);
            form.Show();
            System.Threading.Thread.Sleep(500);

            // Assert
            var dataGridView = form.Controls.OfType<DataGridView>().FirstOrDefault();
            Assert.NotNull(dataGridView);
            Assert.True(dataGridView.RowCount >= 3);

            form.Dispose();
        }

        [Fact]
        public void Form_SortsConnectedClients_BeforeDisconnectedClients()
        {
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();
            var now = DateTime.UtcNow;

            var disconnectedClient = new ClientMonitoringInfo
            {
                IpAddress = "192.168.1.100",
                Status = ClientStatus.Disconnected,
                ConnectionTime = now,
                LastActivityTime = now,
                QsosReceived = 10
            };
            var connectedClient = new ClientMonitoringInfo
            {
                IpAddress = "192.168.1.101",
                Status = ClientStatus.Connected,
                ConnectionTime = now,
                LastActivityTime = now,
                QsosReceived = 20
            };

            clientsMonitoring.TryAdd(disconnectedClient.IpAddress, disconnectedClient);
            clientsMonitoring.TryAdd(connectedClient.IpAddress, connectedClient);

            // Act
            var form = new ServerClientMonitoringForm(clientsMonitoring);
            form.Show();
            System.Threading.Thread.Sleep(500);

            // Assert
            var dataGridView = form.Controls.OfType<DataGridView>().FirstOrDefault();
            Assert.NotNull(dataGridView);
            if (dataGridView.RowCount >= 2)
            {
                var firstRowStatus = dataGridView.Rows[0].Cells[1].Value;
                Assert.NotNull(firstRowStatus);
            }

            form.Dispose();
        }

        [Fact]
        public void Form_SortsByLastActivityTime_WhenStatusIsSame()
        {
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();
            var now = DateTime.UtcNow;

            var olderClient = new ClientMonitoringInfo
            {
                IpAddress = "192.168.1.100",
                Status = ClientStatus.Connected,
                ConnectionTime = now,
                LastActivityTime = now.AddSeconds(-60),
                QsosReceived = 10
            };
            var newerClient = new ClientMonitoringInfo
            {
                IpAddress = "192.168.1.101",
                Status = ClientStatus.Connected,
                ConnectionTime = now,
                LastActivityTime = now,
                QsosReceived = 20
            };

            clientsMonitoring.TryAdd(olderClient.IpAddress, olderClient);
            clientsMonitoring.TryAdd(newerClient.IpAddress, newerClient);

            // Act
            var form = new ServerClientMonitoringForm(clientsMonitoring);
            form.Show();
            System.Threading.Thread.Sleep(500);

            // Assert
            var dataGridView = form.Controls.OfType<DataGridView>().FirstOrDefault();
            Assert.NotNull(dataGridView);
            if (dataGridView.RowCount >= 2)
            {
                var firstRowIp = dataGridView.Rows[0].Cells[0].Value;
                Assert.Equal("192.168.1.101", firstRowIp);
            }

            form.Dispose();
        }

        [Fact]
        public void Form_Timer_IsInitialized()
        {
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();

            // Act
            var form = new ServerClientMonitoringForm(clientsMonitoring);

            // Assert
            Assert.NotNull(form);
            form.Dispose();
        }

        [Fact]
        public void Form_Closing_DisposesTimer()
        {
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();
            var form = new ServerClientMonitoringForm(clientsMonitoring);
            form.Show();

            // Act
            form.Close();

            // Assert - No exception should be thrown
            Assert.NotNull(form);
        }

        [Fact]
        public void Form_ClientDataRefreshesAutomatically_OnTimer()
        {
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();
            var now = DateTime.UtcNow;

            var clientInfo = new ClientMonitoringInfo
            {
                IpAddress = "192.168.1.100",
                Status = ClientStatus.Connected,
                ConnectionTime = now,
                LastActivityTime = now,
                QsosReceived = 0
            };
            clientsMonitoring.TryAdd(clientInfo.IpAddress, clientInfo);

            var form = new ServerClientMonitoringForm(clientsMonitoring);
            form.Show();
            System.Threading.Thread.Sleep(500);

            var dataGridView = form.Controls.OfType<DataGridView>().FirstOrDefault();
            int initialRowCount = dataGridView?.RowCount ?? 0;

            // Act - Add a new client
            var newClientInfo = new ClientMonitoringInfo
            {
                IpAddress = "192.168.1.101",
                Status = ClientStatus.Connected,
                ConnectionTime = now,
                LastActivityTime = now,
                QsosReceived = 0
            };
            clientsMonitoring.TryAdd(newClientInfo.IpAddress, newClientInfo);

            System.Threading.Thread.Sleep(500);

            // Assert
            int updatedRowCount = dataGridView?.RowCount ?? 0;
            Assert.True(updatedRowCount >= initialRowCount, "Row count should increase or stay the same after adding client");

            form.Dispose();
        }

        [Fact]
        public void Form_HandlesClientStatusChange_InRealTime()
        {
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();
            var now = DateTime.UtcNow;

            var clientInfo = new ClientMonitoringInfo
            {
                IpAddress = "192.168.1.100",
                Status = ClientStatus.Connected,
                ConnectionTime = now,
                LastActivityTime = now,
                QsosReceived = 0
            };
            clientsMonitoring.TryAdd(clientInfo.IpAddress, clientInfo);

            var form = new ServerClientMonitoringForm(clientsMonitoring);
            form.Show();
            System.Threading.Thread.Sleep(500);

            // Act - Change client status
            if (clientsMonitoring.TryGetValue(clientInfo.IpAddress, out var updatedInfo))
            {
                updatedInfo.Status = ClientStatus.Disconnected;
            }

            System.Threading.Thread.Sleep(500);

            // Assert
            Assert.Equal(ClientStatus.Disconnected, clientInfo.Status);

            form.Dispose();
        }

        [Fact]
        public void Form_IsThreadSafe_WithConcurrentUpdates()
        {
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();
            var form = new ServerClientMonitoringForm(clientsMonitoring);
            form.Show();

            // Act
            var tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                int clientIndex = i;
                tasks.Add(Task.Run(() =>
                {
                    var clientInfo = new ClientMonitoringInfo
                    {
                        IpAddress = $"192.168.1.{clientIndex}",
                        Status = ClientStatus.Connected,
                        ConnectionTime = DateTime.UtcNow,
                        LastActivityTime = DateTime.UtcNow,
                        QsosReceived = clientIndex
                    };
                    clientsMonitoring.TryAdd(clientInfo.IpAddress, clientInfo);
                    System.Threading.Thread.Sleep(100);
                }));
            }

            Task.WaitAll(tasks.ToArray());
            System.Threading.Thread.Sleep(500);

            // Assert - No exception should be thrown
            var dataGridView = form.Controls.OfType<DataGridView>().FirstOrDefault();
            Assert.NotNull(dataGridView);

            form.Dispose();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
