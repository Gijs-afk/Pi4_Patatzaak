using Microsoft.AspNetCore.Authentication.Cookies;
using Pi4_Patatzaak.Middleware;
using dotenv.net;
using Microsoft.EntityFrameworkCore;
using Pi4_Patatzaak.Data;
using Serilog;
using Pi4_Patatzaak.Logic;
using Pi4_Patatzaak.Models;

var builder = WebApplication.CreateBuilder(args);
DotEnv.Load();
var EnvVars = DotEnv.Read();
var connectionString = EnvVars["CONNECTIONSTRINGS__DEFAULTCONNECTION"];


Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build()
        )
    .CreateLogger();
Log.Information("Created logger");

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.LoginPath = "/Auth/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });
Log.Information("Added cookies");

builder.Services.AddScoped<WelcomeMessageApiClient>();
builder.Services.AddScoped<AuthLogic>();
builder.Services.AddScoped<PricingLogic>();
builder.Services.AddScoped<OrderLogic>();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddSingleton<DotEnvVariables>(_ => new DotEnvVariables
{
    ApiBaseUrl = EnvVars["APIBASEURI"],
    ApiWelcomeMessage = EnvVars["APIWELCOMEMESSAGE"],
    StandardOrderMessage = EnvVars["STANDARDORDERMESSAGE"]
});

builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

Log.Information("Added dependencies");



var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseSwagger();
    app.UseSwaggerUI();
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseMiddleware<GlobalErrorHandeling>();
Log.Information("Configured errorhandeling");

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
Log.Information("Added authentication");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=auth}/{action=login}/{id?}");
Log.Information("Set standard path");
Log.Information("Starting application!");
app.Run();
