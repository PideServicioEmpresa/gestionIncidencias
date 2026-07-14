namespace PideServicio.Application.Features.Usuarios.Queries.GetUsuarioById;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Features.Usuarios.DTOs;

public sealed record GetUsuarioByIdQuery(Guid Id) : IQuery<UsuarioDto>;
