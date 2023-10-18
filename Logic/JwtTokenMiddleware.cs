using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace Stressless_Service.Logic;

public class JwtTokenMiddleware
{
    private readonly RequestDelegate _next;

    public JwtTokenMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string authorizationHeader = context.Request.Headers["Authorization"];
        if (authorizationHeader == null || !authorizationHeader.StartsWith("Bearer "))
        {
            // No JWT token found
            context.Response.StatusCode = 401;
            return;
        }

        var token = authorizationHeader.Substring("Bearer ".Length);

        // Validate the JWT token
        SecurityToken securityToken = await ValidateToken(token);

        if (securityToken == null) 
        {
            // Invalid JWT token
            context.Response.StatusCode = 401;
            return;
        }

        // The JWT token is valid.
        context.Items.Add("SecurityToken", securityToken);

        await _next(context);
    }

    private async Task<SecurityToken> ValidateToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidAudience = "SPA17911",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SJB443SF8BS48AVSRB43V80KID2LFDFRWVEA"))
        };

        var tokenValidator = new JwtSecurityTokenHandler();
        var principle = await tokenValidator.ValidateTokenAsync(token, tokenValidationParameters);

        return principle.SecurityToken;
    }
}
