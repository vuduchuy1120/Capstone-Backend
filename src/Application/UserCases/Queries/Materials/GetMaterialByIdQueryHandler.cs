using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Material.Query;
using Contract.Services.Material.ShareDto;
using Domain.Exceptions.Materials;

namespace Application.UserCases.Queries.Materials;
public sealed class GetMaterialByIdQueryHandler(
    IMaterialRepository _materialRepository,
    IMapper _mapper
    ) : IQueryHandler<GetMaterialByIdQuery, MaterialResponse>
{
    public async Task<Result.Success<MaterialResponse>> Handle(GetMaterialByIdQuery request, CancellationToken cancellationToken)
    {
        var material = await _materialRepository.GetMaterialByIdAsync(request.Id);
        if (material is null)
        {
            throw new MaterialNotFoundException();
        }
        return Result.Success<MaterialResponse>.Get(_mapper.Map<MaterialResponse>(material));
    }
}
