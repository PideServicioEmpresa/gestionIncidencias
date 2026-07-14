namespace PideServicio.Application.Features.Evidencias.Queries.ListEvidencias;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Evidencias.DTOs;
using PideServicio.Domain.Enums;

public sealed class ListEvidenciasQueryHandler
    : IQueryHandler<ListEvidenciasQuery, ListResult<EvidenciaDto>>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly ITicketEvidenciaRepository _evidenciaRepository;

    public ListEvidenciasQueryHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        ITicketRepository ticketRepository,
        ITicketEvidenciaRepository evidenciaRepository)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
        _ticketRepository = ticketRepository;
        _evidenciaRepository = evidenciaRepository;
    }

    public async Task<Result<ListResult<EvidenciaDto>>> Handle(
        ListEvidenciasQuery request,
        CancellationToken cancellationToken)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null)
            return Result.NoAutorizado<ListResult<EvidenciaDto>>();

        var actor = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, cancellationToken)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);
        if (actor is null || !actor.Activo) return Result.NoAutorizado<ListResult<EvidenciaDto>>();

        var ticket = await _ticketRepository.ObtenerPorIdAsync(request.TicketId, cancellationToken);
        if (ticket is null)
            return Result.NoEncontrado<ListResult<EvidenciaDto>>("Ticket", request.TicketId);

        if (actor.Rol != RolTipo.SUPERADMIN && ticket.EmpresaId != actor.EmpresaId)
            return Result.NoPermitido<ListResult<EvidenciaDto>>("No tiene acceso a este ticket.");

        var evidencias = await _evidenciaRepository.ListarPorTicketAsync(
            request.TicketId,
            request.Tipo,
            cancellationToken);

        var dtos = evidencias.Adapt<List<EvidenciaDto>>();

        return Result.Exito(ListResult<EvidenciaDto>.Crear(dtos));
    }
}
