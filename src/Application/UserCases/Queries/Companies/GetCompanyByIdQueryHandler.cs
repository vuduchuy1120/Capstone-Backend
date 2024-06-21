using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Company.Queries;
using Contract.Services.Company.ShareDtos;
using Domain.Exceptions.Companies;

namespace Application.UserCases.Queries.Companies;

public sealed class GetCompanyByIdQueryHandler
    (ICompanyRepository _companyRepository,
    IMapper _mapper
    ) : IQueryHandler<GetCompanyByIdQuery, CompanyResponse>
{
    public async Task<Result.Success<CompanyResponse>> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByIdAsync(request.id);
        if (company is null)
        {
            throw new CompanyNotFoundException();
        }
        var companyResponse = _mapper.Map<CompanyResponse>(company);
        return Result.Success<CompanyResponse>.Get(companyResponse);

    }
}
