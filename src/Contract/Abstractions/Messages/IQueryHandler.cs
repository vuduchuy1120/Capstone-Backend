using Contract.Abstractions.Shared.Results;
using MediatR;

namespace Contract.Abstractions.Messages;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result.Success<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
