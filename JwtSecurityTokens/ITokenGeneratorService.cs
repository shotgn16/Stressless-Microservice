using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Stressless_Service.Logic;

namespace Stressless_Service.JwtSecurityTokens;

public interface ITokenGeneratorService
{
    string GenerateToken(string ID);
}

public class TokenGeneratorService : ITokenGeneratorService
{
    public string GenerateToken(string ID)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GlobalConfiguration._configuration["AppSettings:Secret"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Iss, "stressless-service"),
            new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Exp, DateTime.Now.AddDays(1).ToString()),
            new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
            new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, ID)
        };

        var token = new JwtSecurityToken(
            issuer: "stressless-service",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}