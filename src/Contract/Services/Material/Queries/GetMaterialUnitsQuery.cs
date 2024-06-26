using Contract.Abstractions.Messages;

namespace Contract.Services.Material.Query;

public record GetMaterialUnitsQuery : IQuery<List<string>>;
