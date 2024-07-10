using Contract.Abstractions.Messages;
using Contract.Services.Set.GetSets;

namespace Contract.Services.Set.GetSet;

public record GetSetQuery(Guid setId) : IQuery<SetResponse>;