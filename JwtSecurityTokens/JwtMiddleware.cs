using System.Net.NetworkInformation;
using System.Runtime.Serialization.DataContracts;
using Microsoft.Extensions.Options;
using ServiceStack.Configuration;
using Microsoft.Extensions.Configuration;
using System.Data.Entity;
using Stressless_Service.Database;
using System.Configuration;

namespace Stressless_Service.JwtSecurityTokens;

public class JwtMiddleware
{
    IConfiguration _configuration;

    private readonly RequestDelegate _next;
    private readonly database _dbContext = new database();

    public JwtMiddleware(RequestDelegate next, database dbContext, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task Invoke(HttpContext context, IJwtUtility jwtUtils)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var ID = await jwtUtils.ValidateJwtToken(token);

        if (ID == _configuration.GetSection("AppSettings")["AppSettings:ID"])
        {
            return;
        }
    }
}
