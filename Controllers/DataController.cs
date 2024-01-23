using Microsoft.AspNetCore.Mvc;
using Stressless_Service.Database;
using Stressless_Service.Models;
using Microsoft.AspNetCore.Authorization;
using Stressless_Service.JwtSecurityTokens;
using NLog.Fluent;
using Stressless_Service.Forecaster;
using NLog;

namespace Stressless_Service.Controllers
{
    [ApiController]
    [System.Web.Http.Route("[controller]")]
    public class DataController : ControllerBase
    {
        /// <summary>
        /// * ILogger: Is used to capture messages and errors that occur within this constructor and write them to a designated location. The ILogger implementation feeds directly to the NLog configuration
        /// * ITokenGeneratorService: Used to create a constructor-wide instance of the service used to generate OAuth Tokens. When used, this service can take in a ClientID and MacAddress(UUID) and return a fully generated Base64 OAuth Token used for authentcation between and client and server
        /// * IProductRepository: Used to create a constructor-wide instance of the service that provdes access to 
        /// </summary>
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private readonly ITokenGeneratorService _tokenGeneratorService;
        private readonly IProductRepository _productRepository;
        private readonly EventController _eventController;
        private readonly AuthenticationController _authenticationController;

        public DataController(ILogger<DataController> logger, ITokenGeneratorService tokenGeneratorService, IProductRepository productRepository, EventController eventController, AuthenticationController authenticationController)
        {
            _logger = logger;
            _tokenGeneratorService = tokenGeneratorService;
            _productRepository = productRepository;
            _eventController = eventController;
            _authenticationController = authenticationController;
        }

        // Authorization Request
        [HttpPost("Authorize")]
        public async Task<ActionResult<AuthenticationTokenModel>> Authorize(string macAddress, string clientID)
        {
            var response = _authenticationController.Authenticate(macAddress, clientID);

            if (response == null) {
                _logger.LogInformation($"Authentication Failed for User: {macAddress}\nPlease ensure you have a valid MAC Address and ClientID.");
                return BadRequest(new { message = "MAC Address or ClientID incorrect!" });
            }

            else {
                _logger.LogInformation($"User: {macAddress} Authenticated Successfully!");
                return Ok(response);
            }
        }

        [Authorize]
        [HttpGet("GetPrompt/{promptType}")]
        public async Task<PromptModel> GetPrompt(string promptType)
        {

            PromptModel Prompt = new PromptModel();
            var BearerToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(BearerToken))
            {
                using (JWTokenValidation tokenValidation = new JWTokenValidation(_logger))
                {
                    if (await tokenValidation.Handler(BearerToken))
                    {
                        Prompt = await _productRepository.GetPrompt(promptType);
                    }
                }
            }

            return Prompt;
        }

        [Authorize]
        [HttpPost("InsertPrompt")]
        public async Task<IActionResult> InsertPrompt([FromBody] PromptRequestModel PromptRequest)
        {
            if (PromptRequest == null) {
                return BadRequest("Invalid configuration!");
            }

            var BearerToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(BearerToken))
            {
                using (JWTokenValidation tokenValidation = new JWTokenValidation(_logger))
                {
                    if (await tokenValidation.Handler(BearerToken))
                    {
                        foreach (var Item in PromptRequest.Prompt)
                        {
                            _productRepository.InsertPrompt(Item);
                        }
                    }
                }
            }

            return Ok("Success!");
        }

        [Authorize]
        [HttpGet("GetConfiguration")]
        public async Task<ConfigurationClass> GetConfiguration()
        {
            ConfigurationClass Configuration = new ConfigurationClass();
            var BearerToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(BearerToken))
            {
                using (JWTokenValidation tokenValidation = new JWTokenValidation(_logger))
                {
                    if (await tokenValidation.Handler(BearerToken))
                    {
                        Configuration = await _productRepository.GetConfiguration();
                    }
                }
            }

            return Configuration;
        }

        [Authorize]
        [HttpPost("InsertConfiguration")]
        public async Task<IActionResult> InsertConfiguration([FromBody] ConfigurationClass Configuration)
        {
            if (Configuration == null) {
                return BadRequest("Invalid configuration!");
            }

            var BearerToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(BearerToken))
            {
                using (JWTokenValidation tokenValidation = new JWTokenValidation(_logger))
                {
                    if (await tokenValidation.Handler(BearerToken))
                    {
                        var OriginalConfiguration = await _productRepository.GetConfiguration();

                        if (OriginalConfiguration == null || OriginalConfiguration != Configuration)
                        {
                            await _productRepository.DeleteConfiguration();
                                await _productRepository.InsertConfiguration(Configuration);
                        }
                    }
                }
            } 
            return Ok("Success!");
        }

        [Authorize]
        [HttpGet("GetUsedPrompt")]
        public async Task<UsedPromptsModel> GetUsedPrompt(Guid promptID)
        {
            UsedPromptsModel UsedPrompt = new UsedPromptsModel();
            var BearerToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(BearerToken))
            {
                using (JWTokenValidation tokenValidation = new JWTokenValidation(_logger))
                {
                    if (await tokenValidation.Handler(BearerToken))
                    {
                        //Will get the last used prompt specified by ID (Ordered by the specified time in the database - DESC [Most recent at the top])
                        UsedPrompt = await _productRepository.GetUsedPrompt(promptID);
                    }
                }
            }

            return UsedPrompt;
        }

        [Authorize]
        [HttpPost("InsertUsedPrompt")]
        public async Task<IActionResult> InsertUsedPrompt([FromBody] UsedPromptsModel UsedPrompt)
        {
            if (UsedPrompt == null) {
                return BadRequest("Invalid configuration!");
            }

            var BearerToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(BearerToken))
            {
                using (JWTokenValidation tokenValidation = new JWTokenValidation(_logger))
                {
                    if (await tokenValidation.Handler(BearerToken))
                    {
                        _productRepository.InsertUsedPrompt(UsedPrompt);
                    }
                }
            }

            return Ok("Success!");
        }

        [Authorize]
        [HttpPost("PromptReminder")]
        public async Task<bool> PromptReminder()
        {
            bool reminderUser = false;

            try
            {
                // Define a varaible and fill it with the Bearer token value that the client should have sent. 
                // This value will have been generated from the 'Authorize' endpoint on this app but is used to authenticate the application
                var BearerToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                // Checking that the token value isn't empty 
                if (!string.IsNullOrEmpty(BearerToken))
                {
                    using (JWTokenValidation tokenValidation = new JWTokenValidation(_logger))
                    {
                        // Validating the BearerToken...
                        if (await tokenValidation.Handler(BearerToken))
                        {
                            // If the token is VALID, it will continue into this code...

                            ConfigurationClass Configuration = await _productRepository.GetConfiguration();

                            if (Configuration != null && !string.IsNullOrEmpty(Configuration.CalenderImport))
                            {
                                await _eventController.EventHandler(Configuration.Calender, Configuration);
                                reminderUser = await _eventController.PromptBreak(Configuration);
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            // IF:True, Remind User... | IF:False, Do Nothing...
            return reminderUser;
        }
    } 
}
