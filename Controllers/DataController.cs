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
using Stressless_Service.JwtSecurityTokens;
using System.Reflection.Metadata.Ecma335;
using ServiceStack;

namespace Stressless_Service.Controllers
{
    [ApiController]
    [System.Web.Http.Route("[controller]")]
    public class DataController : ControllerBase
    {
        private readonly ILogger<DataController> _logger;
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
                    return BadRequest(new { message = "MAC Address or ClientID incorrect!" });

                else
                    return Ok(response);
            }
        }

        [Authorize]
        [HttpGet("GetPrompt/{promptType}")]
        public async Task<PromptModel> GetPrompt(string promptType)
        {
            PromptModel Prompt;
            var BearerToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(BearerToken))
            {
                if ()
                {
                    using (database database = new database())
                    {
                        Prompt = await database.GetPrompt(promptType);
                    }
                }
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
