namespace PideServicio.Application.Features.Auth.Commands.RecuperarContrasena;

using PideServicio.Application.Common.CQRS;

public sealed record RecuperarContrasenaCommand(string Email) : ICommand;
