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
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetParametrosQueryHandler(
        IParametroRepository parametroRepository,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUserService)
    {
        _parametroRepository = parametroRepository;
        _usuarioRepository = usuarioRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<IReadOnlyList<ParametroDto>>> Handle(
        GetParametrosQuery request,
        CancellationToken cancellationToken)
    {
        var claims = _currentUserService.UsuarioActual;
        if (claims is null)
            return Result.NoAutorizado<IReadOnlyList<ParametroDto>>();

        // El rol ya lo validó la política AdminOSuperior contra la BD. Aquí se recarga el
        // usuario porque el ÁMBITO de los datos depende de su rol y empresa reales: con
        // los valores del claim sin enriquecer, un SuperAdmin recibiría los parámetros de
        // una empresa vacía en lugar de los globales.
        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, cancellationToken)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);
        if (actorDb is null || !actorDb.Activo)
            return Result.NoAutorizado<IReadOnlyList<ParametroDto>>();

        try
        {
            // SuperAdmin ve parámetros globales (sin empresa); Admin ve los de su empresa.
            var empresaId = actorDb.Rol == RolTipo.SUPERADMIN ? (Guid?)null : actorDb.EmpresaId;

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
