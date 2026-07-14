namespace PideServicio.Application.Features.Categorias.Commands.UpdateCategoria;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class UpdateCategoriaCommandHandler : ICommandHandler<UpdateCategoriaCommand, Guid>
{
    private readonly ICategoriaRepository _categoriaRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _auditService;

    public UpdateCategoriaCommandHandler(
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

    public async Task<Result<Guid>> Handle(UpdateCategoriaCommand request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<Guid>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<Guid>();

        if (actorDb.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido<Guid>("Solo Administradores o SuperAdministradores pueden editar categorías.");

        var cat = await _categoriaRepo.ObtenerPorIdAsync(request.Id, ct);
        if (cat is null) return Result.NoEncontrado<Guid>("Categoría", request.Id);

        if (cat.EsGlobal && actorDb.Rol != RolTipo.SUPERADMIN)
            return Result.NoPermitido<Guid>("Solo los SuperAdministradores pueden editar categorías globales.");

        if (!cat.EsGlobal && actorDb.Rol == RolTipo.ADMIN && cat.EmpresaId != actorDb.EmpresaId)
            return Result.NoPermitido<Guid>("No tiene permisos para editar esta categoría.");

        if (await _categoriaRepo.ExisteNombreAsync(cat.EmpresaId, request.Nombre, request.Id, ct))
            return Result.Fallo<Guid>($"Ya existe una categoría con el nombre '{request.Nombre}' en este alcance.");

        var anterior = new { cat.Nombre, cat.Descripcion };
        try
        {
            cat.Actualizar(request.Nombre, request.Descripcion, actorDb.Id);
            await _categoriaRepo.ActualizarAsync(cat, ct);
            await _auditService.RegistrarAsync("categorias", cat.Id, "ACTUALIZADO", anterior,
                new { cat.Nombre, cat.Descripcion }, ct);
            return Result.Exito(cat.Id);
        }
        catch (ValidationException ex) { return Result.ErrorValidacion<Guid>(ex.Errors); }
        catch (DomainException ex)     { return Result.Fallo<Guid>(ex.Message); }
    }
}
