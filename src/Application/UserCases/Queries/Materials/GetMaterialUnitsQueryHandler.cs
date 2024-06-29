using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Material.Query;

namespace Application.UserCases.Queries.Materials;

public sealed class GetMaterialUnitsQueryHandler(IMaterialRepository _materialRepository) : IQueryHandler<GetMaterialUnitsQuery, List<string>>
{
    public async Task<Result.Success<List<string>>> Handle(GetMaterialUnitsQuery request, CancellationToken cancellationToken)
    {
        var units = await _materialRepository.GetMaterialUnitsAsync();
        return Result.Success<List<string>>.Get(units);
    }
}
