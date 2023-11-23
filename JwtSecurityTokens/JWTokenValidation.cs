﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
            (bool, SecurityToken) isValid = await ValidateToken(token);

            if (isValid.Item1) {
                Sub = GetClaim(token, "Sub");
            }
        }

        if (Sub == GlobalConfiguration._configuration["AppSettings:ID"]) {
            returnValue = true;
        }

        return returnValue;
    }

    public async Task<(bool, SecurityToken)> ValidateToken(string token)
    {
        var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(GlobalConfiguration._configuration["AppSettings:Secret"]));
        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken vToken;

        try {
            tokenHandler.ValidateToken(token, 
                GlobalTokenValidationParameters.ValidationParameters, 
                out SecurityToken validatedToken);

            vToken = validatedToken;
        }
        catch (Exception ex) {
            Log.Error(ex.Message, ex);
            return (false, null);
        }
        return (true, vToken);
    }

    public string GetClaim(string token, string claimType, string returnValue = null)
    {
        foreach (var claim in ClaimsToList(token)) {
            if (claim.Type.Equals(claimType)) {
                returnValue = claim.Value;
            }
        }

        return returnValue;
    }

    private static List<Claim> ClaimsToList(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

        if (jsonToken == null) {
            throw new InvalidOperationException("Invalid JWT Token");
        }

        return jsonToken.Claims.ToList();
    }

    public void Dispose() => GC.Collect();
}
