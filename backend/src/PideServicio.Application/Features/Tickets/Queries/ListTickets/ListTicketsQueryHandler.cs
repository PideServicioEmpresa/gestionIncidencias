namespace PideServicio.Application.Features.Tickets.Queries.ListTickets;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Tickets.DTOs;
using PideServicio.Domain.Enums;

public sealed class ListTicketsQueryHandler : IQueryHandler<ListTicketsQuery, PagedResult<TicketResumenDto>>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITicketRepository _ticketRepo;

    public ListTicketsQueryHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        ITicketRepository ticketRepo)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
        _ticketRepo = ticketRepo;
    }

    public async Task<Result<PagedResult<TicketResumenDto>>> Handle(
        ListTicketsQuery request,
        CancellationToken cancellationToken)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null)
            return Result.NoAutorizado<PagedResult<TicketResumenDto>>();

        var actor = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, cancellationToken)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);
        if (actor is null || !actor.Activo) return Result.NoAutorizado<PagedResult<TicketResumenDto>>();

        Guid? empresaId = null;
        Guid? tecnicoId = request.TecnicoId;
        Guid? solicitanteId = request.SolicitanteId;

        if (actor.Rol == RolTipo.SUPERADMIN)
        {
            // SuperAdmin ve todos los tickets; usa los filtros tal como llegan
        }
        else if (actor.Rol is RolTipo.ADMIN or RolTipo.SUPERVISOR)
        {
            // Admin/Supervisor ve todos los tickets de su empresa
            empresaId = actor.EmpresaId;
        }
        else if (actor.Rol == RolTipo.TECNICO)
        {
            // Técnico ve solo sus tickets asignados
            tecnicoId = actor.Id;
            solicitanteId = null;
        }
        else
        {
            // Trabajador/Usuario ve solo los tickets que creó
            solicitanteId = actor.Id;
            tecnicoId = null;
        }

        var filtros = new TicketConsultaParams(
            EmpresaId: empresaId,
            SucursalId: request.SucursalId,
            AreaId: request.AreaId,
            TecnicoId: tecnicoId,
            SolicitanteId: solicitanteId,
            Estado: request.Estado,
            Prioridad: request.Prioridad,
            FechaDesde: request.FechaDesde,
            FechaHasta: request.FechaHasta,
            BusquedaTexto: request.BusquedaTexto);

        var paginado = await _ticketRepo.ListarAsync(filtros, request.Pagina, request.TamanoPagina, cancellationToken);

        var resultado = new PagedResult<TicketResumenDto>
        {
            Items = paginado.Items.Select(t => new TicketResumenDto(
                Id: t.Id,
                Codigo: t.Codigo,
                Titulo: t.Titulo,
                Estado: t.Estado,
                PrioridadEfectiva: t.PrioridadEfectiva,
                EmpresaId: t.EmpresaId,
                SucursalId: t.SucursalId,
                SucursalNombre: t.SucursalNombre,
                AreaId: t.AreaId,
                AreaNombre: t.AreaNombre,
                TipoServicioId: t.TipoServicioId,
                Tipo: t.TipoServicioNombre,
                TipoServicioNombre: t.TipoServicioNombre,
                CategoriaId: t.CategoriaId,
                SolicitanteId: t.SolicitanteId,
                SolicitanteNombre: t.SolicitanteNombre,
                TecnicoId: t.TecnicoId,
                AsignadoANombre: t.AsignadoANombre,
                FechaCreacion: t.FechaCreacion,
                FechaLimiteResolucion: t.FechaLimiteResolucion
            )).ToList(),
            Pagina = paginado.Pagina,
            TamanoPagina = paginado.TamanoPagina,
            TotalRegistros = paginado.TotalRegistros
        };

        return Result.Exito(resultado);
    }
}
