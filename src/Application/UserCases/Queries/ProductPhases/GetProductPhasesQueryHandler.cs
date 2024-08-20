using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Abstractions.Shared.Search;
using Contract.Services.ProductPhase.Queries;
using Contract.Services.ProductPhase.ShareDto;
using Domain.Exceptions.ProductPhases;

namespace Application.UserCases.Queries.ProductPhases;

public sealed class GetProductPhasesQueryHandler
    (IProductPhaseRepository _productPhaseRepository,
    IMapper _mapper
    ) : IQueryHandler<GetProductPhasesQuery, SearchResponse<List<ProductPhaseResponse>>>
{
    public async Task<Result.Success<SearchResponse<List<ProductPhaseResponse>>>> Handle(GetProductPhasesQuery request, CancellationToken cancellationToken)
    {
        var searchResult = await _productPhaseRepository.GetProductPhases(request);

        var productPhases = searchResult.Item1;
        var totalPage = searchResult.Item2;

        if (productPhases is null || productPhases.Count <= 0 || totalPage <= 0)
        {
            throw new ProductPhaseNotFoundException();
        }

        var data = productPhases.ConvertAll(pphase => _mapper.Map<ProductPhaseResponse>(pphase));

        var searchResponse = new SearchResponse<List<ProductPhaseResponse>>(request.PageIndex, totalPage, data);

        return Result.Success<SearchResponse<List<ProductPhaseResponse>>>.Get(searchResponse);

    }
}
