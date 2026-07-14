namespace PideServicio.Infrastructure.Authentication;

using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using PideServicio.Application.Common.Interfaces;

public sealed class SupabaseAuthService : ISupabaseAuthService
{
    private readonly HttpClient _http;
    private readonly SupabaseAuthOptions _opts;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public SupabaseAuthService(HttpClient http, IOptions<SupabaseAuthOptions> opts)
    {
        _http = http;
        _opts = opts.Value;
        _http.BaseAddress = new Uri(_opts.Url.TrimEnd('/') + "/auth/v1/");
        _http.DefaultRequestHeaders.Add("apikey", _opts.AnonKey);
    }

    public async Task<SupabaseAuthResult> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync(
            "token?grant_type=password",
            new { email, password },
            ct);

        return await ParseAuthResponseAsync(response, ct);
    }

    public async Task<SupabaseAuthResult> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync(
            "token?grant_type=refresh_token",
            new { refresh_token = refreshToken },
            ct);

        return await ParseAuthResponseAsync(response, ct);
    }

    public async Task LogoutAsync(string accessToken, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "logout");
        request.Headers.Add("Authorization", $"Bearer {accessToken}");

        var response = await _http.SendAsync(request, ct);

        // 204 o 200 son éxito; cualquier otro es error
        if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NoContent)
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException(ExtraerMensajeError(body));
        }
    }

    public async Task CambiarContrasenaAsync(string accessToken, string nuevaContrasena, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, "user");
        request.Headers.Add("Authorization", $"Bearer {accessToken}");
        request.Content = JsonContent.Create(new { password = nuevaContrasena });

        var response = await _http.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            var mensaje = ExtraerMensajeError(body);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new InvalidOperationException(mensaje);
            throw new Exception(mensaje);
        }
    }

    public async Task RecuperarContrasenaAsync(string email, CancellationToken ct = default)
    {
        await _http.PostAsJsonAsync("recover", new { email }, ct);
        // No se valida la respuesta — la ausencia del usuario no debe revelarse
    }

    // ── Admin API (ServiceRoleKey requerido) ──────────────────────────────────

    public async Task<Guid> CrearUsuarioEnAuthAsync(string email, string contrasena, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "admin/users");
        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _opts.ServiceRoleKey);
        request.Content = JsonContent.Create(new
        {
            email,
            password      = contrasena,
            email_confirm = true
        });

        var response = await _http.SendAsync(request, ct);
        var body = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(
                $"Error al crear usuario en Supabase Auth: {ExtraerMensajeError(body)}");

        using var doc = JsonDocument.Parse(body);
        var idStr = doc.RootElement.TryGetProperty("id", out var idElem) ? idElem.GetString() : null;
        if (!Guid.TryParse(idStr, out var authId))
            throw new InvalidOperationException("Supabase Auth no devolvió un ID de usuario válido.");

        return authId;
    }

    public async Task EliminarUsuarioDeAuthAsync(Guid authId, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"admin/users/{authId}");
        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _opts.ServiceRoleKey);

        // Best-effort: se ignoran errores de red; el rollback no debe bloquear al cliente
        try { await _http.SendAsync(request, ct); } catch { /* ignorado intencionalmente */ }
    }

    // ── Parseo de respuesta de token ──────────────────────────────────────────

    private static async Task<SupabaseAuthResult> ParseAuthResponseAsync(HttpResponseMessage response, CancellationToken ct)
    {
        var body = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            var mensaje = ExtraerMensajeError(body);
            if (response.StatusCode is System.Net.HttpStatusCode.BadRequest
                                    or System.Net.HttpStatusCode.Unauthorized
                                    or System.Net.HttpStatusCode.Forbidden)
                throw new InvalidOperationException(mensaje);
            throw new HttpRequestException(mensaje);
        }

        var json = JsonSerializer.Deserialize<SupabaseTokenResponse>(body, JsonOpts)
            ?? throw new HttpRequestException("Respuesta de autenticación inválida.");

        if (string.IsNullOrWhiteSpace(json.AccessToken))
            throw new HttpRequestException("El servidor de autenticación no devolvió un token.");

        if (!Guid.TryParse(json.User?.Id, out var supabaseUserId))
            throw new HttpRequestException("El servidor de autenticación no devolvió un identificador válido.");

        return new SupabaseAuthResult(
            AccessToken: json.AccessToken,
            RefreshToken: json.RefreshToken ?? string.Empty,
            ExpiresIn: json.ExpiresIn,
            SupabaseUserId: supabaseUserId,
            Email: json.User?.Email ?? string.Empty);
    }

    private static string ExtraerMensajeError(string body)
    {
        try
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            // Supabase devuelve { "error": "...", "error_description": "..." }
            // o { "message": "..." }
            if (root.TryGetProperty("error_description", out var desc) && desc.ValueKind == JsonValueKind.String)
                return desc.GetString()!;

            if (root.TryGetProperty("error", out var err) && err.ValueKind == JsonValueKind.String)
                return err.GetString()!;

            if (root.TryGetProperty("message", out var msg) && msg.ValueKind == JsonValueKind.String)
                return msg.GetString()!;
        }
        catch { }

        return "Error de autenticación.";
    }

    // ── Modelos de respuesta de Supabase Auth ─────────────────────────────────

    private sealed class SupabaseTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("user")]
        public SupabaseUser? User { get; set; }
    }

    private sealed class SupabaseUser
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string? Email { get; set; }
    }
}
