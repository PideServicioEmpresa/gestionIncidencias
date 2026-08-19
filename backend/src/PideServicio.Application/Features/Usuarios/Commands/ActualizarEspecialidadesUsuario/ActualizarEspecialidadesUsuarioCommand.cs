namespace PideServicio.Application.Features.Usuarios.Commands.ActualizarEspecialidadesUsuario;

using PideServicio.Application.Common.CQRS;

public sealed record ActualizarEspecialidadesUsuarioCommand(
    Guid UsuarioId,
    IReadOnlyList<Guid> Especialidades) : ICommand;
