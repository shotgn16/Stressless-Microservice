using System.Net;
using System.Text;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Stressless_Service.Auto_Run;
using Stressless_Service.Logic;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = "SPA17911",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenIssuer.ISK)),
        };
    });

//builder.WebHost.UseKestrel(options =>
//{
//    options.Listen(IPAddress.Any, 5055, listeningOptions =>
//    {
//        listeningOptions.UseHttps("PK-devcert.pfx", "J1998ack");
//        listeningOptions.UseConnectionLogging();
//    });
//});

builder.Services.AddAuthorization();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMvc();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "StressLess API", Version = "v1" }));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.UseHsts();

using (AutoBootTimer autoBoot = new AutoBootTimer())
    await autoBoot.StartABTimer();

app.Run();
