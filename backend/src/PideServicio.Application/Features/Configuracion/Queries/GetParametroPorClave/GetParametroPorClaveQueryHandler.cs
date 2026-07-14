namespace PideServicio.Application.Features.Configuracion.Queries.GetParametroPorClave;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Configuracion.DTOs;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class GetParametroPorClaveQueryHandler
    : IQueryHandler<GetParametroPorClaveQuery, ParametroDto>
{
    private readonly IParametroRepository _parametroRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetParametroPorClaveQueryHandler(
        IParametroRepository parametroRepository,
        ICurrentUserService currentUserService)
    {
        _parametroRepository = parametroRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ParametroDto>> Handle(
        GetParametroPorClaveQuery request,
        CancellationToken cancellationToken)
    {
        var usuario = _currentUserService.UsuarioActual;
        if (usuario is null)
            return Result.NoAutorizado<ParametroDto>();

        if (usuario.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido<ParametroDto>(
                "Solo Administradores y SuperAdministradores pueden consultar parámetros de configuración.");

        try
        {
            var empresaId = usuario.EsSuperAdmin ? (Guid?)null : usuario.EmpresaId;

            var parametro = await _parametroRepository.ObtenerPorClaveAsync(
                request.Clave,
                empresaId,
                cancellationToken);

            if (parametro is null)
                return Result.NoEncontrado<ParametroDto>(
                    $"Parámetro con clave '{request.Clave}' no encontrado.");

            return Result.Exito(parametro.Adapt<ParametroDto>());
        }
        catch (DomainException ex)
        {
            return Result.Fallo<ParametroDto>(ex.Message);
        }
    }
}
