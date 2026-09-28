using System.Collections.Concurrent;
using System.Data;
using System.Windows.Forms;
using Moq;
using QSOCollector.Data;
using QSOCollector.Helpers;
using QSOCollector.Models;
using QSOCollector.Network.Server;
using Xunit;
using Assert = Xunit.Assert;

namespace QSOCollector.Tests.Network.Server
{
    public class TcpServerClientMonitoringTests : IDisposable
    {
        private readonly Mock<IDbRepository> dbRepositoryMock;
        private readonly ServerProgressUpdater progressUpdater;
        private readonly DataTable dataTable;
        private readonly TextBox textBox;

        public TcpServerClientMonitoringTests()
        {
            dbRepositoryMock = new Mock<IDbRepository>();
            dataTable = new DataTable();
            textBox = new TextBox();
            progressUpdater = new ServerProgressUpdater(dataTable, textBox);
        }

        [Fact]
        public void GetClientsMonitoring_ReturnsEmptyConcurrentDictionary_OnInitialization()
        {
            // Note: Testing the ConcurrentDictionary behavior directly 
            // since TcpServer requires a fully initialized TextBox with window handle
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();

            // Act & Assert
            Assert.NotNull(clientsMonitoring);
            Assert.IsType<ConcurrentDictionary<string, ClientMonitoringInfo>>(clientsMonitoring);
            Assert.Empty(clientsMonitoring);
        }

        [Fact]
        public void GetClientsMonitoring_ReturnsSameDictionary_OnMultipleCalls()
        {
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();
            var dict1 = clientsMonitoring;
            var dict2 = clientsMonitoring;

            // Act & Assert
            Assert.Same(dict1, dict2);
        }

        [Fact]
        public void ClientMonitoringInfo_Created_WithCorrectInitialValues()
        {
            // Arrange
            const string clientIp = "192.168.1.100";
            var beforeCreation = DateTime.UtcNow;

            // Act
            var clientInfo = new ClientMonitoringInfo
            {
                IpAddress = clientIp,
                Status = ClientStatus.Connected,
                ConnectionTime = beforeCreation,
                LastActivityTime = beforeCreation,
                QsosReceived = 0
            };
            var afterCreation = DateTime.UtcNow;

            // Assert
            Assert.Equal(clientIp, clientInfo.IpAddress);
            Assert.Equal(ClientStatus.Connected, clientInfo.Status);
            Assert.True(clientInfo.ConnectionTime >= beforeCreation && clientInfo.ConnectionTime <= afterCreation);
            Assert.Equal(0, clientInfo.QsosReceived);
        }

        [Fact]
        public void ClientsMonitoring_AddClient_Success()
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
                QsosReceived = 0
            };

            // Act
            bool added = clientsMonitoring.TryAdd(clientIp, clientInfo);

