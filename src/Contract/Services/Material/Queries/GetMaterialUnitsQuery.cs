using Contract.Abstractions.Messages;

namespace Contract.Services.Material.Query;

public record GetMaterialUnitsQuery : IQueryHandler<List<string>>;
