using Stressless_Service.Controllers;
using Stressless_Service.Database;
using Stressless_Service.Logic;
using Stressless_Service.Models;

namespace Stressless_Service.JwtSecurityTokens;

public class AuthenticateClass : IDisposable
{
    private readonly ILogger<DataController> _logger;
    private readonly ITokenGeneratorService _tokenGeneratorService;

    public AuthenticateClass(ILogger<DataController> logger, ITokenGeneratorService tokenGeneratorService)
    {
        _logger = logger;
        _tokenGeneratorService = tokenGeneratorService;
    }

    public AuthenticationTokenModel Authenticate(AuthorizeModel model)
    {
        AuthenticationTokenModel returnModel = new AuthenticationTokenModel();

        using (database db = new database())
        {
            // Checking if an authentication request with this MAC and ClientID already exists
            int AuthResponse = AuthExists(model.MACAddress, model.ClientID);

            // DOSEN'T EXIST = Creates a new authentication database instance
            if (AuthResponse == 1)
            {
                db.InsertAuth(
                    new AuthorizeModel
                    {
                        ClientID = model.ClientID,
                        MACAddress = model.MACAddress
                    });

                returnModel = new AuthenticationTokenModel
                {
                    Token = _tokenGeneratorService.GenerateToken(model.ClientID),
                    Expires = DateTime.Now.AddDays(1).ToString(),
                    TokenType = "Bearer"
                };
            }

            // DOES EXIST = Updates the time of authentication to DateTime.Now 
            else if (AuthResponse == 2)
            {
                // Updates the [EXISTING USER] database extry with the current datetime - time of new token generation!
                db.UpdateAuth(model.MACAddress);

                returnModel = new AuthenticationTokenModel
                {
                    Token = _tokenGeneratorService.GenerateToken(model.ClientID),
                    Expires = DateTime.Now.AddDays(1).ToString(),
                    TokenType = "Bearer"
                };
            }

            else if (AuthResponse == 0)
                returnModel = null;
        }

        return returnModel;
    }

    public int AuthExists(string MACAddress, string ID)
    {
        int value = 10;
        string configID = GlobalConfiguration._configuration.GetSection("AppSettings")["ID"];

        using (database db = new database())
        {
            if (db.GetAuth(MACAddress).Result == 0 && ID == configID)
            {
                // [NEW USER]
                // IF: An Auth with that MAC DOSEN'T EXIST!
                // IF: The ClientID DOES match the one in appsettings.json
                value = 1;
            }

            else if (db.GetAuth(MACAddress).Result == 1 && ID == configID)
            {
                // [RETURNING USER]
                // IF: An Auth with that MAC DOES EXIST!
                // IF: The ClientID does match
                value = 2;
            }

            else if (db.GetAuth(MACAddress).Result == 0 && ID != configID)
                // [NEW USER - INCORRECT CLIENTID]
                // IF: An Auth with that MAC DOES ALREADY exists in the database
                // IF: The ClientID DOES NOT match the one in the appsettings.json

                value = 0;
        }

        return value;
    }

    public void Dispose() => GC.Collect();
}
