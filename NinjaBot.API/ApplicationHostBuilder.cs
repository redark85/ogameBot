using Microsoft.EntityFrameworkCore;
using NinjaBot.Infrastructure.Persistence;

namespace NinjaBot.API;

public static class ApplicationHostBuilder
{
    public static IHostBuilder ConfigureHostBuilder(this IHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((hostingContext, config) =>
        {
            var env = hostingContext.HostingEnvironment;

            config.SetBasePath(env.ContentRootPath);
            config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            config.AddEnvironmentVariables();
        });

        return builder;
    }

    public static void ApplyAppMigrations(this IApplicationBuilder builder, IConfiguration config)
    {
        using var scope = builder.ApplicationServices.CreateScope();
        var appContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        appContext.Database.Migrate();

    }
}
