namespace PideServicio.Application.Features.Empresas.Commands.UpdateEmpresa;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class UpdateEmpresaCommandHandler : ICommandHandler<UpdateEmpresaCommand, Guid>
{
    private readonly IEmpresaRepository _empresaRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _auditService;

    public UpdateEmpresaCommandHandler(
        IEmpresaRepository empresaRepo,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUser,
        IAuditService auditService)
    {
        _empresaRepo = empresaRepo;
        _usuarioRepository = usuarioRepository;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    public async Task<Result<Guid>> Handle(UpdateEmpresaCommand request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<Guid>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<Guid>();

        if (actorDb.Rol != RolTipo.SUPERADMIN)
            return Result.NoPermitido<Guid>("Solo SuperAdministradores pueden modificar empresas.");

        var empresa = await _empresaRepo.ObtenerPorIdAsync(request.Id, ct);
        if (empresa is null)
            return Result.NoEncontrado<Guid>("Empresa", request.Id);

        var antes = new
        {
            empresa.NombreComercial,
            empresa.RazonSocial,
            empresa.ZonaHoraria,
            empresa.LogoUrl,
            empresa.ColorPrimario,
            empresa.ColorSecundario
        };

        try
        {
            empresa.Actualizar(
                request.NombreComercial,
                request.RazonSocial,
                request.ZonaHoraria,
                request.LogoUrl,
                request.ColorPrimario,
                request.ColorSecundario,
                actorDb.Id);

            await _empresaRepo.ActualizarAsync(empresa, ct);
            await _auditService.RegistrarAsync("empresas", empresa.Id, "ACTUALIZADO", antes, new
            {
                empresa.NombreComercial,
                empresa.RazonSocial,
                empresa.ZonaHoraria,
                empresa.LogoUrl,
                empresa.ColorPrimario,
                empresa.ColorSecundario
            }, ct);

            return Result.Exito(empresa.Id);
        }
        catch (ValidationException ex)
        {
            return Result.ErrorValidacion<Guid>(ex.Errors);
        }
        catch (DomainException ex)
        {
            return Result.Fallo<Guid>(ex.Message);
        }
    }
}
