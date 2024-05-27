using Contract.Abstractions.Shared.Results;
using MediatR;

namespace Contract.Abstractions.Messages;

public interface IQuery<TResponse> : IRequest<Result.Success<TResponse>>
{
}