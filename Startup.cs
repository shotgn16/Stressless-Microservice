using Stressless_Service.Auto_Run;

namespace Stressless_Service;

public class Startup : IDisposable
{
    private readonly ILogger<Startup> _logger;

    public async Task InitializeSystem()
    {
        using (AutoBootTimer autoBoot = new AutoBootTimer()) {
            await autoBoot.StartABTimer();
        }
    }

    public void Dispose() => GC.Collect();
}
