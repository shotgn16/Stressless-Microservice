using Microsoft.AspNetCore.Mvc;
using Stressless_Service.Database;
using Stressless_Service.Models;
using Newtonsoft.Json;

namespace Stressless_Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        private readonly ILogger<DataController> _logger;
        public DataController(ILogger<DataController> logger) => _logger = logger;

        [HttpGet("GetPrompt")]
        public async Task<PromptModel> GetPrompt(int promptID)
        {
            PromptModel Prompt;

            using (database database = new database())
            {
                Prompt = await database.GetPrompt(promptID);
            }

            return Prompt;
        }

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
