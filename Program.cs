using Microsoft.AspNetCore.HttpLogging;
using Microsoft.OpenApi.Models;
using Stressless_Service.Auto_Run;
using Stressless_Service.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Stressless_Service.JwtSecurityTokens;
using Stressless_Service.Configuration;
using NLog.Web;
using NLog.Fluent;

var builder = WebApplication.CreateBuilder(args);
var configuration = NLog.LogManager.Configuration;

NLog.LogManager.GetCurrentClassLogger().Info("Stressless Service Starting....");

try
{
    builder.Host.UseNLog();
    builder.Logging.AddNLog(configuration);

    builder.Services.AddHttpLogging(logging => {
        logging.LoggingFields = HttpLoggingFields.All;
    });

    builder.WebHost.ConfigureKestrel((context, options) => {
        options.ListenAnyIP(7257, listenOptions => {
            listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2AndHttp3;
            listenOptions.UseConnectionLogging();
            listenOptions.UseHttps("Stressless-Service.pfx", "J1998ack");
        });
    });

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

    // Add services to the container.

    builder.Services.AddControllers();
    builder.Services.AddLogging();
    builder.Services.AddSingleton<database>();
    builder.Services.AddScoped<ITokenGeneratorService, TokenGeneratorService>();


    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle 
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

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

    using (AutoBootTimer autoBoot = new AutoBootTimer())
        await autoBoot.StartABTimer();

    app.Run();
}

catch (Exception ex)
{
    Log.Fatal("Application terminated unexpectantly" + ex);
}

finally
{
    NLog.LogManager.Flush();
}

