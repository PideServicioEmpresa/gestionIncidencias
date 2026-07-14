namespace PideServicio.Persistence;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Persistence.Factories;
using PideServicio.Persistence.Options;
using PideServicio.Persistence.Repositories;
using UnitOfWorkImpl = PideServicio.Persistence.UnitOfWork.UnitOfWork;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(
            configuration.GetSection(DatabaseOptions.SeccionNombre));

        // Singleton: DbConnectionFactory mantiene el NpgsqlDataSource (pool de conexiones).
        services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

        // Unit of Work — Scoped: una instancia por request HTTP.
        services.AddScoped<IUnitOfWork, UnitOfWorkImpl>();

        // ── Repositorios — Scoped para alinearse con el ciclo de vida de la petición HTTP ──

        // Fase 6.1 — Entidades base
        services.AddScoped<IEmpresaRepository, EmpresaRepository>();
        services.AddScoped<ISucursalRepository, SucursalRepository>();
        services.AddScoped<IAreaRepository, AreaRepository>();

        // Fase 6.2 — RBAC
        services.AddScoped<IRolRepository, RolRepository>();
        services.AddScoped<IPermisoRepository, PermisoRepository>();

        // Fase 6.4 — Usuarios, Notificaciones y Auditoría
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<INotificacionRepository, NotificacionRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();

        // Fase 6.4 — Ecosistema Ticket
        services.AddScoped<ITicketRepository,           TicketRepository>();
        services.AddScoped<ITicketHistorialRepository,  TicketHistorialRepository>();
        services.AddScoped<ITicketComentarioRepository, TicketComentarioRepository>();
        services.AddScoped<ITicketEvidenciaRepository,  TicketEvidenciaRepository>();
        services.AddScoped<ITicketAsignacionRepository, TicketAsignacionRepository>();

        // Fase 6.4 Parte B — Catálogos de configuración
        services.AddScoped<ITipoServicioRepository,     TipoServicioRepository>();
        services.AddScoped<ICategoriaRepository,         CategoriaRepository>();
        services.AddScoped<IMotivoCancelacionRepository, MotivoCancelacionRepository>();
        services.AddScoped<IMotivoRechazoRepository,     MotivoRechazoRepository>();
        services.AddScoped<IParametroRepository,         ParametroRepository>();
        services.AddScoped<ITecnicoSucursalRepository,   TecnicoSucursalRepository>();

        // Fase 7.0 — Dashboard Analytics
        services.AddScoped<IDashboardRepository, DashboardRepository>();

        return services;
    }
}
