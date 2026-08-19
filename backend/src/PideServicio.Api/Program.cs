using Microsoft.Extensions.Configuration.Json;
using Serilog;
using PideServicio.Api.Extensions;
using PideServicio.Application;
using PideServicio.Infrastructure;
using PideServicio.Persistence;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Iniciando PideServicio API...");

    var builder = WebApplication.CreateBuilder(args);

    // ---------------------------------------------------------------------------
    // Desactivar la recarga en caliente de los appsettings.
    //
    // CreateBuilder registra appsettings.json y appsettings.{Environment}.json con
    // reloadOnChange: true, y cada uno abre un FileSystemWatcher (una instancia de
    // inotify en Linux). En contenedores con el límite por defecto esto agota la
    // cuota y el arranque falla con:
    //   IOException: The configured user limit (128) on the number of inotify
    //   instances has been reached.
    //
    // No se necesita: la configuración que cambia en producción llega por variables
    // de entorno, que se leen al iniciar y no dependen de ningún watcher.
    //
    // Se sustituye cada fuente EN SU MISMA POSICIÓN para no alterar la precedencia:
    // las variables de entorno deben seguir ganando sobre los archivos JSON.
    // ---------------------------------------------------------------------------
    var fuentes = builder.Configuration.Sources;
    for (var i = 0; i < fuentes.Count; i++)
    {
        if (fuentes[i] is JsonConfigurationSource { ReloadOnChange: true, Path: not null } json)
        {
            fuentes[i] = new JsonConfigurationSource
            {
                Path = json.Path,
                Optional = json.Optional,
                FileProvider = json.FileProvider,
                ReloadOnChange = false,
            };
        }
    }

    builder.Host.UseSerilog((ctx, lc) => lc
        .ReadFrom.Configuration(ctx.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithThreadId()
        .WriteTo.Console(outputTemplate:
            "[{Timestamp:HH:mm:ss} {Level:u3}] [{TraceId}] {Message:lj}{NewLine}{Exception}")
        .WriteTo.File(
            "logs/pideservicio-.log",
            rollingInterval: RollingInterval.Day,
            outputTemplate:
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{TraceId}] {Message:lj}{NewLine}{Exception}"));

    builder.Services
        .AddApplication(builder.Configuration)
        .AddInfrastructure(builder.Configuration)
        .AddPersistence(builder.Configuration)
        .AddApiServices(builder.Configuration);

    var app = builder.Build();

    app.UseApiPipeline();

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Error fatal al iniciar PideServicio API");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
