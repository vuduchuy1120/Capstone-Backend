using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Company.Queries;
using Contract.Services.Company.ShareDtos;

namespace Application.UserCases.Queries.Companies;

public class GetCompaniesByCompanyTypeQueryHandler
    (ICompanyRepository _companyRepository,
    IMapper _mapper) : IQueryHandler<GetCompaniesByCompanyTypeQuery, List<CompanyResponse>>
{
    public async Task<Result.Success<List<CompanyResponse>>> Handle(GetCompaniesByCompanyTypeQuery request, CancellationToken cancellationToken)
    {
        var queryResult = await _companyRepository.GetCompanyFactory(request.CompanyType);
        var result = _mapper.Map<List<CompanyResponse>>(queryResult);
        return Result.Success<List<CompanyResponse>>.Get(result);
    }
}
