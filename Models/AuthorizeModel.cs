using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace Stressless_Service.Models;

public class AuthorizeModel : DbContext
{
    public int AuthorizeID { get; set; } // Primnary Key
    public string MACAddress { get; set; }
    public string ClientID { get; set; }
    public string Token { get; set; }
    public string LatestLogin { get; set; }
    public DateTime Expires { get; set; }
}