using Microsoft.AspNetCore.HttpLogging;
using Microsoft.OpenApi.Models;
using Stressless_Service.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Stressless_Service.JwtSecurityTokens;
using Stressless_Service.Configuration;
using NLog.Web;
using NLog.Extensions.Logging;
using Stressless_Service;
using System.Diagnostics;
using Stressless_Service.logging;
using Stressless_Service.Autorun;
using Stressless_Service.Forecaster;

var builder = WebApplication.CreateBuilder(args);

NLog.LogManager.GetCurrentClassLogger().Info("Stressless Service Starting...");

System.Diagnostics.Trace.Listeners.Add(new NLog.NLogTraceListener { Name = "NLog" });
Trace.Listeners.Add(new NLogTraceListener());

try {

    builder.Host.UseNLog();
    LoggerFactory.Create(builder => builder.AddNLog());

    builder.Services.AddHttpLogging(logging => {
        logging.LoggingFields = HttpLoggingFields.All;
    });

    // Configuring Logging (ILoggerFactory)
    builder.Services.AddLogging(builder =>
    {
        builder.AddConsole();
        builder.AddDebug();
    });

    // Configuring Kestrel to enable HTTPS via certificate
    //https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/endpoints?view=aspnetcore-8.0
    builder.WebHost.ConfigureKestrel((context, options) => {
        options.ListenAnyIP(7257, listenOptions => {
            listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2AndHttp3;
            listenOptions.UseConnectionLogging();
            listenOptions.UseHttps(GlobalConfiguration._configuration["Certificate:File"], GlobalConfiguration._configuration["Certificate:Password"]);
        });
    });

    // Configuring JwtBearer Authentication to enable Bearer token authentication
    builder.Services.AddAuthentication(config => {
        config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    }).AddJwtBearer(options => {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = GlobalTokenValidationParameters.ValidationParameters;
    });

    builder.Services.AddAuthorization(options => {
        options.AddPolicy("isAuthenticated", AuthPolicy => {
            AuthPolicy.RequireAuthenticatedUser();
            AuthPolicy.RequireClaim("ID", GlobalConfiguration._configuration["AppSettings:ID"]);
        });
    });

    builder.Services.AddControllers();

    // Adding database & TokenGeneratorService to services
    builder.Services.AddLogging();
    builder.Services.AddTransient<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<DBConnectionFactory>();
    builder.Services.AddScoped<ITokenGeneratorService, TokenGeneratorService>();
    builder.Services.AddScoped<TimerInitiation>();
    builder.Services.AddScoped<BootController>();
    builder.Services.AddScoped<PromptController>();
    builder.Services.AddScoped<EventController>();
    builder.Services.AddScoped<AuthenticationController>();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle 
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Configuring swager to allow Bearer authentication in the webUI to allow endpoint testing execution via the webUI
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "StressLess API", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },

                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                },
                new List<string>()
            }
        });
    }); 

    var app = builder.Build();

    app.UseHttpLogging();
        app.UseSwaggerUI(options =>
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        });

    app.UseAuthentication();
    app.UseAuthorization();
    
    app.UseHttpsRedirection();
    app.MapControllers();  
    app.UseHsts();

    using (var serviceScope = app.Services.CreateScope())
    {
        var services = serviceScope.ServiceProvider;

        var myTimeService = services.GetRequiredService<TimerInitiation>();
        await myTimeService.InitalizeSystem();
    }

    app.Run();
}

catch (Exception ex)
{
    Console.WriteLine("Application terminated unexpectantly" + ex);
}

finally
{
    NLog.LogManager.Flush();
}