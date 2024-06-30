using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Company.Shared;
using Contract.Services.Company.ShareDtos;

namespace Contract.Services.Company.Queries;

public record SearchCompanyQuery
(
    string? Name,
    string? Address,
    string? PhoneNumber,
    string? Email,
    string? DirectorName,
    string? CompanyType,
    int PageIndex = 1,
    int PageSize = 10
    ) : IQuery<SearchResponse<List<CompanyResponse>>>;
