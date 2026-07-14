namespace PideServicio.Application.Features.Auth.Commands.CambiarContrasena;

using PideServicio.Application.Common.CQRS;

public sealed record CambiarContrasenaCommand(
    string AccessToken,
    string NuevaContrasena,
    string ConfirmacionContrasena) : ICommand;
