namespace PideServicio.Application.Features.Categorias.Commands.DesactivarCategoria;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Enums;

public sealed class DesactivarCategoriaCommandHandler : ICommandHandler<DesactivarCategoriaCommand, Guid>
{
    private readonly ICategoriaRepository _categoriaRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _auditService;

    public DesactivarCategoriaCommandHandler(
        ICategoriaRepository categoriaRepo,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUser,
        IAuditService auditService)
    {
        _categoriaRepo = categoriaRepo;
        _usuarioRepository = usuarioRepository;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    public async Task<Result<Guid>> Handle(DesactivarCategoriaCommand request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<Guid>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<Guid>();

        if (actorDb.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido<Guid>("Solo Administradores o SuperAdministradores pueden desactivar categorías.");

        var cat = await _categoriaRepo.ObtenerPorIdAsync(request.Id, ct);
        if (cat is null) return Result.NoEncontrado<Guid>("Categoría", request.Id);

        if (cat.EsGlobal && actorDb.Rol != RolTipo.SUPERADMIN)
            return Result.NoPermitido<Guid>("Solo los SuperAdministradores pueden desactivar categorías globales.");

        if (!cat.EsGlobal && actorDb.Rol == RolTipo.ADMIN && cat.EmpresaId != actorDb.EmpresaId)
            return Result.NoPermitido<Guid>("No tiene permisos para modificar esta categoría.");

        if (!cat.Activa) return Result.Fallo<Guid>("La categoría ya está inactiva.");

        cat.Desactivar(actorDb.Id);
        await _categoriaRepo.ActualizarAsync(cat, ct);
        await _auditService.RegistrarAsync("categorias", cat.Id, "DESACTIVADO",
            new { Activa = true }, new { Activa = false }, ct);
        return Result.Exito(cat.Id);
    }
}
