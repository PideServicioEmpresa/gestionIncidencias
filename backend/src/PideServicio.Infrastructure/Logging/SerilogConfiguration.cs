namespace PideServicio.Infrastructure.Logging;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

/// <summary>
/// Configuración centralizada de Serilog para PideServicio.
/// Se invoca desde Program.cs antes de construir el host.
/// En producción suprime logs de ruido de EF Core y Microsoft internals.
/// </summary>
public static class SerilogConfiguration
{
    public static WebApplicationBuilder ConfigurarSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, config) =>
        {
            config
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .WriteTo.Console(outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
                .WriteTo.Conditional(
                    evt => evt.Level >= LogEventLevel.Warning,
                    wt => wt.Console(theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code));

            if (context.HostingEnvironment.IsProduction())
            {
                config
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error);
            }
            else
            {
                config
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information);
            }
        });

        return builder;
    }
}
