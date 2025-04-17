using NinjaBot.Core.Services;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using NinjaBot.Infrastructure;
using NinjaBot.API;
using NinjaBot.API.Middleware;
using Microsoft.Extensions.DependencyInjection.Extensions;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAppInfrastructure(builder.Configuration);
builder.Services.AddScoped<IWebDriver>(sp =>
{
    var options = new ChromeOptions();
    // Descomentar si querés sin UI
    // options.AddArgument("--headless");
    return new ChromeDriver(options);
});
builder.Services.TryAddScoped<OGameLoginService>();
//builder.Services.AddHttpClient<OGameLoginService>();

var app = builder.Build();
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.ApplyAppMigrations(app.Configuration);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();