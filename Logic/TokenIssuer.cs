using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

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

    public async Task<HttpResponseMessage> VerifyToken(HttpRequestMessage RequestMessage)
    {
        string authorizationHeader = RequestMessage.Headers.GetValues("Authorization").FirstOrDefault();

        if (authorizationHeader == null)
            return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);

        string token = authorizationHeader.Split(' ')[1];
        JwtSecurityToken jwtToken = new JwtSecurityToken(token);

        // Second element [1] in the ....Split function returns the Bearer token key
        string TokenMAC = await GetTokenMAC(token);

        if (TokenMAC == await Network.GetMac() && /* Check it exists in database -> Have database delete tokens after 24 hours */)
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