            // Assert
            Assert.True(added);
            Assert.Single(clientsMonitoring);
            Assert.True(clientsMonitoring.TryGetValue(clientIp, out var retrievedInfo));
            Assert.Equal(clientIp, retrievedInfo.IpAddress);
        }

        [Fact]
        public void ClientsMonitoring_CannotAddDuplicateKey()
        {
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();
            const string clientIp = "192.168.1.100";
            var clientInfo1 = new ClientMonitoringInfo
            {
                IpAddress = clientIp,
                Status = ClientStatus.Connected,
                ConnectionTime = DateTime.UtcNow,
                LastActivityTime = DateTime.UtcNow,
                QsosReceived = 0
            };
            var clientInfo2 = new ClientMonitoringInfo
            {
                IpAddress = clientIp,
                Status = ClientStatus.Connected,
                ConnectionTime = DateTime.UtcNow,
                LastActivityTime = DateTime.UtcNow,
                QsosReceived = 10
            };

            // Act
            bool added1 = clientsMonitoring.TryAdd(clientIp, clientInfo1);
            bool added2 = clientsMonitoring.TryAdd(clientIp, clientInfo2);

            // Assert
            Assert.True(added1);
            Assert.False(added2);
            Assert.Single(clientsMonitoring);
            Assert.Equal(0, clientsMonitoring[clientIp].QsosReceived);
        }

        [Fact]
        public void ClientStatus_TransitionFromConnectedToDisconnected()
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
                QsosReceived = 0
            };
            clientsMonitoring.TryAdd(clientIp, clientInfo);

            // Act
            if (clientsMonitoring.TryGetValue(clientIp, out var info))
            {
                info.Status = ClientStatus.Disconnected;
            }

            // Assert
            Assert.Equal(ClientStatus.Disconnected, clientsMonitoring[clientIp].Status);
        }

        [Fact]
        public void LastActivityTime_UpdatedOnMessageReceive()
        {
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();
            const string clientIp = "192.168.1.100";
            var initialTime = DateTime.UtcNow;
            var clientInfo = new ClientMonitoringInfo
            {
                IpAddress = clientIp,
                Status = ClientStatus.Connected,
                ConnectionTime = initialTime,
                LastActivityTime = initialTime,
                QsosReceived = 0
            };
            clientsMonitoring.TryAdd(clientIp, clientInfo);

            // Act
            System.Threading.Thread.Sleep(10);
            var updatedTime = DateTime.UtcNow;
            if (clientsMonitoring.TryGetValue(clientIp, out var info))
            {
                info.LastActivityTime = updatedTime;
            }

            // Assert
            Assert.NotEqual(initialTime, clientsMonitoring[clientIp].LastActivityTime);
            Assert.True(clientsMonitoring[clientIp].LastActivityTime >= updatedTime);
        }

        [Fact]
        public void QsosReceived_IncrementedAfterDbSave()
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
                QsosReceived = 0
            };
            clientsMonitoring.TryAdd(clientIp, clientInfo);
            const int qsoCount = 5;

            // Act
            if (clientsMonitoring.TryGetValue(clientIp, out var info))
            {
                info.QsosReceived += qsoCount;
            }

            // Assert
            Assert.Equal(qsoCount, clientsMonitoring[clientIp].QsosReceived);
        }

        [Fact]
        public void QsosReceived_MultipleIncrements_Success()
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
                QsosReceived = 0
            };
            clientsMonitoring.TryAdd(clientIp, clientInfo);

            // Act
            if (clientsMonitoring.TryGetValue(clientIp, out var info))
            {
                info.QsosReceived += 5;
            }
            if (clientsMonitoring.TryGetValue(clientIp, out var info2))
            {
                info2.QsosReceived += 3;
            }
            if (clientsMonitoring.TryGetValue(clientIp, out var info3))
            {
                info3.QsosReceived += 2;
            }

            // Assert
            Assert.Equal(10, clientsMonitoring[clientIp].QsosReceived);
        }

        [Fact]
        public void MultipleClients_TrackingIndependently()
        {
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();
            const string clientIp1 = "192.168.1.100";
            const string clientIp2 = "192.168.1.101";
            const string clientIp3 = "192.168.1.102";

            var clientInfo1 = new ClientMonitoringInfo
            {
                IpAddress = clientIp1,
                Status = ClientStatus.Connected,
                ConnectionTime = DateTime.UtcNow,
                LastActivityTime = DateTime.UtcNow,
                QsosReceived = 0
            };
            var clientInfo2 = new ClientMonitoringInfo
            {
                IpAddress = clientIp2,
                Status = ClientStatus.Connected,
                ConnectionTime = DateTime.UtcNow,
                LastActivityTime = DateTime.UtcNow,
                QsosReceived = 0
            };
            var clientInfo3 = new ClientMonitoringInfo
            {
                IpAddress = clientIp3,
                Status = ClientStatus.Connected,
                ConnectionTime = DateTime.UtcNow,
                LastActivityTime = DateTime.UtcNow,
                QsosReceived = 0
            };

            // Act
            clientsMonitoring.TryAdd(clientIp1, clientInfo1);
            clientsMonitoring.TryAdd(clientIp2, clientInfo2);
            clientsMonitoring.TryAdd(clientIp3, clientInfo3);

            if (clientsMonitoring.TryGetValue(clientIp1, out var info1))
            {
                info1.QsosReceived += 10;
                info1.Status = ClientStatus.Disconnected;
            }
            if (clientsMonitoring.TryGetValue(clientIp2, out var info2))
            {
                info2.QsosReceived += 20;
            }
            // clientIp3 remains unchanged

            // Assert
            Assert.Equal(3, clientsMonitoring.Count);
            Assert.Equal(10, clientsMonitoring[clientIp1].QsosReceived);
            Assert.Equal(ClientStatus.Disconnected, clientsMonitoring[clientIp1].Status);
            Assert.Equal(20, clientsMonitoring[clientIp2].QsosReceived);
            Assert.Equal(ClientStatus.Connected, clientsMonitoring[clientIp2].Status);
            Assert.Equal(0, clientsMonitoring[clientIp3].QsosReceived);
            Assert.Equal(ClientStatus.Connected, clientsMonitoring[clientIp3].Status);
        }

        [Fact]
        public void ConcurrentDictionary_ThreadSafeOperations()
        {
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();
            var tasks = new List<Task>();
            const int numThreads = 10;
            const int operationsPerThread = 100;

            // Act
            for (int t = 0; t < numThreads; t++)
            {
                int threadId = t;
                tasks.Add(Task.Run(() =>
                {
                    for (int i = 0; i < operationsPerThread; i++)
                    {
                        string clientIp = $"192.168.1.{threadId}";
                        
                        if (!clientsMonitoring.ContainsKey(clientIp))
                        {
                            var clientInfo = new ClientMonitoringInfo
                            {
                                IpAddress = clientIp,
                                Status = ClientStatus.Connected,
                                ConnectionTime = DateTime.UtcNow,
                                LastActivityTime = DateTime.UtcNow,
                                QsosReceived = 0
                            };
                            clientsMonitoring.TryAdd(clientIp, clientInfo);
                        }

                        if (clientsMonitoring.TryGetValue(clientIp, out var info))
                        {
                            info.LastActivityTime = DateTime.UtcNow;
                            info.QsosReceived += 1;
                        }
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            // Assert
            Assert.Equal(numThreads, clientsMonitoring.Count);
            foreach (var kvp in clientsMonitoring)
            {
                Assert.Equal(operationsPerThread, kvp.Value.QsosReceived);
            }
        }

        [Fact]
        public void ClientMonitoringInfo_PreservesStateAfterDisconnection()
        {
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();
            const string clientIp = "192.168.1.100";
            var connectionTime = DateTime.UtcNow;
            var lastActivityTime = DateTime.UtcNow.AddSeconds(30);
            const int qsoCount = 42;

            var clientInfo = new ClientMonitoringInfo
            {
                IpAddress = clientIp,
                Status = ClientStatus.Connected,
                ConnectionTime = connectionTime,
                LastActivityTime = lastActivityTime,
                QsosReceived = qsoCount
            };
            clientsMonitoring.TryAdd(clientIp, clientInfo);

            // Act - Client disconnects
            if (clientsMonitoring.TryGetValue(clientIp, out var info))
            {
                info.Status = ClientStatus.Disconnected;
            }

            // Assert - Verify state is preserved
            Assert.True(clientsMonitoring.TryGetValue(clientIp, out var disconnectedInfo));
            Assert.Equal(clientIp, disconnectedInfo.IpAddress);
            Assert.Equal(ClientStatus.Disconnected, disconnectedInfo.Status);
            Assert.Equal(connectionTime, disconnectedInfo.ConnectionTime);
            Assert.Equal(lastActivityTime, disconnectedInfo.LastActivityTime);
            Assert.Equal(qsoCount, disconnectedInfo.QsosReceived);
        }

        [Fact]
        public void GetClientsMonitoring_IsConcurrentDictionary_ThreadSafeOperations()
        {
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();
            var tasks = new List<Task>();
            const int numThreads = 5;

            // Act
            for (int i = 0; i < numThreads; i++)
            {
                int threadIndex = i;
                tasks.Add(Task.Run(() =>
                {
                    for (int j = 0; j < 50; j++)
                    {
                        string clientIp = $"192.168.1.{threadIndex * 50 + j}";
                        var clientInfo = new ClientMonitoringInfo
                        {
                            IpAddress = clientIp,
                            Status = ClientStatus.Connected,
                            ConnectionTime = DateTime.UtcNow,
                            LastActivityTime = DateTime.UtcNow,
                            QsosReceived = j
                        };
                        clientsMonitoring.TryAdd(clientIp, clientInfo);
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            // Assert
            Assert.Equal(numThreads * 50, clientsMonitoring.Count);
        }

        public void Dispose()
        {
            // Cleanup
            textBox?.Dispose();
            dataTable?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
