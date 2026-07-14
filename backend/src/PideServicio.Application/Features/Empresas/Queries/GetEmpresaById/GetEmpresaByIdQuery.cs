namespace PideServicio.Application.Features.Empresas.Queries.GetEmpresaById;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Features.Empresas.DTOs;

public sealed record GetEmpresaByIdQuery(Guid Id) : IQuery<EmpresaDto>;
