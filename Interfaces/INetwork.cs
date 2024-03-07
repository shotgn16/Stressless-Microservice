namespace Stressless_Service.Interfaces
{
    public interface INetwork
    {
        Task<string> GetMac();
        Task<string> GetIPv4();
    }
}
