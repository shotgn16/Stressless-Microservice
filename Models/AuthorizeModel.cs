using System.Net;

namespace Stressless_Service.Models;

public class AuthorizeModel
{
    public string MACAddress { get; set; }
    public string ClientID { get; set; }
    public string Token { get; set; }
    public string LatestLogin { get; set; }
    public DateTime Expires { get; set; }
}