namespace PideServicio.Application.Features.Configuracion.Commands.UpdateParametro;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class UpdateParametroCommandHandler
    : ICommandHandler<UpdateParametroCommand>
{
    private readonly IParametroRepository _parametroRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;

    public UpdateParametroCommandHandler(
        IParametroRepository parametroRepository,
        ICurrentUserService currentUserService,
        IAuditService auditService)
    {
        _parametroRepository = parametroRepository;
        _currentUserService = currentUserService;
        _auditService = auditService;
    }

    public async Task<Result> Handle(
        UpdateParametroCommand request,
        CancellationToken cancellationToken)
    {
        var usuario = _currentUserService.UsuarioActual;
        if (usuario is null)
            return Result.NoAutorizado();

        if (usuario.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido("Solo Administradores y SuperAdministradores pueden actualizar parámetros.");

        // Solo SuperAdmin puede actualizar parámetros globales (EmpresaId null).
        if (request.EmpresaId is null && !usuario.EsSuperAdmin)
            return Result.NoPermitido("Solo el SuperAdministrador puede modificar parámetros globales.");

        try
        {
            var empresaIdBusqueda = request.EmpresaId ?? (usuario.EsSuperAdmin ? null : usuario.EmpresaId);

            var parametro = await _parametroRepository.ObtenerPorClaveAsync(
                request.Clave,
                empresaIdBusqueda,
                cancellationToken);

            string valorAnterior;

            if (parametro is null)
            {
                // La clave no existe aún: se crea con los valores proporcionados.
                // El tipo de dato se infiere del valor; descripción queda vacía hasta que
                // un administrador la enriquezca mediante otro endpoint.
                parametro = Parametro.CrearNuevo(
                    request.Clave,
                    request.NuevoValor,
                    empresaIdBusqueda);

                valorAnterior = string.Empty;
            }
            else
            {
                // Guardar valor anterior para auditoría.
                valorAnterior = parametro.Valor;
                parametro.ActualizarValor(request.NuevoValor, usuario.Id);
            }

            // UPSERT: inserta si es nuevo, actualiza si ya existía.
            await _parametroRepository.UpsertAsync(parametro, cancellationToken);

            await _auditService.RegistrarAsync(
                "parametros",
                parametro.Id,
                string.IsNullOrEmpty(valorAnterior) ? "CREAR" : "ACTUALIZAR",
                new { Valor = valorAnterior },
                new { parametro.Valor },
                cancellationToken);

            return Result.Exito();
        }
        catch (DomainException ex)
        {
            return Result.Fallo(ex.Message);
        }
    }
}
