namespace PideServicio.Application.Features.Evidencias.Commands.SubirEvidencia;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Evidencias.DTOs;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class SubirEvidenciaCommandHandler
    : ICommandHandler<SubirEvidenciaCommand, SubirEvidenciaResponse>
{
    private const string Bucket = "tickets-evidence";

    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly ITicketEvidenciaRepository _evidenciaRepository;
    private readonly ITicketHistorialRepository _historialRepository;
    private readonly IStorageService _storageService;

    public SubirEvidenciaCommandHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        ITicketRepository ticketRepository,
        ITicketEvidenciaRepository evidenciaRepository,
        ITicketHistorialRepository historialRepository,
        IStorageService storageService)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
        _ticketRepository = ticketRepository;
        _evidenciaRepository = evidenciaRepository;
        _historialRepository = historialRepository;
        _storageService = storageService;
    }

    public async Task<Result<SubirEvidenciaResponse>> Handle(
        SubirEvidenciaCommand request,
        CancellationToken cancellationToken)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null)
            return Result.NoAutorizado<SubirEvidenciaResponse>();

        var actor = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, cancellationToken)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);
        if (actor is null || !actor.Activo) return Result.NoAutorizado<SubirEvidenciaResponse>();

        var ticket = await _ticketRepository.ObtenerPorIdAsync(request.TicketId, cancellationToken);
        if (ticket is null)
            return Result.NoEncontrado<SubirEvidenciaResponse>("Ticket", request.TicketId);

        if (actor.Rol != RolTipo.SUPERADMIN && ticket.EmpresaId != actor.EmpresaId)
            return Result.NoPermitido<SubirEvidenciaResponse>("No tiene acceso a este ticket.");

        var esTecnicoAsignado = ticket.TecnicoId == actor.Id;
        var esSolicitante = ticket.SolicitanteId == actor.Id;
        var tieneAccesoAdministrativo = actor.Rol is RolTipo.ADMIN or RolTipo.SUPERADMIN or RolTipo.SUPERVISOR;
        if (!esTecnicoAsignado && !esSolicitante && !tieneAccesoAdministrativo)
            return Result.NoPermitido<SubirEvidenciaResponse>(
                "Solo el técnico asignado, el solicitante o un administrador puede subir evidencias.");

        try
        {
            // Path.GetFileName elimina componentes de directorio (../, /) previniendo path traversal.
            var nombreSeguro = Path.GetFileName(request.NombreOriginal);
            var ruta = $"tickets/{request.TicketId}/{Guid.NewGuid()}-{nombreSeguro}";

            string url;
            try
            {
                url = await _storageService.SubirAsync(
                    Bucket,
                    ruta,
                    request.Contenido,
                    request.TipoMime,
                    cancellationToken);
            }
            catch (HttpRequestException ex)
            {
                return Result.Fallo<SubirEvidenciaResponse>(
                    $"Error al subir el archivo al almacenamiento: {ex.Message}. Verifica la configuración del bucket.");
            }

            var evidencia = TicketEvidencia.Crear(
                request.TicketId,
                actor.Id,
                request.Tipo,
                nombreSeguro,
                request.TipoMime,
                request.TamanoBytes,
                url);

            var id = await _evidenciaRepository.CrearAsync(evidencia, cancellationToken);

            var entrada = TicketHistorialEntrada.Crear(
                ticketId: request.TicketId,
                tipoEvento: TipoEventoHistorialTipo.EVIDENCIA_SUBIDA,
                actorId: actor.Id);

            await _historialRepository.CrearAsync(entrada, cancellationToken);

            return Result.Exito(new SubirEvidenciaResponse(id, url));
        }
        catch (DomainException ex)
        {
            return Result.Fallo<SubirEvidenciaResponse>(ex.Message);
        }
    }
}
