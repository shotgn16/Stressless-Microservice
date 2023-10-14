using Microsoft.AspNetCore.Mvc;
using Stressless_Service.Database;
using Stressless_Service.Models;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Stressless_Service.Logic;
using System.Net;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Stressless_Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        private readonly ILogger<DataController> _logger;
        public DataController(ILogger<DataController> logger) => _logger = logger;

        // Authorization Request
        [HttpPost("Authorize")]
        public async Task<ActionResult<AuthenticationTokenModel>> Authorize([FromBody] AuthorizeModel RequestBody)
        {
            AuthenticationTokenModel AuthenticationTokenModel = new AuthenticationTokenModel();

            try
            {
                //IF: Request is NOT NULL + AudienceCode is VALID!
                if (!RequestBody.Equals(null) &&
                    RequestBody.AudienceCode.Equals("SPA17911"))
                {
                    //Instancing database
                    using (database database = new database())
                    using (TokenIssuer tokenIssuer = new TokenIssuer())
                    { 
                        //Inserting into database then new auth instance
                        await database.InsertAuth(RequestBody);

                        //Generating an OAuth 2.0 Bearer token
                        JwtSecurityToken token = await tokenIssuer.IssueToken();
                        
                        //Passing all the parameters into the token model
                        AuthenticationTokenModel = new AuthenticationTokenModel()
                        {
                            Token = token.EncodedPayload,
                            Expires = token.ValidTo.ToString(),
                            TokenType = "Bearer"
                        };
                    }
                }

                //IF: Request is NOT NULL + AudienceCode is INVALID!
                else if (!RequestBody.Equals(null) &&
                    !RequestBody.AudienceCode.Equals("SPA17911"))
                {
                    return Unauthorized("Invalid audience code!");
                }

                //IF: Request body is NULL
                else if (RequestBody.Equals(null))
                {
                    return BadRequest("Invalid request body!");
                }
            }

            catch (Exception ex)
            {
                return BadRequest("An Internal Error has occurred! " + ex);
            }

            // Returning the token model
            return Ok(AuthenticationTokenModel);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetConfiguration")]
        public async Task<ConfigurationModel> GetConfiguration()
        {
            ConfigurationModel Configuration;

            using (database database = new database())
            {
                Configuration = await database.GetConfiguration();
            }

            return Configuration;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
