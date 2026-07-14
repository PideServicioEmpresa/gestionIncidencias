namespace PideServicio.Application.Common.CQRS;

using MediatR;
using PideServicio.Application.Common.Models;

public interface ICommand : IRequest<Result> { }

public interface ICommand<TResponse> : IRequest<Result<TResponse>> { }
