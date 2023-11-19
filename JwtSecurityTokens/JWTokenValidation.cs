using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Stressless_Service.Logic;

namespace Stressless_Service.JwtSecurityTokens;

public class JWTokenValidation
{
    public async Task Handler(string token)
    {
        if (!string.IsNullOrEmpty(token))
        {
            
        }
    }

    public async Task<bool> ValidateToken(string token)
    {
        var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(GlobalConfiguration._configuration["AppSettings:Secret"]));

        var myIssuer = "stressless-service";
        string[] myAudience = { "https://localhost:7257", "http://localhost:7257", "https://127.0.0.1:7257", "http://127.0.0.1:7257" };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = myIssuer,
                ValidAudiences = myAudience,
                IssuerSigningKey = mySecurityKey
            },
            out SecurityToken validatedToken);
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message, ex);
            return false;
        }
        return true;
    }

    public string GetClaim(string token, string claimType)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

        var stringClaimValue = securityToken.Claims.First(claim => claim.Type == claimType).Value;

        return stringClaimValue;
    }
}
