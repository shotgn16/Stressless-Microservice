using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Stressless_Service.Interfaces
{
    public interface iJWTokenValidation
    {
        Task<bool> Handler(string token, string ClientID = "", bool returnValue = false);
        Task<(bool, SecurityToken)> ValidateToken(string token);
        string GetClaim(string token, string claimType, string returnValue = null);
        List<Claim> ClaimsToList(string token);
    }
}
