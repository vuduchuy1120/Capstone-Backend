using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Set.GetSet;
using Contract.Services.Set.Search;

namespace Application.UserCases.Queries.Sets.Search;

internal sealed class SearchSetQueryHandler(ISetRepository _setRepository, IMapper _mapper)
    : IQueryHandler<SearchSetQuery, List<SetsWithProductSalaryResponse>>
{
    public async Task<Result.Success<List<SetsWithProductSalaryResponse>>> Handle(SearchSetQuery request, CancellationToken cancellationToken)
    {
        var sets = await _setRepository.SearchSetAsync(request.SearchTerm);

        if(sets is null)
        {
            return Result.Success<List<SetsWithProductSalaryResponse>>.Get(null);
        }

        var data = _mapper.Map<List<SetsWithProductSalaryResponse>>(sets);

        return Result.Success<List<SetsWithProductSalaryResponse>>.Get(data);
    }
}
