using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Set.GetSets;
using Domain.Entities;
using Domain.Exceptions.Sets;

namespace Application.UserCases.Queries.Sets.GetSets;

internal sealed class GetSetsQueryHandler(
    ISetRepository _setRepository,
    IMapper _mapper) : IQueryHandler<GetSetsQuery, SearchResponse<List<SetsResponse>>>
{
    public async Task<Result.Success<SearchResponse<List<SetsResponse>>>> Handle(
        GetSetsQuery request, 
        CancellationToken cancellationToken)
    {
        var result = await _setRepository.SearchSetAsync(request);

        var sets = result.Item1;
        var totalPages = result.Item2;

        if (sets is null || sets.Count <= 0 || totalPages <= 0)
        {
            throw new SetNotFoundException();
        }

        var data = _mapper.Map<List<SetsResponse>>(sets);

        var searchResponse = new SearchResponse<List<SetsResponse>>(request.PageIndex, totalPages, data);

        return Result.Success<SearchResponse<List<SetsResponse>>>.Get(searchResponse);
    }
}
