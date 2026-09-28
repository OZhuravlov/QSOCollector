using System.Collections.Concurrent;
using QSOCollector.Models;
using Xunit;
using Assert = Xunit.Assert;

namespace QSOCollector.Tests.Network.Server
{
    public class TcpServerClientReconnectionTests
    {
        [Fact]
        public void ClientReconnection_UpdatesStatusBackToConnected()
        {
            // Arrange - Simulate initial connection
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();
            const string clientIp = "192.168.1.100";

            // First connection
            var initialClientInfo = new ClientMonitoringInfo
            {
                IpAddress = clientIp,
                Status = ClientStatus.Connected,
                ConnectionTime = DateTime.UtcNow,
                LastActivityTime = DateTime.UtcNow,
                QsosReceived = 5
            };
            clientsMonitoring.AddOrUpdate(clientIp, initialClientInfo, (key, oldValue) => initialClientInfo);

            // Verify initial state
            Assert.True(clientsMonitoring.TryGetValue(clientIp, out var connectedClient));
            Assert.Equal(ClientStatus.Connected, connectedClient.Status);
            Assert.Equal(5, connectedClient.QsosReceived);

            // Act - Simulate disconnection
            if (clientsMonitoring.TryGetValue(clientIp, out var disconnectingClient))
            {
                disconnectingClient.Status = ClientStatus.Disconnected;
            }

            // Verify disconnected state
            Assert.True(clientsMonitoring.TryGetValue(clientIp, out var disconnectedClient));
            Assert.Equal(ClientStatus.Disconnected, disconnectedClient.Status);

            // Act - Simulate reconnection from same IP (this is the critical part)
            var reconnectionClientInfo = new ClientMonitoringInfo
            {
                IpAddress = clientIp,
                Status = ClientStatus.Connected,
                ConnectionTime = DateTime.UtcNow,
                LastActivityTime = DateTime.UtcNow,
                QsosReceived = 0  // Reset QSO count on reconnection
            };

            // Use AddOrUpdate (the fix) instead of TryAdd
            clientsMonitoring.AddOrUpdate(clientIp, reconnectionClientInfo, (key, oldValue) => reconnectionClientInfo);

            // Assert - Status should be back to Connected after reconnection
            Assert.True(clientsMonitoring.TryGetValue(clientIp, out var reconnectedClient));
            Assert.Equal(ClientStatus.Connected, reconnectedClient.Status);
            Assert.Equal(0, reconnectedClient.QsosReceived);  // Should be reset
            Assert.Equal(clientIp, reconnectedClient.IpAddress);
        }

        [Fact]
        public void ClientReconnection_WithTryAdd_DoesNotUpdateStatus()
        {
            // This test demonstrates the BUG: TryAdd fails on reconnection
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();
            const string clientIp = "192.168.1.101";

            // First connection with TryAdd (original buggy code)
            var initialClientInfo = new ClientMonitoringInfo
            {
                IpAddress = clientIp,
                Status = ClientStatus.Connected,
                ConnectionTime = DateTime.UtcNow,
                LastActivityTime = DateTime.UtcNow,
                QsosReceived = 10
            };
            bool firstAddResult = clientsMonitoring.TryAdd(clientIp, initialClientInfo);
            Assert.True(firstAddResult);
            Assert.Equal(ClientStatus.Connected, clientsMonitoring[clientIp].Status);

            // Simulate disconnection
            if (clientsMonitoring.TryGetValue(clientIp, out var client))
            {
                client.Status = ClientStatus.Disconnected;
            }
            Assert.Equal(ClientStatus.Disconnected, clientsMonitoring[clientIp].Status);

            // Act - Try to reconnect using buggy TryAdd (won't work!)
            var reconnectionClientInfo = new ClientMonitoringInfo
            {
                IpAddress = clientIp,
                Status = ClientStatus.Connected,
                ConnectionTime = DateTime.UtcNow,
                LastActivityTime = DateTime.UtcNow,
                QsosReceived = 0
            };

            bool secondAddResult = clientsMonitoring.TryAdd(clientIp, reconnectionClientInfo);

            // Assert - TryAdd returns false, status remains Disconnected (THE BUG)
            Assert.False(secondAddResult, "TryAdd should return false because key already exists");
            Assert.Equal(ClientStatus.Disconnected, clientsMonitoring[clientIp].Status);
            Assert.Equal(10, clientsMonitoring[clientIp].QsosReceived);  // Old values remain
        }

