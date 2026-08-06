namespace PideServicio.Application.Features.CorreosGuardados.Commands.AgregarCorreoGuardado;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Features.CorreosGuardados.DTOs;

public sealed record AgregarCorreoGuardadoCommand(string Correo) : ICommand<CorreoGuardadoDto>;
