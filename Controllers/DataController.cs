using Microsoft.AspNetCore.Mvc;
using Stressless_Service.Database;
using Stressless_Service.Models;
using Microsoft.AspNetCore.Authorization;
using Stressless_Service.JwtSecurityTokens;
using Stressless_Service.Forecaster;
using Newtonsoft.Json;

namespace Stressless_Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
        private readonly IEventController _eventController;
        private readonly IAuthenticationController _authenticationController;

        public DataController(ILogger<DataController> logger, ITokenGeneratorService tokenGeneratorService, IProductRepository productRepository, IEventController eventController, IAuthenticationController authenticationController)
        {
            _logger = logger;
            _tokenGeneratorService = tokenGeneratorService;
            _productRepository = productRepository;
            _eventController = eventController;
            _authenticationController = authenticationController;
        }

        // Authorization Request
        [HttpPost("Authorize")]
        public async Task<AuthenticationTokenModel> Authorize([FromBody] RequestAuthorizationModel _model)
        {
            var response = _authenticationController.Authenticate(_model.MACAddress, _model.ClientID);

            if (response == null)
            {
                _logger.LogInformation($"Class: 'DataController' | Function: 'Authorize' | Status: 'Failed' | User: '{_model.MACAddress}' Message: 'Please ensure you have a valid MAC Address and ClientID.'");
                throw new ArgumentNullException("MAC Address or ClientID incorrect!");
            }

            else
            {
                _logger.LogInformation($"Class: 'DataController' | Function: 'Authorize' | Status: 'Success' | User: '{_model.MACAddress}'");
                return response;
            }
        }

        [Authorize]
        [HttpGet("GetPrompt/{promptType}")]
        public async Task<PromptModel> GetPrompt(string promptType)
        {
            PromptModel Prompt = new PromptModel();
            var BearerToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            try
            {
                if (!string.IsNullOrEmpty(BearerToken))
                {
                    using (JWTokenValidation tokenValidation = new JWTokenValidation(_logger))
                    {
                        if (await tokenValidation.Handler(BearerToken))
                        {
                            Prompt = _productRepository.GetPrompt(promptType);
                        }
                    }
                }

                _logger.LogInformation("Class: 'DataController' | Function: 'GetPrompt' | Status - " + JsonConvert.SerializeObject(promptType, Formatting.Indented));
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return Prompt;
        }

        [Authorize]
        [HttpPost("InsertPrompt")]
        public async Task<IActionResult> InsertPrompt([FromBody] PromptRequestModel PromptRequest)
        {
            List<string> types = new();
            var BearerToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            try
            {
                if (PromptRequest == null)
                {
                    return BadRequest("Invalid configuration!");
                }

                if (!string.IsNullOrEmpty(BearerToken))
                {
                    using (JWTokenValidation tokenValidation = new JWTokenValidation(_logger))
                    {
                        if (await tokenValidation.Handler(BearerToken))
                        {
                            foreach (var Item in PromptRequest.Prompt)
                            {
                                types = _productRepository.InsertPrompt(Item);
                            }
                        }
                    }
                }

                _logger.LogInformation("Class: 'DataController' | Function: 'InsertPrompt' | Status - " + JsonConvert.SerializeObject(PromptRequest, Formatting.Indented));
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return Ok(types.ToList());
        }

        [Authorize]
        [HttpGet("GetConfiguration")]
        public async Task<ConfigurationClass> GetConfiguration()
        {
            ConfigurationClass Configuration = new ConfigurationClass();
            var BearerToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            try
            {
                if (!string.IsNullOrEmpty(BearerToken))
                {
                    using (JWTokenValidation tokenValidation = new JWTokenValidation(_logger))
                    {
                        if (await tokenValidation.Handler(BearerToken))
                        {
                            Configuration = _productRepository.GetConfiguration();
                        }
                    }
                }

                _logger.LogInformation("Class: 'DataController' | Function: 'GetConfiguration' | Status - " + JsonConvert.SerializeObject(Configuration, Formatting.Indented));
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return Configuration;
        }


        // HERE NEXT TIME
        [Authorize]
        [HttpPost("InsertConfiguration")]
        public async Task<IActionResult> InsertConfiguration([FromBody] ConfigurationClass Configuration)
        {
            if (Configuration == null)
            {
                return BadRequest("Invalid configuration!");
            }

            var BearerToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(BearerToken))
            {
                using (JWTokenValidation tokenValidation = new JWTokenValidation(_logger))
                {
                    if (await tokenValidation.Handler(BearerToken))
                    {
                        var OriginalConfiguration = _productRepository.GetConfiguration();

                        if (OriginalConfiguration == null || OriginalConfiguration != Configuration)
                        {
                            _productRepository.DeleteConfiguration();
                            await _productRepository.InsertConfiguration(Configuration);
                        }
                    }
                }
            }
            return Ok("Success!");
        }

        [Authorize]
        [HttpGet("GetUsedPrompt/{promptID}")]
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
                        UsedPrompt = _productRepository.GetUsedPrompt(promptID);
                    }
                }
            }

            return UsedPrompt;
        }

        [Authorize]
        [HttpPost("InsertUsedPrompt")]
        public async Task<IActionResult> InsertUsedPrompt([FromBody] UsedPromptsModel UsedPrompt)
        {
            if (UsedPrompt == null)
            {
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
        public async Task<bool> PromptReminder(bool reminderUser = false)
        {
            try
            {
                var BearerToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (!string.IsNullOrEmpty(BearerToken))
                {
                    using (JWTokenValidation tokenValidation = new JWTokenValidation(_logger))
                    {
                        if (await tokenValidation.Handler(BearerToken))
                        {
                            ConfigurationClass Configuration = _productRepository.GetConfiguration();

                            if (Configuration != null && !string.IsNullOrEmpty(Configuration.CalenderImport))
                            {
                                await _eventController.EventHandler(Configuration.Calender, Configuration);
                                reminderUser = await _eventController.PromptBreak();
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

        [Authorize]
        [HttpDelete("DeleteConfiguration")]
        public async Task<bool> DeleteConfiguration(bool Value = true)
        {
            try
            {
                var BearerToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                
                if (!string.IsNullOrEmpty(BearerToken))
                {
                    using (JWTokenValidation tokenValidation = new JWTokenValidation(_logger))
                    {
                        if (await tokenValidation.Handler(BearerToken))
                        {
                            _productRepository.DeleteConfiguration();
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Value = false;
                _logger.LogError(ex.Message, ex);
            }

            return Value;
        }
    }
}
