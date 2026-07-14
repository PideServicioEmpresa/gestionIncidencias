namespace PideServicio.Application.Features.Empresas.Commands.CreateEmpresa;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class CreateEmpresaCommandHandler : ICommandHandler<CreateEmpresaCommand, Guid>
{
    private readonly IEmpresaRepository _empresaRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _auditService;

    public CreateEmpresaCommandHandler(
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

    public async Task<Result<Guid>> Handle(CreateEmpresaCommand request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<Guid>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<Guid>();

        if (actorDb.Rol != RolTipo.SUPERADMIN)
            return Result.NoPermitido<Guid>("Solo SuperAdministradores pueden crear empresas.");

        if (await _empresaRepo.ExisteIdentificacionFiscalAsync(request.IdentificacionFiscal, null, ct))
            return Result.Fallo<Guid>($"Ya existe una empresa con la identificación fiscal '{request.IdentificacionFiscal}'.");

        try
        {
            var empresa = Empresa.Crear(
                request.NombreComercial,
                request.RazonSocial,
                request.IdentificacionFiscal,
                request.ZonaHoraria,
                actorDb.Id);

            if (request.LogoUrl is not null || request.ColorPrimario is not null || request.ColorSecundario is not null)
                empresa.ActualizarDatosVisuales(request.LogoUrl, request.ColorPrimario, request.ColorSecundario, actorDb.Id);

            var id = await _empresaRepo.CrearAsync(empresa, ct);
            await _auditService.RegistrarAsync("empresas", id, "CREADO", null, new
            {
                empresa.NombreComercial,
                empresa.RazonSocial,
                empresa.IdentificacionFiscal,
                empresa.ZonaHoraria
            }, ct);
            return Result.Exito(id);
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
