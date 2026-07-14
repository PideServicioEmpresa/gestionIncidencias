namespace PideServicio.Application.Features.Categorias.Commands.CreateCategoria;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class CreateCategoriaCommandHandler : ICommandHandler<CreateCategoriaCommand, Guid>
{
    private readonly ICategoriaRepository _categoriaRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _auditService;

    public CreateCategoriaCommandHandler(
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

    public async Task<Result<Guid>> Handle(CreateCategoriaCommand request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<Guid>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<Guid>();

        if (actorDb.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido<Guid>("Solo Administradores o SuperAdministradores pueden crear categorías.");

        Guid? empresaId;
        if (actorDb.Rol == RolTipo.SUPERADMIN)
            empresaId = request.EmpresaId; // null = global
        else
            empresaId = actorDb.EmpresaId; // ADMIN siempre asocia a su empresa

        if (await _categoriaRepo.ExisteNombreAsync(empresaId, request.Nombre, null, ct))
            return Result.Fallo<Guid>($"Ya existe una categoría con el nombre '{request.Nombre}' en este alcance.");

        try
        {
            var cat = Categoria.Crear(request.Nombre, empresaId, request.Descripcion, actorDb.Id);
            var id = await _categoriaRepo.CrearAsync(cat, ct);
            await _auditService.RegistrarAsync("categorias", id, "CREADO", null,
                new { cat.Nombre, cat.EmpresaId, cat.EsGlobal }, ct);
            return Result.Exito(id);
        }
        catch (ValidationException ex) { return Result.ErrorValidacion<Guid>(ex.Errors); }
        catch (DomainException ex)     { return Result.Fallo<Guid>(ex.Message); }
    }
}
