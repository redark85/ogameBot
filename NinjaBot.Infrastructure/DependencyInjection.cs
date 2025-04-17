
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using NinjaBot.Domain.Interfaces.Services;
using NinjaBot.Domain.Utils;
using NinjaBot.Infrastructure.Persistence;
using NinjaBot.Infrastructure.Services;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace NinjaBot.Infrastructure;

public static class DependencyInjection
{
    //To add migration
    //Add-Migration UpdateUserEntities -Project NinjaBot.Infrastructure -StartupProject NinjaBot.Api -Context AppDbContext -OutputDir "Migrations"
    //Update-Database -Context AppDbcontext
    public static IServiceCollection AddAppInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddScoped<IAppDataService, AppDataService>();

        services.AddAppDbContext(connectionString!);

        services.AddAutoMapper(c => c.AddProfile(new MappingProfile()));
        return services;
    }

    private static IServiceCollection AddAppDbContext(this IServiceCollection services, string connectionString)
    {
        Check.NotEmpty(connectionString, nameof(connectionString));
#if DEBUG
        IdentityModelEventSource.ShowPII = true;
#endif
        services.AddDbContext<AppDbContext>(options =>
        {
#if DEBUG
            options.UseLoggerFactory(LoggerFactory.Create(c => c.AddDebug().AddConsole()));
            options.EnableSensitiveDataLogging();
#endif
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), o =>
            {
                o.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                o.SchemaBehavior(MySqlSchemaBehavior.Translate, DbConstants.GenerateTableSchema);
                o.MigrationsHistoryTable($"{DbConstants.Scheme}{DbConstants.MigrationsTableName}");
            });
        });

        return services;
    }
}
