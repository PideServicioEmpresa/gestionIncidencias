namespace PideServicio.Infrastructure;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Infrastructure.Authentication;
using PideServicio.Infrastructure.Caching;
using PideServicio.Infrastructure.Notifications;
using PideServicio.Infrastructure.Services;
using PideServicio.Infrastructure.Storage;
using CurrentUserService = PideServicio.Infrastructure.Services.CurrentUserService;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        // Servicios base
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IDateTimeService, DateTimeService>();

        // Opciones de configuración
        services.Configure<SupabaseAuthOptions>(
            configuration.GetSection(SupabaseAuthOptions.SeccionNombre));

        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SeccionNombre));

        services.Configure<StorageOptions>(
            configuration.GetSection(StorageOptions.SeccionNombre));

        services.Configure<NotificationOptions>(
            configuration.GetSection(NotificationOptions.SeccionNombre));

        services.Configure<CacheOptions>(
            configuration.GetSection(CacheOptions.SeccionNombre));

        // Autenticación JWT con ES256 vía OIDC/JWKS (Supabase Cloud)
        // Supabase publica sus claves públicas ECDSA en su endpoint JWKS.
        // La librería las descarga automáticamente al inicio y las cachea.
        var jwtOptions = configuration
            .GetSection(JwtOptions.SeccionNombre)
            .Get<JwtOptions>() ?? new JwtOptions();

        var supabaseOpts = configuration
            .GetSection(SupabaseAuthOptions.SeccionNombre)
            .Get<SupabaseAuthOptions>() ?? new SupabaseAuthOptions();

        var supabaseAuthority = supabaseOpts.Url.TrimEnd('/') + "/auth/v1";

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = supabaseAuthority;
                options.MetadataAddress = supabaseAuthority + "/.well-known/openid-configuration";
                options.RequireHttpsMetadata = true;
                options.Audience = jwtOptions.Audience;
                options.TokenValidationParameters = new()
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidIssuer = supabaseAuthority,
                    ValidateAudience = !string.IsNullOrEmpty(jwtOptions.Audience),
                    ValidAudience = jwtOptions.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    NameClaimType = "sub",
                    RoleClaimType = "rol"
                };
            });

        services.AddAuthorization();

        // Servicios de auditoría y notificaciones
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IPushNotificationService, PushNotificationService>();

        // EmailService — usa Brevo HTTP API en lugar de SMTP; HttpClient gestionado por el factory
        services.AddHttpClient("brevo", client =>
        {
            client.BaseAddress = new Uri("https://api.brevo.com/v3/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        services.AddScoped<IEmailService, EmailService>();

        // Supabase Auth Service — HttpClient tipado para la API de Auth de Supabase
        services.AddHttpClient<SupabaseAuthService>();
        services.AddScoped<ISupabaseAuthService, SupabaseAuthService>();

        // Storage — HttpClient tipado para Supabase Storage REST API
        services.AddHttpClient<SupabaseStorageService>();
        services.AddScoped<IStorageService, SupabaseStorageService>();

        // Caché en memoria (IMemoryCache thread-safe, Singleton apropiado)
        services.AddMemoryCache();
        services.AddSingleton<CacheService>();

        return services;
    }
}
