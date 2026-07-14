namespace PideServicio.Application.Common.CQRS;

using MediatR;
using PideServicio.Application.Common.Models;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
