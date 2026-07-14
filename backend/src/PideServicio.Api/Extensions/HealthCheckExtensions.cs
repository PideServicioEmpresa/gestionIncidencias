namespace PideServicio.Api.Extensions;

using PideServicio.Persistence.Options;

public static class HealthCheckExtensions
{
    public static IHealthChecksBuilder AddHealthCheckExtensions(
        this IHealthChecksBuilder builder,
        IConfiguration configuration)
    {
        var dbOpts = configuration
            .GetSection(DatabaseOptions.SeccionNombre)
            .Get<DatabaseOptions>();

        if (!string.IsNullOrEmpty(dbOpts?.ConnectionString))
        {
            builder.AddNpgSql(
                dbOpts.ConnectionString,
                name: "postgresql",
                tags: ["bd", "postgresql"]);
        }

        return builder;
    }
}
