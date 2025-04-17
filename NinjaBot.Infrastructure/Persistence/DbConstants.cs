namespace NinjaBot.Infrastructure.Persistence;

public class DbConstants
{
    public const string Scheme = "co";
    public const string MigrationsTableName = "_ef_migrations";

    public const string SelectForUpdateTag = "ForUpdate";

    public static string GenerateTableSchema(string schema, string entityName)
        => $"{schema}_{entityName}".ToLower();
}
