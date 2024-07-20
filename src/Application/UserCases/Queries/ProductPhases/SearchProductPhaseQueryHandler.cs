using Application.Abstractions.Data;
using Application.Abstractions.Services;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Abstractions.Shared.Search;
using Contract.Services.ProductPhase.Queries;
using Contract.Services.ProductPhase.ShareDto;
using Domain.Entities;

namespace Application.UserCases.Queries.ProductPhases;

public sealed class SearchProductPhaseQueryHandler
    (IProductPhaseRepository _productPhaseRepository,
    ICloudStorage _cloudStorage
    ) : IQueryHandler<SearchProductPhaseQuery, SearchResponse<List<SearchProductPhaseResponse>>>
{
    public async Task<Result.Success<SearchResponse<List<SearchProductPhaseResponse>>>> Handle(SearchProductPhaseQuery request, CancellationToken cancellationToken)
    {
        var searchResult = await _productPhaseRepository.SearchProductPhase(request);

        var productPhases = searchResult.Item1;
        var totalPage = searchResult.Item2;

        var data = new List<SearchProductPhaseResponse>();

        foreach (var pphase in productPhases)
        {

            var response = new SearchProductPhaseResponse
            (
                CompanyId: pphase.CompanyId,
                CompanyName: pphase.Company.Name,
                ProductId: pphase.ProductId,
                ProductName: pphase.Product.Name,
                ProductCode: pphase.Product.Code,
                ImageUrl: await _cloudStorage.GetSignedUrlAsync(pphase.Product.Images.FirstOrDefault()?.ImageUrl ?? string.Empty),
                PhaseId: pphase.PhaseId,
                PhaseName: pphase.Phase.Name,
                PhaseDescription: pphase.Phase.Description,
                ErrorAvailableQuantity: pphase.ErrorAvailableQuantity,
                AvailableQuantity: pphase.AvailableQuantity,
                BrokenAvailableQuantity: pphase.BrokenAvailableQuantity,
                FailureAvailabeQuantity: pphase.FailureAvailabeQuantity
            );

            data.Add(response);
        }

        var searchResponse = new SearchResponse<List<SearchProductPhaseResponse>>(request.PageIndex, totalPage, data);

        return Result.Success<SearchResponse<List<SearchProductPhaseResponse>>>.Get(searchResponse);
    }
}
