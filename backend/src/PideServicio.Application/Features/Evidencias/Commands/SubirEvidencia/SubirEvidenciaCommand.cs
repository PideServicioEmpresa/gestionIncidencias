namespace PideServicio.Application.Features.Evidencias.Commands.SubirEvidencia;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Features.Evidencias.DTOs;
using PideServicio.Domain.Enums;

public sealed record SubirEvidenciaCommand(
    Guid TicketId,
    EvidenciaTipo Tipo,
    string NombreOriginal,
    string TipoMime,
    long TamanoBytes,
    Stream Contenido) : ICommand<SubirEvidenciaResponse>;
