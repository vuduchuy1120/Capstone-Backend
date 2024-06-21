using Contract.Abstractions.Messages;

namespace Contract.Services.Set.GetSet;

public record GetSetQuery(Guid setId) : IQueryHandler<SetResponse>;