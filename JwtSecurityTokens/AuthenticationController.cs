using Microsoft.AspNetCore.Mvc;
using ServiceStack.Auth;
using ServiceStack.Script;
using Stressless_Service.Configuration;
using Stressless_Service.Database;
using Stressless_Service.Models;

namespace Stressless_Service.JwtSecurityTokens
{
    public class AuthenticationController : Controller, IAuthenticationController
    {
        private IProductRepository _productRepository;
        private ILogger<AuthenticationController> _logger;
        private ITokenGeneratorService _tokenGeneratorService;

        public AuthenticationController(ILogger<AuthenticationController> logger, ITokenGeneratorService tokenGeneratorService, IProductRepository productRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
            _tokenGeneratorService = tokenGeneratorService;
        }

        public AuthenticationTokenModel Authenticate(string macAddres, string clientID)
        {
            AuthenticationTokenModel returnmodel = new();

            int auth = CheckPreviousAuth(macAddres, clientID).Result;

            if (auth == 1)
            { // NEW USER [REGISTER] GENERATE TOKEN & INSERT AUTH

                var token = _tokenGeneratorService.GenerateToken(clientID);
                Guid AuthID = new();

                // DATABASE
                AuthID = _productRepository.InsertAuthentication(new AuthorizeModel
                {
                    ClientID = clientID,
                    MACAddress = macAddres,
                    Token = token,
                    Expires = DateTime.Now.AddDays(1),
                    LatestLogin = DateTime.Now.AsString()
                });

                // USER VIEW MODEL
                returnmodel = new AuthenticationTokenModel
                {
                    ID = AuthID,
                    Token = token,
                    Expires = DateTime.Now.AddDays(1).ToString(),
                    TokenType = "Bearer"
                };

                _logger.LogInformation($"Bearer Token Created!\nID: {macAddres}");
            }

            else if (auth == 2)
            { // PREVIOUS USER - [AUTHENTICATE] RETRIEVE TOKEN (IF LESS THAN EXPIRY) & PUSH TO USER

                AuthorizeModel latestModel = _productRepository.GetLatestAuthorization(macAddres);

                if (latestModel.Expires <= DateTime.Now.AddHours(1))
                { // LESS THAN 1 HOUR LEFT - EXPIRED

                    var token = _tokenGeneratorService.GenerateToken(latestModel.ClientID);
                    Guid AuthGuild = new();

                    AuthGuild = _productRepository.InsertAuthentication(new AuthorizeModel
                    {
                        ClientID = clientID,
                        MACAddress = macAddres,
                        Token = token,
                        Expires = DateTime.Now.AddDays(1),
                        LatestLogin = DateTime.Now.AsString()
                    });

                    _logger.LogInformation($"Bearer Token Generated...\nID: {macAddres}");
                }

                else if (latestModel.Expires > DateTime.Now.AddHours(1))
                { // Returns the latest model from the database - NOT GENERATE

                    returnmodel = new AuthenticationTokenModel
                    {
                        Token = latestModel.Token,
                        Expires = latestModel.Expires.ToString(),
                        TokenType = "Bearer"
                    };

                    _logger.LogInformation($"Bearer Token Retrieved...\nID: {macAddres}");
                }
            }

            else if (auth == 0)
                returnmodel = null;

            return returnmodel;
        }

        public async Task<int> CheckPreviousAuth(string macAddress, string id, int returnValue = -1)
        {
            int CountOfUserAuth = _productRepository.UserPreviousAuthenticated(macAddress);

            string configurationID = GlobalConfiguration._configuration.GetSection("AppSettings")["ID"];

            if (CountOfUserAuth == 0 && id == configurationID)
            { // NEW USER - [REGISTER] GENERATE TOKEN & INSERT AUTH
                returnValue = 1;
            }

            else if (CountOfUserAuth >= 1 && id == configurationID)
            { // PREVIOUS USER - [AUTHENTICATE] RETRIEVE TOKEN (IF LESS THAN EXPIRY) & PUSH TO USER
                returnValue = 2;
            }

            else if (id != configurationID)
            { // INVALID - [ERROR] CLIENTID INCORRECT
                returnValue = -1;
            }

            return returnValue;
        }
    }
}
