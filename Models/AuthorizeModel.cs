using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace Stressless_Service.Models;

public class AuthorizeModel
{
    [Column("AuthorizeID")]
    public Guid ID { get; set; }

    public string MACAddress { get; set; }
    public string ClientID { get; set; }
    public string Token { get; set; }
    public string LatestLogin { get; set; }
    public DateTime Expires { get; set; }
}