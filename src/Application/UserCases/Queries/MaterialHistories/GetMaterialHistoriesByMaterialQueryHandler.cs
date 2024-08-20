using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Company.ShareDtos;
using Contract.Services.MaterialHistory.Queries;
using Contract.Services.MaterialHistory.ShareDto;
using Domain.Entities;
using Domain.Exceptions.MaterialHistories;

namespace Application.UserCases.Queries.MaterialHistories;

public sealed class GetMaterialHistoriesByMaterialQueryHandler(
    IMaterialHistoryRepository _materialHistoryRepository,
    IMapper _mapper
    ) : IQueryHandler<GetMaterialHistoriesByMaterialQuery, SearchResponse<List<MaterialHistoryResponse>>>
{
    public async Task<Result.Success<SearchResponse<List<MaterialHistoryResponse>>>> Handle(GetMaterialHistoriesByMaterialQuery request, CancellationToken cancellationToken)
    {
        var searchResult = await _materialHistoryRepository.GetMaterialHistoriesByMaterialNameAndDateAsync(request);

        var materialHistories = searchResult.Item1;
        var totalPage = searchResult.Item2;

        if (materialHistories is null || materialHistories.Count <= 0 || totalPage <= 0)
        {
            throw new MaterialHistoryNotFoundException();
        }


        var result = _mapper.Map<List<MaterialHistoryResponse>>(materialHistories);


        var searchResponse = new SearchResponse<List<MaterialHistoryResponse>>(request.PageIndex, totalPage, result);

        return Result.Success<SearchResponse<List<MaterialHistoryResponse>>>.Get(searchResponse);
    }
}