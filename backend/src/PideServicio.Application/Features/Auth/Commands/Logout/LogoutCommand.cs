namespace PideServicio.Application.Features.Auth.Commands.Logout;

using PideServicio.Application.Common.CQRS;

public sealed record LogoutCommand(string AccessToken) : ICommand;
