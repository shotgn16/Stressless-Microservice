using System.Net;

namespace Stressless_Service.Models;

public class AuthorizeModel
{
    public string MACAddress { get; set; }
    public string ClientID { get; set; }
}