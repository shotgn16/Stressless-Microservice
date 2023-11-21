namespace Stressless_Service;

public class Startup
{
    public Startup(IConfiguration config) => Configuration = config;
    public IConfiguration Configuration { get; set; }

    public void ConfigureServices(IServiceCollection services)
    {

    }
}
