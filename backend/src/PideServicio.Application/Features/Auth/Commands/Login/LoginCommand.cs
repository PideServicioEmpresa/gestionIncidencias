namespace PideServicio.Application.Features.Auth.Commands.Login;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Features.Auth.DTOs;

public sealed record LoginCommand(string Email, string Password) : ICommand<LoginResponseDto>;
