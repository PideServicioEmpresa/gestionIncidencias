namespace PideServicio.Application.Features.CorreosGuardados.Commands.AgregarCorreoGuardado;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.CorreosGuardados.DTOs;

public sealed class AgregarCorreoGuardadoCommandHandler
    : ICommandHandler<AgregarCorreoGuardadoCommand, CorreoGuardadoDto>
{
    private const int LimiteMaximo = 20;

    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICorreoGuardadoRepository _correoRepo;

    public AgregarCorreoGuardadoCommandHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        ICorreoGuardadoRepository correoRepo)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
        _correoRepo = correoRepo;
    }

    public async Task<Result<CorreoGuardadoDto>> Handle(
        AgregarCorreoGuardadoCommand request, CancellationToken cancellationToken)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<CorreoGuardadoDto>();

        var actor = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, cancellationToken)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);
        if (actor is null || !actor.Activo)
            return Result.NoAutorizado<CorreoGuardadoDto>();

        var correo = request.Correo.Trim().ToLowerInvariant();
        if (!correo.Contains('@') || correo.Length < 5)
            return Result.ErrorValidacion<CorreoGuardadoDto>("correo", "Formato de correo inválido.");

        var existe = await _correoRepo.ExisteAsync(actor.Id, correo, cancellationToken);
        if (existe)
            return Result.Fallo<CorreoGuardadoDto>("Este correo ya está en tu lista guardada.");

        var total = await _correoRepo.ContarPorUsuarioAsync(actor.Id, cancellationToken);
        if (total >= LimiteMaximo)
            return Result.NoPermitido<CorreoGuardadoDto>(
                $"Has alcanzado el límite de {LimiteMaximo} correos guardados.");

        var dto = await _correoRepo.AgregarAsync(actor.Id, correo, cancellationToken);
        return Result.Exito(dto);
    }
}
