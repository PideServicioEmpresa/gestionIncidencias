namespace PideServicio.Application.Features.Empresas.Commands.AgregarCorreoCopiaEmpresa;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Enums;

public sealed class AgregarCorreoCopiaEmpresaCommandHandler : ICommandHandler<AgregarCorreoCopiaEmpresaCommand, Guid>
{
    private readonly IEmpresaCorreoCopiaRepository _repo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _auditService;

    public AgregarCorreoCopiaEmpresaCommandHandler(
        IEmpresaCorreoCopiaRepository repo,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUser,
        IAuditService auditService)
    {
        _repo = repo;
        _usuarioRepository = usuarioRepository;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    public async Task<Result<Guid>> Handle(AgregarCorreoCopiaEmpresaCommand request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<Guid>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<Guid>();

        if (actorDb.Rol == RolTipo.ADMIN)
        {
            if (actorDb.EmpresaId != request.EmpresaId)
                return Result.NoPermitido<Guid>("Solo puede gestionar correos en copia de su propia empresa.");
        }
        else if (actorDb.Rol != RolTipo.SUPERADMIN)
        {
            return Result.NoPermitido<Guid>("No tiene permisos para agregar correos en copia.");
        }

        if (await _repo.ExisteAsync(request.EmpresaId, request.Correo, ct))
            return Result.Fallo<Guid>("Este correo ya está configurado para esta empresa.");

        var id = await _repo.AgregarAsync(request.EmpresaId, request.Correo, ct);

        await _auditService.RegistrarAsync(
            "empresa_correos_copia",
            id,
            "CREADO",
            null,
            new { request.EmpresaId, request.Correo },
            ct);

        return Result.Exito(id);
    }
}
