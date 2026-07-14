namespace PideServicio.Application.Features.Auth.Commands.RefreshToken;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Features.Auth.DTOs;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<RefreshTokenResponseDto>;
