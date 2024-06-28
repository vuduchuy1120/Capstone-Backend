using Contract.Abstractions.Messages;
using Contract.Services.Set.GetSet;

namespace Contract.Services.Set.Search;

public record SearchSetQuery(string SearchTerm) : IQuery<List<SetResponse>>;
