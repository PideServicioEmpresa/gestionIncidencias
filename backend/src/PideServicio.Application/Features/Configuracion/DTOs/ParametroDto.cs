namespace PideServicio.Application.Features.Configuracion.DTOs;

public sealed record ParametroDto(
    Guid Id,
    Guid? EmpresaId,
    string Clave,
    string Valor,
    string TipoDato,
    string? Descripcion,
    DateTimeOffset UpdatedAt);
