using Stressless_Service.Auto_Run;
using Stressless_Service.Database;
using Stressless_Service.JwtSecurityTokens;
using Stressless_Service.Models;

namespace Stressless_Service;

public class Startup : IDisposable
{
    private readonly ILogger _logger;

    public async Task InitializeSystem()
    {
        using (AutoBootTimer autoBoot = new AutoBootTimer()) {
            await autoBoot.StartABTimer();
        }
    }

    public void Dispose() => GC.Collect();

    //public async Task TEST_DATABASE()
    //{
    //    using (database database = new database())
    //    {
    //        database.InsertConfiguration(new ConfigurationModel
    //        {

    //        });
    //    }
    //}

}
