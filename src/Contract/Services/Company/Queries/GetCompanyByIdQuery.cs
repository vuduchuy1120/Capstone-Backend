using Contract.Abstractions.Messages;
using Contract.Services.Company.ShareDtos;

namespace Contract.Services.Company.Queries;

public record GetCompanyByIdQuery(Guid id) : IQueryHandler<CompanyResponse>;
