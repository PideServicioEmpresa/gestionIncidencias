namespace PideServicio.Application.Features.Empresas.Commands.ToggleEmpresaActiva;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class ToggleEmpresaActivaCommandHandler : ICommandHandler<ToggleEmpresaActivaCommand, Guid>
{
    private readonly IEmpresaRepository _empresaRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _auditService;

    public ToggleEmpresaActivaCommandHandler(
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

    public async Task<Result<Guid>> Handle(ToggleEmpresaActivaCommand request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<Guid>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<Guid>();

        if (actorDb.Rol != RolTipo.SUPERADMIN)
            return Result.NoPermitido<Guid>("Solo SuperAdministradores pueden activar o desactivar empresas.");

        var empresa = await _empresaRepo.ObtenerPorIdAsync(request.Id, ct);
        if (empresa is null)
            return Result.NoEncontrado<Guid>("Empresa", request.Id);

        var estadoAnterior = empresa.Activa;

        try
        {
            if (empresa.Activa)
                empresa.Desactivar(actorDb.Id);
            else
                empresa.Activar(actorDb.Id);

            await _empresaRepo.ActualizarAsync(empresa, ct);
            await _auditService.RegistrarAsync(
                "empresas",
                empresa.Id,
                empresa.Activa ? "ACTIVADO" : "DESACTIVADO",
                new { Activa = estadoAnterior },
                new { empresa.Activa },
                ct);

            return Result.Exito(empresa.Id);
        }
        catch (DomainException ex)
        {
            return Result.Fallo<Guid>(ex.Message);
        }
    }
}
