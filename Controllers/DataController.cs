﻿using Microsoft.AspNetCore.Mvc;
using Stressless_Service.Database;
using Stressless_Service.Models;
using Microsoft.AspNetCore.Authorization;
using Stressless_Service.JwtSecurityTokens;
using NLog.Fluent;
using Stressless_Service.Forecaster;

namespace Stressless_Service.Controllers
{
    [ApiController]
    [System.Web.Http.Route("[controller]")]
    public class DataController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ITokenGeneratorService _tokenGeneratorService;

        public DataController(ILogger<DataController> logger, ITokenGeneratorService tokenGeneratorService)
        {
            _logger = logger;
            _tokenGeneratorService = tokenGeneratorService;
        }

        // Authorization Request
        [HttpPost("Authorize")]
        public async Task<ActionResult<AuthenticationTokenModel>> Authorize([FromBody] AuthorizeModel model)
        {
            using (AuthenticateClass authenticateClass = new AuthenticateClass(_logger, _tokenGeneratorService))
            {
                var response = authenticateClass.Authenticate(model);

                if (response == null)
                {
                    Log.Warn($"Authentication Failed for User: {model.MACAddress}\nPlease ensure you have a valid MAC Address and ClientID.");
                    return BadRequest(new { message = "MAC Address or ClientID incorrect!" }); 
                }

                else
                {
                    Log.Info($"User: {model.MACAddress} Authenticated Successfully!");
                    return Ok(response);
                }
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
                        using (database database = new database())
                        {
                            Prompt = await database.GetPrompt(promptType);
                        }
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
                        using (database database = new database())
                        {
                            foreach (var Item in PromptRequest.Prompt)
                            {
                                await database.InsertPrompt(Item);
                            }
                        }
                    }
                }
            }

            return Ok("Success!");
        }

        [Authorize]
        [HttpGet("GetConfiguration")]
        public async Task<ConfigurationModel> GetConfiguration()
        {
            ConfigurationModel Configuration = new ConfigurationModel();
            var BearerToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(BearerToken))
            {
                using (JWTokenValidation tokenValidation = new JWTokenValidation(_logger))
                {
                    if (await tokenValidation.Handler(BearerToken))
                    {
                        using (database database = new database())
                        {
                            Configuration = await database.GetConfiguration();
                        }
                    }
                }
            }

            return Configuration;
        }

        [Authorize]
        [HttpPost("InsertConfiguration")]
        public async Task<IActionResult> InsertConfiguration([FromBody] ConfigurationModel Configuration)
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
                        using (database database = new database())
                        {
                            ConfigurationModel OriginalConfiguration = await database.GetConfiguration();

                            if (!OriginalConfiguration.Equals(Configuration))
                            {
                                await database.DeleteConfiguration();
                                    await database.InsertConfiguration(Configuration);
                                    await database.InsertCalenderEvents(Configuration.calender);
                            }
                        }
                    }
                }
            } 
            return Ok("Success!");
        }

        [Authorize]
        [HttpGet("GetUsedPrompt")]
        public async Task<UsedPromptsModel> GetUsedPrompt(int promptID)
        {
            UsedPromptsModel UsedPrompt = new UsedPromptsModel();
            var BearerToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(BearerToken))
            {
                using (JWTokenValidation tokenValidation = new JWTokenValidation(_logger))
                {
                    if (await tokenValidation.Handler(BearerToken))
                    {
                        using (database database = new database())
                        {
                            //Will get the last used prompt specified by ID (Ordered by the specified time in the database - DESC [Most recent at the top])
                            UsedPrompt = await database.GetUsedPrompts(promptID);
                        }
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
                        using (database database = new database())
                        {
                            await database.InsertUsedPrompt(UsedPrompt);
                        }
                    }
                }
            }

            return Ok("Success!");
        }

        [Authorize]
        [HttpPost("PromptReminder")]
        public async Task<bool> PromptReminder([FromBody] ConfigurationModel NewConfiguration)
        {
            bool reminderUser = false;

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

                        using (database database = new database())
                        {
                            ConfigurationModel OriginalConfiguration = await database.GetConfiguration();

                            // If the new configuration is NOT equal to the one stored in the database...
                            if (!OriginalConfiguration.Equals(NewConfiguration))
                            {
                                // Delete the one stored in the database
                                await database.DeleteConfiguration();

                                // Insert the new one to replace it
                                await database.InsertConfiguration(NewConfiguration);
                            }
                        }

                        if (!string.IsNullOrEmpty(NewConfiguration.calenderImport) && NewConfiguration.calender[0].Name != null)
                        {
                            using (EventCalculator Calculator = new EventCalculator())
                            {
                                await Calculator.EventHandler(NewConfiguration.calender, NewConfiguration);
                                reminderUser = await Calculator.PromptBreak(NewConfiguration);
                            }
                        }
                    }
                }
            }

            // IF:True, Remind User... | IF:False, Do Nothing...
            return reminderUser;
        }
    }
}
