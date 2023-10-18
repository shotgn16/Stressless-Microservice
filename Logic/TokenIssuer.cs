using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Stressless_Service.Database;

namespace Stressless_Service.Logic;

public class TokenIssuer : IDisposable
{
    public async Task<JwtSecurityToken> IssueToken()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SJB443SF8BS48AVSRB43V80KID2LFDFRWVEA"));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "StressLess - Jack Barnard"),
            new Claim("MAC", await Network.GetMac())
        };

        var expiration = DateTime.Now.AddDays(1);

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: "StressLess", 
            audience: "", 
            claims: claims, 
            expires: expiration, 
            signingCredentials: credentials);

        return token;
    }

    public async Task<HttpResponseMessage> VerifyToken(HttpRequestMessage RequestMessage, HttpResponseMessage returnMessage)
    {
        string authorizationHeader = RequestMessage.Headers.GetValues("Authorization").FirstOrDefault();

        if (authorizationHeader == null)
        {
            returnMessage = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            goto Ending;
        }

        string token = authorizationHeader.Split(' ')[1];
        JwtSecurityToken jwtToken = new JwtSecurityToken(token);

        using (database database = new database())
        {
            // Second element [1] in the ....Split function returns the Bearer token key
            string TokenMAC = await GetTokenMAC(token);
            int dbAuth = await database.GetAuth(TokenMAC);

            if (TokenMAC == await Network.GetMac() && dbAuth == 1)
            {
                // Token MAC : MATCH 
                // Token Exists in database : IS STILL VALID

                returnMessage = new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
            }
        }

        Ending:
        return returnMessage;
    }

    private async Task<string> GetTokenMAC(string tokenKey, string returnValue = null)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.ReadToken(tokenKey) as JwtSecurityToken;

        if (securityToken != null)
        {
            var claims = securityToken.Claims;

            var MACAddress = claims.FirstOrDefault(claim => claim.Type == "MAC");

            if (MACAddress != null) {
                returnValue = MACAddress.Value;
            }

            else {
                returnValue = null;
            }
        }

        return returnValue;
    }

    public void Dispose() => GC.Collect();
}
