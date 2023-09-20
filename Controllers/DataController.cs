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
        public async Task<PromptModel> GetPrompt(int PromptID)
        {
            PromptModel Prompt;

            using (database database = new database())
            {
                Prompt = await database.GetPrompt(PromptID);
            }

            return Prompt;
        }

        [HttpPost("InsertPrompt")]
        public async Task<IActionResult> InsertPrompt([FromBody] PromptModel Prompt)
        {
            if (Prompt == null)
            {
                return BadRequest("Invalid configuration!");
            }

            using (database database = new database())
            {
                await database.InsertPrompt(Prompt);
            }

            return Ok("Success!");
        }

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
    }
}
