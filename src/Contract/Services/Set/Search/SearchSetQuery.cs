using Contract.Abstractions.Messages;
using Contract.Services.Set.GetSet;
using Contract.Services.Set.GetSets;

namespace Contract.Services.Set.Search;

public record SearchSetQuery(string SearchTerm) : IQuery<List<SetResponse>>;
