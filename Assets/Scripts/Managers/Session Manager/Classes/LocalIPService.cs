using System.Net.Sockets;
using System.Net;

public class LocalIPService : IIPService
{
    public string GetLocalIPAddress()
    {
        string localIP = "127.0.0.1"; // Default fallback
        foreach (var host in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (host.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = host.ToString();
                break;
            }
        }
        return localIP;
    }
}