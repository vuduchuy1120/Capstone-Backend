using Contract.Abstractions.Shared.Results;
using MediatR;

namespace Contract.Abstractions.Messages;

public interface IQueryHandler<TResponse> : IRequest<Result.Success<TResponse>>
{
}