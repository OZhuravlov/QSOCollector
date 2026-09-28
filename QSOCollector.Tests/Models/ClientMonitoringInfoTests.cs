using QSOCollector.Models;
using Xunit;
using Assert = Xunit.Assert;

namespace QSOCollector.Tests.Models
{
    public class ClientMonitoringInfoTests
    {
        [Fact]
        public void Constructor_SetRequiredIpAddress_Success()
        {
            // Arrange
            const string ipAddress = "192.168.1.100";

            // Act
            var clientInfo = new ClientMonitoringInfo
            {
                IpAddress = ipAddress,
                Status = ClientStatus.Connected,
                ConnectionTime = DateTime.UtcNow,
                LastActivityTime = DateTime.UtcNow,
                QsosReceived = 0
            };

            // Assert
            Assert.NotNull(clientInfo);
            Assert.Equal(ipAddress, clientInfo.IpAddress);
        }

        [Fact]
        public void Status_DefaultValue_IsUnknown()
        {
            // Arrange & Act
            var clientInfo = new ClientMonitoringInfo
            {
                IpAddress = "192.168.1.1"
            };

            // Assert
            Assert.Equal(ClientStatus.Unknown, clientInfo.Status);
        }

        [Fact]
        public void Status_SetToConnected_Success()
        {
            // Arrange
            var clientInfo = new ClientMonitoringInfo
            {
                IpAddress = "192.168.1.1",
                Status = ClientStatus.Connected
            };

            // Act & Assert
            Assert.Equal(ClientStatus.Connected, clientInfo.Status);
        }

        [Fact]
        public void Status_SetToDisconnected_Success()
        {
            // Arrange
            var clientInfo = new ClientMonitoringInfo
            {
                IpAddress = "192.168.1.1",
                Status = ClientStatus.Disconnected
            };

            // Act & Assert
            Assert.Equal(ClientStatus.Disconnected, clientInfo.Status);
        }

        [Fact]
        public void ConnectionTime_CanBeSetAndRetrieved()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var clientInfo = new ClientMonitoringInfo
            {
                IpAddress = "192.168.1.1",
                ConnectionTime = now
            };

            // Act & Assert
            Assert.Equal(now, clientInfo.ConnectionTime);
        }

        [Fact]
        public void LastActivityTime_CanBeSetAndRetrieved()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var clientInfo = new ClientMonitoringInfo
            {
                IpAddress = "192.168.1.1",
                LastActivityTime = now
            };

            // Act & Assert
            Assert.Equal(now, clientInfo.LastActivityTime);
        }

        [Fact]
        public void LastActivityTime_CanBeUpdated()
        {
            // Arrange
            var initialTime = DateTime.UtcNow;
            var clientInfo = new ClientMonitoringInfo
            {
                IpAddress = "192.168.1.1",
                LastActivityTime = initialTime
            };

            // Act
            System.Threading.Thread.Sleep(10);
            var updatedTime = DateTime.UtcNow;
            clientInfo.LastActivityTime = updatedTime;

            // Assert
            Assert.NotEqual(initialTime, clientInfo.LastActivityTime);
            Assert.Equal(updatedTime, clientInfo.LastActivityTime);
        }

        [Fact]
        public void QsosReceived_DefaultValue_IsZero()
        {
            // Arrange & Act
            var clientInfo = new ClientMonitoringInfo
            {
                IpAddress = "192.168.1.1"
            };

            // Assert
            Assert.Equal(0, clientInfo.QsosReceived);
        }

        [Fact]
        public void QsosReceived_CanBeIncremented()
        {
            // Arrange
            var clientInfo = new ClientMonitoringInfo
            {
                IpAddress = "192.168.1.1",
                QsosReceived = 5
            };

            // Act
            clientInfo.QsosReceived += 3;

            // Assert
            Assert.Equal(8, clientInfo.QsosReceived);
        }

        [Fact]
        public void QsosReceived_CanBeSetToLargeNumber()
        {
            // Arrange & Act
            var clientInfo = new ClientMonitoringInfo
            {
                IpAddress = "192.168.1.1",
                QsosReceived = 1000000
            };

            // Assert
            Assert.Equal(1000000, clientInfo.QsosReceived);
        }

        [Fact]
        public void Multiple_ClientMonitoringInfo_Instances_AreIndependent()
        {
            // Arrange
            var client1 = new ClientMonitoringInfo
            {
                IpAddress = "192.168.1.1",
                Status = ClientStatus.Connected,
                QsosReceived = 10
            };

            var client2 = new ClientMonitoringInfo
            {
                IpAddress = "192.168.1.2",
                Status = ClientStatus.Disconnected,
                QsosReceived = 20
            };

            // Act
            client1.QsosReceived += 5;
            client2.QsosReceived += 15;

            // Assert
            Assert.Equal(15, client1.QsosReceived);
            Assert.Equal(35, client2.QsosReceived);
            Assert.Equal(ClientStatus.Connected, client1.Status);
            Assert.Equal(ClientStatus.Disconnected, client2.Status);
        }

        [Theory]
        [InlineData("127.0.0.1")]
        [InlineData("192.168.1.1")]
        [InlineData("10.0.0.1")]
        [InlineData("172.16.0.1")]
        public void ClientMonitoringInfo_AcceptsVariousIpAddresses(string ipAddress)
        {
            // Arrange & Act
            var clientInfo = new ClientMonitoringInfo
            {
                IpAddress = ipAddress
            };

            // Assert
            Assert.Equal(ipAddress, clientInfo.IpAddress);
        }

        [Fact]
        public void ClientStatus_Enum_HasCorrectValues()
        {
            // Arrange & Act & Assert
            Assert.Equal(0, (int)ClientStatus.Unknown);
            Assert.Equal(1, (int)ClientStatus.Connected);
            Assert.Equal(2, (int)ClientStatus.Disconnected);
        }
    }
}
