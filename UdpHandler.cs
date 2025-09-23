using System.Net;
using System.Text;

namespace QSOCollector
{
    public class UdpHandler
    {
        public void HandleUdpPacket(byte[] data, int port)
        {
            string message = Encoding.UTF8.GetString(data);
            Console.WriteLine($"Received message from {port}: {message}");

            // Add additional processing logic here
        }
    }
}