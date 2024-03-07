using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Stressless_Service.Configuration;
using Stressless_Service.Controllers;

namespace Stressless_Service.JwtSecurityTokens;

public interface ITokenGeneratorService
{
    string GenerateToken(string ID);
}

public class TokenGeneratorService : ITokenGeneratorService
{
    private readonly ILogger<TokenGeneratorService> _logger;
    public TokenGeneratorService(ILogger<TokenGeneratorService> logger) => _logger = logger;

    public string GenerateToken(string ID)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GlobalConfiguration._configuration["AppSettings:Secret"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var handler = new JwtSecurityTokenHandler();

        var claims = new[] {
            new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Iss, "stressless-service"),
            new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Exp, DateTime.Now.AddDays(1).ToString()),
            new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Iat, DateTime.Now.ToString().ToString()),
            new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("ClientID", ID)
        };

        var token = handler.CreateJwtSecurityToken("stressless-service", "https://localhost:7257/",
            new ClaimsIdentity(claims),
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials,
            notBefore: DateTime.Now,
            issuedAt: DateTime.Now);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}