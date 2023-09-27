namespace Stressless_Service.Models;

public class AuthenticationTokenModel
{
    public int Expires { get; set; }
    public string TokenType { get; set; }
    public string Token { get; set; }
}
