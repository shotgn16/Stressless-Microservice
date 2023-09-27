using System.Net;

namespace Stressless_Service.Models;

public class AuthorizeModel
{
    public string IpAddress { get; set; }
    public string AudienceCode { get; set; }
}
