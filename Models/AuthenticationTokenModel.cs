using System.ComponentModel.DataAnnotations.Schema;

namespace Stressless_Service.Models;

public class AuthenticationTokenModel
{
    [Column("TokenID")]
    public Guid ID { get; set; }
    public string Expires { get; set; }
    public string TokenType { get; set; }
    public string Token { get; set; }
}
