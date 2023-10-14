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
}
