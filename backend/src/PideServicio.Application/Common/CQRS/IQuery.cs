namespace PideServicio.Application.Common.CQRS;

using MediatR;
using PideServicio.Application.Common.Models;

public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }
