using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Stressless_Service.Logic;

namespace Stressless_Service.JwtSecurityTokens;

public class JWTokenValidation : IDisposable
{
    public async Task<bool> Handler(string token, string Sub = "", bool returnValue = false)
    {
        if (!string.IsNullOrEmpty(token)) {
            bool isValid = await ValidateToken(token);

            if (isValid) {
                Sub = GetClaim(token, JwtRegisteredClaimNames.Sub);
            }
        }

        if (Sub == GlobalConfiguration._configuration["AppSettings:ID"]) {
            returnValue = true;
        }

        return returnValue;
    }

    public async Task<bool> ValidateToken(string token)
    {
        var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(GlobalConfiguration._configuration["AppSettings:Secret"]));

        var myIssuer = "stressless-service";

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidIssuer = myIssuer,
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

    public void Dispose() => GC.Collect();
}
