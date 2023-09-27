using Microsoft.AspNetCore.Mvc;
using Stressless_Service.Database;
using Stressless_Service.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Stressless_Service.Logic;
using System.Net;
using Microsoft.AspNetCore.Authentication;

namespace Stressless_Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        private readonly ILogger<DataController> _logger;
        public DataController(ILogger<DataController> logger) => _logger = logger;

        //TODO - Re-write / Improve the structure of this request!
        [HttpPost("Authorize")]
        public async Task<AuthenticationTokenModel> Authorize([FromBody] AuthorizeModel AuthorizeModel)
        {
            AuthenticationTokenModel AuthenticationTokenModel = new AuthenticationTokenModel();

            using (database database = new database()) 
            using (TokenIssuer tokenIssuer = new TokenIssuer())
            {
                if (AuthorizeModel == null)
                {
                    await database.InsertAuth(AuthorizeModel);
                    string token = await tokenIssuer.IssueToken();

                    DateTime Expires = DateTime.Now.AddDays(1);

                    AuthenticationTokenModel = new AuthenticationTokenModel()
                    {
                        Token = token,
                        Expires = Expires.Millisecond,
                        TokenType = "Bearer"
                    };
                }

                else if (AuthorizeModel != null) 
                {
                    throw new ArgumentNullException("Configuration already exists!");
                }

                else if ()
                {

                }
            }

            return AuthenticationTokenModel;
        }


            //        AuthenticationTokenModel tokenModel = new AuthenticationTokenModel();
            //string token;

            //using (database database = new database()) {
            //    AuthorizeModel result = await database.GetAuth(authorizeModel.IpAddress);

            //    if (result == null)
            //    {
            //        using (TokenIssuer Issuer = new TokenIssuer())
            //        {
            //            token = await Issuer.IssueToken();

            //            tokenModel.Token = token;
            //            tokenModel.TokenType = "Bearer";
            //            tokenModel.Expires = 86400000;
            //        }
            //    }

            //    else if (result != null || !TokenIssuer.ValidAudiences.Contains(result.AudienceCode))
            //    {
            //        throw new AccessViolationException("Authentication instance already exists!");
            //    }
            //}

            //return tokenModel;

        [Authorize]
        [HttpGet("GetPrompt")]
        public async Task<PromptModel> GetPrompt(string promptType)
        {
            PromptModel Prompt;

            using (database database = new database())
            {
                Prompt = await database.GetPrompt(promptType);
            }

            return Prompt;
        }

        [Authorize]
        [HttpPost("InsertPrompt")]
        public async Task<IActionResult> InsertPrompt([FromBody] PromptRequestModel PromptRequest)
        {
            if (PromptRequest == null)
            {
                return BadRequest("Invalid configuration!");
            }

            using (database database = new database())
            {
                foreach (var Item in PromptRequest.Prompt)
                {
                    await database.InsertPrompt(Item);
                }
            }

            return Ok("Success!");
        }

        [Authorize]
        [HttpGet("GetConfiguration")]
        public async Task<ConfigurationModel> GetConfiguration(int configurationID)
        {
            ConfigurationModel Configuration;

            using (database database = new database())
            {
                Configuration = await database.GetConfiguration(configurationID);
            }

            return Configuration;
        }

        [Authorize]
        [HttpPost("InsertConfiguration")]
        public async Task<IActionResult> InsertConfiguration([FromBody] ConfigurationModel Configuration)
        {
            if (Configuration == null)
            {
                return BadRequest("Invalid configuration!");
            }

            using (database database = new database())
            {
                await database.InsertConfiguration(Configuration);
            }

            return Ok("Success!");
        }

        [Authorize]
        [HttpGet("GetUsedPrompt")]
        public async Task<UsedPromptsModel> GetUsedPrompt(int promptID)
        {
            UsedPromptsModel UsedPrompt;

            using (database database = new database())
            {
                //Will get the last used prompt specified by ID (Ordered by the specified time in the database - DESC [Most recent at the top])
                UsedPrompt = await database.GetUsedPrompts(promptID);
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

            using (database database = new database())
            {
                await database.InsertUsedPrompt(UsedPrompt);
            }

            return Ok("Success!");
        }
    }
}
