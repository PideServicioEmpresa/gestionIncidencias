namespace PideServicio.Application.Features.Empresas.Queries.ListEmpresas;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Empresas.DTOs;
using PideServicio.Domain.Enums;

public sealed class ListEmpresasQueryHandler : IQueryHandler<ListEmpresasQuery, PagedResult<EmpresaResumenDto>>
{
    private readonly IEmpresaRepository _empresaRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;

    public ListEmpresasQueryHandler(
        IEmpresaRepository empresaRepo,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUser)
    {
        _empresaRepo = empresaRepo;
        _usuarioRepository = usuarioRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<EmpresaResumenDto>>> Handle(ListEmpresasQuery request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<PagedResult<EmpresaResumenDto>>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<PagedResult<EmpresaResumenDto>>();

        if (actorDb.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido<PagedResult<EmpresaResumenDto>>("No tiene permisos para listar empresas.");

        // SuperAdmin ve todas las empresas; otros roles solo ven la suya.
        if (actorDb.Rol != RolTipo.SUPERADMIN)
        {
            var solaEmpresa = await _empresaRepo.ObtenerPorIdAsync(actorDb.EmpresaId, ct);
            if (solaEmpresa is null)
                return Result.NoEncontrado<PagedResult<EmpresaResumenDto>>("Empresa", actorDb.EmpresaId);

            var soloMia = new PagedResult<EmpresaResumenDto>
            {
                Items = [solaEmpresa.Adapt<EmpresaResumenDto>()],
                Pagina = 1,
                TamanoPagina = 1,
                TotalRegistros = 1
            };
            return Result.Exito(soloMia);
        }

        var resultado = await _empresaRepo.ListarAsync(
            request.Pagina, request.TamanoPagina, request.SoloActivas, request.Busqueda, ct);
        var dtos = resultado.Items.Adapt<List<EmpresaResumenDto>>();

        var paged = new PagedResult<EmpresaResumenDto>
        {
            Items = dtos,
            Pagina = resultado.Pagina,
            TamanoPagina = resultado.TamanoPagina,
            TotalRegistros = resultado.TotalRegistros
        };

        return Result.Exito(paged);
    }
}
