using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Stressless_Service.Configuration;
using Stressless_Service.Interfaces;

namespace Stressless_Service.JwtSecurityTokens;

public class JWTokenValidation : Controller, iJWTokenValidation
{
    private readonly ILogger _logger;
    public JWTokenValidation(ILogger logger)
    {
        _logger = logger;
    }

    public async Task<bool> Handler(string token, string ClientID = "", bool returnValue = false)
    {
        if (!string.IsNullOrEmpty(token))
        {
            (bool, SecurityToken) isValid = await ValidateToken(token);

            if (isValid.Item1)
            {
                ClientID = GetClaim(token, "ClientID");
            }
        }

        if (ClientID == GlobalConfiguration._configuration["AppSettings:ID"])
        {
            returnValue = true;
        }

        return returnValue;
    }

    public async Task<(bool, SecurityToken)> ValidateToken(string token)
    {
        var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(GlobalConfiguration._configuration["AppSettings:Secret"]));
        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken vToken;

        try
        {
            tokenHandler.ValidateToken(token,
                GlobalTokenValidationParameters.ValidationParameters,
                out SecurityToken validatedToken);

            vToken = validatedToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message + ex);
            return (false, null);
        }
        return (true, vToken);
    }

    public string GetClaim(string token, string claimType, string returnValue = null)
    {
        foreach (var claim in ClaimsToList(token))
        {
            if (claim.Type.Equals(claimType))
            {
                returnValue = claim.Value;
            }
        }

        return returnValue;
    }

    public List<Claim> ClaimsToList(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

        if (jsonToken == null)
        {
            throw new InvalidOperationException("Invalid JWT Token");
        }

        return jsonToken.Claims.ToList();
    }
}