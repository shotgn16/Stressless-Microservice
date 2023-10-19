using System.Collections;
using System.ComponentModel;
using System.Net.Mail;
using ServiceStack;
using ServiceStack.Auth;
using Stressless_Service.Database;
using Stressless_Service.Models;

namespace Stressless_Service.JwtSecurityTokens;

public interface IAuthenticateClass
{
    Task<AuthenticationTokenModel> Authenticate(AuthorizeModel model);
    Task<int> AuthExists(string MACAddress, string ID);
}

public class AuthenticateClass : IAuthenticateClass
{
    private readonly IJwtUtility _jwtUtility;
    IConfiguration _configuration;
    ILogger _logger;

    public AuthenticateClass(IJwtUtility jwtUtility, IConfiguration configuration, ILogger logger)
    {
        _jwtUtility = jwtUtility;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthenticationTokenModel> Authenticate(AuthorizeModel model)
    {
        AuthenticationTokenModel returnModel = null;

        using (database db = new database())
        {
            int AuthResponse = await AuthExists(model.MACAddress, model.ClientID);

            if (AuthResponse == 1)
            {
                await db.InsertAuth(
                    new AuthorizeModel
                    {
                        ClientID = model.ClientID,
                        MACAddress = model.MACAddress
                    });

                returnModel = new AuthenticationTokenModel
                {
                    Token = await _jwtUtility.GenerateJwtToken(),
                    Expires = DateTime.Now.AddDays(1).ToString(),
                    TokenType = "Bearer"
                };
            }

            else if (AuthResponse == 2)
            {
                // Updates the [EXISTING USER] database extry with the current datetime - time of new token generation!
                await db.UpdateAuth(model.MACAddress);

                returnModel = new AuthenticationTokenModel
                {
                    Token = await _jwtUtility.GenerateJwtToken(),
                    Expires = DateTime.Now.AddDays(1).ToString(),
                    TokenType = "Bearer"
                };
            }

            else if (AuthResponse == 0)
                returnModel = null;
        }

        return returnModel;
    }

    public async Task<int> AuthExists(string MACAddress, string ID)
    {
        int value = 10; 

        using (database db = new database())
        {
            if (await db.GetAuth(MACAddress) == 0 && ID == _configuration.GetSection("AppSettings")["AppSettings:ID"])
            {
                // [NEW USER]
                // IF: An Auth with that MAC DOSEN'T EXIST!
                // IF: The ClientID DOES match the one in appsettings.json
                value = 1;
            }

            else if (await db.GetAuth(MACAddress) == 1 && ID == _configuration.GetSection("AppSettings")["AppSettings:ID"])
            {
                // [RETURNING USER]
                // IF: An Auth with that MAC DOES EXIST!
                // IF: The ClientID does match
                value = 2;
            }

            else if (await db.GetAuth(MACAddress) == 0 && ID != _configuration.GetSection("AppSettings")["AppSettings:Secret"])
                // [NEW USER - INCORRECT CLIENTID]
                // IF: An Auth with that MAC DOES ALREADY exists in the database
                // IF: The ClientID DOES NOT match the one in the appsettings.json
                
                value = 0;
        }

        return value;
    }
}
