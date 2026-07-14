namespace PideServicio.Application.Features.Configuracion.Queries.GetParametros;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Configuracion.DTOs;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class GetParametrosQueryHandler
    : IQueryHandler<GetParametrosQuery, IReadOnlyList<ParametroDto>>
{
    private readonly IParametroRepository _parametroRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetParametrosQueryHandler(
        IParametroRepository parametroRepository,
        ICurrentUserService currentUserService)
    {
        _parametroRepository = parametroRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<IReadOnlyList<ParametroDto>>> Handle(
        GetParametrosQuery request,
        CancellationToken cancellationToken)
    {
        var usuario = _currentUserService.UsuarioActual;
        if (usuario is null)
            return Result.NoAutorizado<IReadOnlyList<ParametroDto>>();

        if (usuario.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido<IReadOnlyList<ParametroDto>>(
                "Solo Administradores y SuperAdministradores pueden consultar parámetros de configuración.");

        try
        {
            // SuperAdmin ve parámetros globales (sin empresa); Admin ve los de su empresa.
            var empresaId = usuario.EsSuperAdmin ? (Guid?)null : usuario.EmpresaId;

            var parametros = await _parametroRepository.ListarPorEmpresaAsync(
                empresaId,
                cancellationToken);

            var dtos = parametros.Adapt<IReadOnlyList<ParametroDto>>();
            return Result.Exito(dtos);
        }
        catch (DomainException ex)
        {
            return Result.Fallo<IReadOnlyList<ParametroDto>>(ex.Message);
        }
    }
}
