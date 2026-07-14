namespace PideServicio.Application.Features.Usuarios.Commands.CambiarEstadoLaboral;

using PideServicio.Application.Common.CQRS;
using PideServicio.Domain.Enums;

public sealed record CambiarEstadoLaboralCommand(
    Guid UsuarioId,
    EstadoLaboralTipo NuevoEstado) : ICommand;
