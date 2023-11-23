using Microsoft.AspNetCore.HttpLogging;
using Microsoft.OpenApi.Models;
using Serilog;
using Stressless_Service.Auto_Run;
using Stressless_Service.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Stressless_Service.Logic;
using Stressless_Service.JwtSecurityTokens;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

try
{
    builder.Host.UseSerilog();

    builder.Logging.AddSerilog(new LoggerConfiguration()
        .CreateLogger());

    builder.Services.AddHttpLogging(logging =>
    {
        logging.LoggingFields = HttpLoggingFields.All;
    });

    builder.WebHost.ConfigureKestrel((context, options) =>
    {
        options.ListenAnyIP(7257, listenOptions =>
        {
            listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2AndHttp3;
            listenOptions.UseConnectionLogging();
            listenOptions.UseHttps("Stressless-Service.pfx", "J1998ack");
        });
    });

    builder.Services.AddAuthentication(config =>
    {
        config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    }).AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = GlobalTokenValidationParameters.ValidationParameters;
    });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("isAuthenticated", AuthPolicy =>
        {
            AuthPolicy.RequireAuthenticatedUser();
            AuthPolicy.RequireClaim("ID", GlobalConfiguration._configuration["AppSettings:ID"]);
        });
    });

    // Add services to the container.

    builder.Services.AddControllers();

    builder.Services.AddLogging();
     
    builder.Services.AddSingleton<database>();

    //HERER
    builder.Services.AddScoped<ITokenGeneratorService, TokenGeneratorService>();
    //builder.Services.AddScoped<IAuthenticateClass, AuthenticateClass>();
    //builder.Services.AddScoped<IJwtUtility, JwtUtility>();
     
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

    // Enable HttpLogging (Middleware)
    // #6 [Order]
    app.UseHttpLogging();

    //// Configure the HTTP request pipeline.
    //if (app.Environment.IsDevelopment())
    //{
        app.UseSwaggerUI(options =>
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        });
    //}

    // Enable Authentication / Authorization middlware in the app

    // #3 [Order]
    app.UseAuthentication();
    app.UseAuthorization();

    //app.UseMiddleware<JwtMiddleware>();

    // #1 [Order]
    app.UseHttpsRedirection();

    app.MapControllers();

    // #2 [Order]
    app.UseHsts();

    using (AutoBootTimer autoBoot = new AutoBootTimer())
        await autoBoot.StartABTimer();

    app.Run();
}

catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectantly");
}

finally
{
    Log.CloseAndFlush();
}

