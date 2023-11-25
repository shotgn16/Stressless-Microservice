using System.Text;
using Microsoft.IdentityModel.Tokens;
using Stressless_Service.Logic;

namespace Stressless_Service.Configuration;

public class GlobalTokenValidationParameters
{
    internal static readonly object _lock = new object();
    public static TokenValidationParameters TokenValidationParameters;

    public static TokenValidationParameters ValidationParameters
    {
        get
        {
            lock (_lock)
            {
                if (TokenValidationParameters == null)
                {
                    TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "stressless-service",
                        ValidAudiences = new string[] {
                            "https://localhost:7257/",
                            "https://localhost:7257/",
                            "https://127.0.0.1:7257/",
                            "http://127.0.0.1:7257",
                            "https://" + Network.GetIPv4().Result + ":7257/",
                            "http://" + Network.GetIPv4().Result + ":7257/" },

                        TokenDecryptionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GlobalConfiguration._configuration["AppSettings:Secret"])),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GlobalConfiguration._configuration["AppSettings:Secret"])),
                        ClockSkew = TimeSpan.Zero
                    };
                }

                return TokenValidationParameters;
            }
        }
    }
}
