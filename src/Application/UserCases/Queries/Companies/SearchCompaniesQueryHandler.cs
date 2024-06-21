using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Company.Queries;
using Contract.Services.Company.ShareDtos;
using Contract.Services.User.SharedDto;
using Domain.Exceptions.Companies;

namespace Application.UserCases.Queries.Companies;

public sealed class SearchCompaniesQueryHandler
    (ICompanyRepository _companyRepository,
    IMapper _mapper
    ) : IQueryHandler<SearchCompanyQuery, SearchResponse<List<CompanyResponse>>>
{
    public async Task<Result.Success<SearchResponse<List<CompanyResponse>>>> Handle(SearchCompanyQuery request, CancellationToken cancellationToken)
    {        
        var searchResult = await _companyRepository.SearchCompanyAsync(request);
        var companies = searchResult.Item1;
        var totalPage = searchResult.Item2;
        if (companies is null)
        {
            throw new CompanyNotFoundException();
        }
        var data = companies.ConvertAll(_mapper.Map<CompanyResponse>);
        var searchResponse = new SearchResponse<List<CompanyResponse>>(request.PageIndex, totalPage, data);
        return Result.Success<SearchResponse<List<CompanyResponse>>>.Get(searchResponse);

    }
}
