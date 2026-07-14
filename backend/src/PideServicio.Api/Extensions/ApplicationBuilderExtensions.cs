namespace PideServicio.Api.Extensions;

using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using PideServicio.Api.Middleware;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseApiPipeline(this WebApplication app)
    {
        // Necesario cuando el API corre detrás de un reverse proxy (Render, AWS ALB, etc.)
        // Debe ir antes de UseHttpsRedirection para que el scheme detectado sea correcto.
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        // A05 — Security Headers: X-Content-Type-Options, X-Frame-Options, Referrer-Policy, Permissions-Policy
        // X-XSS-Protection está deprecated en navegadores modernos pero se mantiene para
        // compatibilidad con proxies y WAFs que aún lo evalúan.
        app.Use(async (context, next) =>
        {
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers["X-Frame-Options"] = "DENY";
            context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
            context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            context.Response.Headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";
            await next();
        });

        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<GlobalExceptionMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PideServicio API v1");
                c.RoutePrefix = "swagger";
            });
        }

        app.UseSerilogRequestLogging(opts =>
        {
            opts.MessageTemplate =
                "HTTP {RequestMethod} {RequestPath} respondió {StatusCode} en {Elapsed:0.0000}ms";
        });

        // Debe ir antes de UseStaticFiles y endpoints para comprimir todas las respuestas.
        app.UseResponseCompression();
        app.UseHttpsRedirection();
        app.UseCors("PideServicioPolicy");
        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        // Endpoint de salud sin autenticación — usado por Render para health checks.
        app.MapGet("/ping", () => Results.Ok(new { status = "ok" })).AllowAnonymous();

        // En produccion debe restringirse tambien a nivel de red/reverse proxy.
        // RequireAuthorization evita que monitoreo externo anonimo detecte el estado de la BD.
        app.MapHealthChecks("/health").RequireAuthorization();

        return app;
    }
}
