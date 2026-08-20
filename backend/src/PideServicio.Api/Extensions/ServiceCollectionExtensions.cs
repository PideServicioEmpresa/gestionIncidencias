namespace PideServicio.Api.Extensions;

using System.IO.Compression;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PideServicio.Api.Authorization;
using PideServicio.Domain.Enums;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using PideServicio.Contracts.Common;
using PideServicio.Persistence.Options;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
            .AddJsonOptions(opts =>
                opts.JsonSerializerOptions.Converters.Add(
                    new System.Text.Json.Serialization.JsonStringEnumConverter()));
        services.AddEndpointsApiExplorer();

        // ---------------------------------------------------------------------------
        // Errores automáticos de model binding: ASP.NET responde ProblemDetails por
        // defecto, un formato que el frontend no sabe leer y termina mostrando un
        // mensaje genérico. Los reescribimos al mismo contrato ApiResponse que usan
        // el resto de errores para que el usuario vea siempre un mensaje útil.
        // ---------------------------------------------------------------------------
        services.Configure<ApiBehaviorOptions>(opts =>
        {
            opts.InvalidModelStateResponseFactory = context =>
            {
                // "" y "request" son claves internas del binder (cuerpo ausente), no campos reales
                var errores = context.ModelState
                    .Where(kv => kv.Value is { Errors.Count: > 0 }
                                 && !string.IsNullOrEmpty(kv.Key)
                                 && !kv.Key.Equals("request", StringComparison.OrdinalIgnoreCase))
                    .ToDictionary(
                        kv => kv.Key,
                        kv => kv.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

                var mensaje = errores.Count > 0
                    ? $"Revisa los datos enviados: {string.Join(", ", errores.Keys)}."
                    : "Faltan datos requeridos en la solicitud.";

                var respuesta = ApiResponse.Fallo(
                    new ApiError("ERROR_VALIDACION", mensaje, errores.Count > 0 ? errores : null),
                    context.HttpContext.TraceIdentifier);

                return new BadRequestObjectResult(respuesta);
            };
        });

        services.AddApiVersioning(opts =>
        {
            opts.DefaultApiVersion = new ApiVersion(1, 0);
            opts.AssumeDefaultVersionWhenUnspecified = true;
            opts.ReportApiVersions = true;
            opts.ApiVersionReader = new UrlSegmentApiVersionReader();
        })
        .AddApiExplorer(opts =>
        {
            opts.GroupNameFormat = "'v'VVV";
            opts.SubstituteApiVersionInUrl = true;
        });

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title       = "PideServicio API",
                Version     = "v1",
                Description =
                    "API REST para la gestión de incidencias, solicitudes y riesgos empresariales.\n\n" +
                    "**Autenticación:** JWT de Supabase Auth. Incluir el token en el header: " +
                    "`Authorization: Bearer {token}`"
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description  = "JWT de Supabase Auth. Formato: 'Bearer {token}'",
                Name         = "Authorization",
                In           = ParameterLocation.Header,
                Type         = SecuritySchemeType.Http,
                Scheme       = "bearer",
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id   = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // Agrupa operaciones por el atributo [Tags] en vez del nombre del controller
            c.TagActionsBy(api =>
                [api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] ?? "General"]);
            c.DocInclusionPredicate((_, _) => true);

            // Muestra enums como strings en la documentación
            c.UseInlineDefinitionsForEnums();
        });

        // ---------------------------------------------------------------------------
        // Políticas de autorización por rol.
        //
        // El rol se resuelve contra la BASE DE DATOS (RolDesdeBaseDeDatosHandler), no
        // contra el claim "rol" del token: ese claim solo existe si el hook de Supabase
        // llegó a enriquecer el JWT, y cuando falta, usuarios con rol válido en la BD
        // recibían 403. La BD es la fuente de verdad, igual que en los handlers CQRS.
        // ---------------------------------------------------------------------------
        // Scoped, no Singleton: depende de ICurrentUserService e IUsuarioRepository, que
        // viven por petición. Registrarlo como Singleton provocaría una dependencia
        // cautiva y el contenedor fallaría al validar el arranque.
        services.AddScoped<IAuthorizationHandler, RolDesdeBaseDeDatosHandler>();

        services.AddAuthorization(opts =>
        {
            opts.AddPolicy("Autenticado", p =>
                p.RequireAuthenticatedUser());

            opts.AddPolicy("SoloSuperAdmin", p =>
                p.RequireAuthenticatedUser()
                 .AddRequirements(new RolMinimoRequirement(RolTipo.SUPERADMIN)));

            opts.AddPolicy("AdminOSuperior", p =>
                p.RequireAuthenticatedUser()
                 .AddRequirements(new RolMinimoRequirement(RolTipo.SUPERADMIN, RolTipo.ADMIN)));

            opts.AddPolicy("SupervisorOSuperior", p =>
                p.RequireAuthenticatedUser()
                 .AddRequirements(new RolMinimoRequirement(
                     RolTipo.SUPERADMIN, RolTipo.ADMIN, RolTipo.SUPERVISOR)));

            opts.AddPolicy("Tecnico", p =>
                p.RequireAuthenticatedUser()
                 .AddRequirements(new RolMinimoRequirement(
                     RolTipo.SUPERADMIN, RolTipo.ADMIN, RolTipo.SUPERVISOR, RolTipo.TECNICO)));
        });

        // ---------------------------------------------------------------------------
        // Rate Limiting (ventana fija — 100 req/min por usuario o IP)
        // Implementación built-in .NET 8; no requiere paquete adicional.
        // ---------------------------------------------------------------------------
        services.AddRateLimiter(opts =>
        {
            opts.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: ctx.User?.FindFirst("sub")?.Value
                                  ?? ctx.Connection.RemoteIpAddress?.ToString()
                                  ?? "anonimo",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit          = 100,
                        Window               = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit           = 0
                    }));

            opts.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = 429;
                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    exitoso = false,
                    error   = new
                    {
                        codigo  = "LIMITE_SOLICITUDES",
                        mensaje = "Demasiadas solicitudes. Intenta de nuevo en un minuto."
                    }
                }, token);
            };
        });

        var origenesPermitidos = configuration
            .GetSection("Cors:OrigenesPermitidos")
            .Get<string[]>() ?? [];

        services.AddCors(opts =>
        {
            opts.AddPolicy("PideServicioPolicy", policy =>
                policy.WithOrigins(origenesPermitidos)
                      .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
                      .WithHeaders("Authorization", "Content-Type", "X-Correlation-Id", "X-Trace-Id", "X-Sucursal-Activa")
                      .AllowCredentials());
        });

        // ---------------------------------------------------------------------------
        // Response Compression (Brotli primero, Gzip de respaldo)
        // Habilitado también para HTTPS porque el API está detrás de un reverse proxy
        // que ya termina TLS. El middleware comprime JSON, texto y SVG.
        // ---------------------------------------------------------------------------
        services.AddResponseCompression(opts =>
        {
            opts.EnableForHttps = true;
            opts.Providers.Add<BrotliCompressionProvider>();
            opts.Providers.Add<GzipCompressionProvider>();
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
            [
                "application/json",
                "application/problem+json",
                "image/svg+xml",
            ]);
        });
        services.Configure<BrotliCompressionProviderOptions>(opts =>
            opts.Level = CompressionLevel.Fastest);
        services.Configure<GzipCompressionProviderOptions>(opts =>
            opts.Level = CompressionLevel.Fastest);

        services.AddHealthChecks()
            .AddHealthCheckExtensions(configuration);

        return services;
    }
}
