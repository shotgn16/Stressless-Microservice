namespace Stressless_Service.Logic;

public class GlobalConfiguration
{
    internal static readonly object _lock = new object();
    public static IConfiguration configuration;
    public static IConfiguration _configuration
    {
        get
        {
            lock (_lock)
            {
                if (configuration == null)
                {
                     createInstance();
                }

                return configuration;
            }
        }
    }

    private static async Task createInstance()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json");

        configuration = builder.Build();
    }
}
