using Microsoft.AspNetCore.Mvc;
using Stressless_Service.Configuration;
using Stressless_Service.Database;
using Stressless_Service.Models;

namespace Stressless_Service.JwtSecurityTokens
{
    public class AuthenticationController : Controller
    {
        private IProductRepository _productRepository;
        private ILogger<AuthenticationController> _logger;
        private ITokenGeneratorService _tokenGeneratorService;

        public AuthenticationController(ILogger<AuthenticationController> logger, ITokenGeneratorService tokenGeneratorService)
        {
            _logger = logger;
            _tokenGeneratorService = tokenGeneratorService;
        }

        public async Task<AuthenticationTokenModel> Authenticate(AuthorizeModel authorizeModel)
        {
            AuthenticationTokenModel returnmodel = new();

            int auth = await CheckPreviousAuth(authorizeModel.MACAddress, authorizeModel.ClientID);

            if (auth == 1)
            {
                _productRepository.InsertAuthentication(new AuthorizeModel
                {
                    ClientID = authorizeModel.ClientID,
                    MACAddress = authorizeModel.MACAddress
                });

                returnmodel = new AuthenticationTokenModel
                {
                    Token = _tokenGeneratorService.GenerateToken(authorizeModel.ClientID),
                    Expires = DateTime.Now.AddDays(1).ToString(),
                    TokenType = "Bearer"
                };

                _logger.LogInformation($"Bearer Token Created!\nID: {authorizeModel.MACAddress}");
            }

            else if (auth == 2)
            {
                _productRepository.UpdateAuthentication(authorizeModel.MACAddress);

                returnmodel = new AuthenticationTokenModel
                {
                    Token = _tokenGeneratorService.GenerateToken(authorizeModel.ClientID),
                    Expires = DateTime.Now.AddDays(1).ToString(),
                    TokenType = "Bearer"
                };

                _logger.LogInformation($"Bearer Token Updated!\nID: {authorizeModel.MACAddress}");
            }

            else if (auth == 0)
                returnmodel = null;

            return returnmodel;
        }

        public async Task<int> CheckPreviousAuth(string macAddress, string id)
        {

            int returnValue = -1;
            int auth = await _productRepository.GetAuthentication(macAddress);
            string configID = GlobalConfiguration._configuration.GetSection("AppSettings")["ID"];

            if (auth == 1 && id == configID)
                returnValue = 1;

            else if (auth == 2 && id == configID)
                returnValue = 2;

            else if (auth == 3 && id == configID)
                returnValue = 3;

            return returnValue;
        }
    }
}
