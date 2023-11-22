using System.Net;
using System.Net.NetworkInformation;

namespace Stressless_Service.Logic;

public class Network
{
    public static async Task<string> GetMac()
    {
        return (from nic in NetworkInterface.GetAllNetworkInterfaces()
        where nic.OperationalStatus == OperationalStatus.Up select 
        nic.GetPhysicalAddress().ToString()).FirstOrDefault();
    }

    public static async Task<string> GetIPv4()
    {
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddress = ipHostInfo.AddressList
            .FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

        return ipAddress.Address.ToString();
    }
}
