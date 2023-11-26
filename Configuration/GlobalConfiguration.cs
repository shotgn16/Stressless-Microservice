using System.Runtime.InteropServices;
using Stressless_Service.JwtSecurityTokens;

namespace Stressless_Service.Configuration;

public class GlobalConfiguration
{
    private readonly ILogger<GlobalConfiguration> _logger;
    public GlobalConfiguration(ILogger<GlobalConfiguration> logger) => _logger = logger;

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
                    loadConfig();
                }

                return configuration;
            }
        }
    }
    private static async Task loadConfig()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json");

        configuration = builder.Build();
    }
}