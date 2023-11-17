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

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["AppSettings:Secret"])),
                ValidateIssuer = false,
                ValidateAudience = true
            };
        });

    builder.Services.AddAuthorization();

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

        var securityScheme = new OpenApiSecurityScheme
        {
            Name = "JWT Authentication",
            Description = "Enter Bearer token **_only_**",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            Reference = new OpenApiReference
            {
                Id = "Bearer",
                Type = ReferenceType.SecurityScheme
            }
        };

        c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);

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