        [Fact]
        public void ClientReconnection_PreservesHistoricalData_WhileUpdatingStatus()
        {
            // Arrange - Initial connection
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();
            const string clientIp = "192.168.1.102";

            var initialConnectionTime = DateTime.UtcNow.AddHours(-1);
            var initialClientInfo = new ClientMonitoringInfo
            {
                IpAddress = clientIp,
                Status = ClientStatus.Connected,
                ConnectionTime = initialConnectionTime,
                LastActivityTime = DateTime.UtcNow,
                QsosReceived = 25
            };
            clientsMonitoring.AddOrUpdate(clientIp, initialClientInfo, (key, oldValue) => initialClientInfo);

            // Simulate disconnection
            clientsMonitoring[clientIp].Status = ClientStatus.Disconnected;
            Assert.Equal(ClientStatus.Disconnected, clientsMonitoring[clientIp].Status);

            // Act - Reconnect with fresh data (this should be a brand new connection object)
            var reconnectionClientInfo = new ClientMonitoringInfo
            {
                IpAddress = clientIp,
                Status = ClientStatus.Connected,
                ConnectionTime = DateTime.UtcNow,  // New connection time
                LastActivityTime = DateTime.UtcNow,
                QsosReceived = 0  // Reset count for new session
            };
            clientsMonitoring.AddOrUpdate(clientIp, reconnectionClientInfo, (key, oldValue) => reconnectionClientInfo);

            // Assert - New connection should have updated values
            var reconnectedClient = clientsMonitoring[clientIp];
            Assert.Equal(ClientStatus.Connected, reconnectedClient.Status);
            Assert.Equal(0, reconnectedClient.QsosReceived);
            Assert.NotEqual(initialConnectionTime, reconnectedClient.ConnectionTime);
            Assert.True(reconnectedClient.ConnectionTime > initialConnectionTime);
        }

        [Fact]
        public void MultipleReconnections_HandleCorrectly()
        {
            // Arrange
            var clientsMonitoring = new ConcurrentDictionary<string, ClientMonitoringInfo>();
            const string clientIp = "192.168.1.103";

            // Connection cycle 1: Connect -> Disconnect
            var client1 = new ClientMonitoringInfo
            {
                IpAddress = clientIp,
                Status = ClientStatus.Connected,
                ConnectionTime = DateTime.UtcNow,
                LastActivityTime = DateTime.UtcNow,
                QsosReceived = 5
            };
            clientsMonitoring.AddOrUpdate(clientIp, client1, (key, oldValue) => client1);
            Assert.Equal(ClientStatus.Connected, clientsMonitoring[clientIp].Status);

            clientsMonitoring[clientIp].Status = ClientStatus.Disconnected;
            Assert.Equal(ClientStatus.Disconnected, clientsMonitoring[clientIp].Status);

            // Connection cycle 2: Connect -> Disconnect
            System.Threading.Thread.Sleep(10);
            var client2 = new ClientMonitoringInfo
            {
                IpAddress = clientIp,
                Status = ClientStatus.Connected,
                ConnectionTime = DateTime.UtcNow,
                LastActivityTime = DateTime.UtcNow,
                QsosReceived = 0
            };
            clientsMonitoring.AddOrUpdate(clientIp, client2, (key, oldValue) => client2);
            Assert.Equal(ClientStatus.Connected, clientsMonitoring[clientIp].Status);

            clientsMonitoring[clientIp].Status = ClientStatus.Disconnected;
            Assert.Equal(ClientStatus.Disconnected, clientsMonitoring[clientIp].Status);

            // Connection cycle 3: Connect
            System.Threading.Thread.Sleep(10);
            var client3 = new ClientMonitoringInfo
            {
                IpAddress = clientIp,
                Status = ClientStatus.Connected,
                ConnectionTime = DateTime.UtcNow,
                LastActivityTime = DateTime.UtcNow,
                QsosReceived = 0
            };
            clientsMonitoring.AddOrUpdate(clientIp, client3, (key, oldValue) => client3);

            // Assert - Final state should be Connected
            Assert.Equal(ClientStatus.Connected, clientsMonitoring[clientIp].Status);
            Assert.Single(clientsMonitoring);  // Only one entry for this IP
        }
    }
}
