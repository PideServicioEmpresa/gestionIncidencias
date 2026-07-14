namespace PideServicio.Application.Features.Evidencias.Queries.ListEvidencias;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Evidencias.DTOs;
using PideServicio.Domain.Enums;

public sealed record ListEvidenciasQuery(
    Guid TicketId,
    EvidenciaTipo? Tipo = null) : IQuery<ListResult<EvidenciaDto>>;
