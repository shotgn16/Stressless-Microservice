using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Stressless_Service.Logic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Stressless_Service.JwtSecurityTokens;

public interface IJwtUtility
{
    public Task<string> GenerateJwtToken();
    public Task<string> ValidateJwtToken(string? token);
}

public class JwtUtility : IJwtUtility, IDisposable
{
    private IConfiguration _configuration;

    public JwtUtility(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> GenerateJwtToken()
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings")["AppSettings:Secret"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("ID", _configuration.GetSection("AppSettings")["AppSettings:ID"]) }),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<string> ValidateJwtToken(string? token)
    {
        if (token == null)
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings")["AppSettings:Secret"]);
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var ID = Convert.ToString(jwtToken.Claims.First(x => x.Type == "ID").Value);

            // return account id from JWT token if validation successful
            return ID;
        }
        catch
        {
            // return null if validation fails
            return null;
        }
    }

    public void Dispose() => GC.Collect();

}
