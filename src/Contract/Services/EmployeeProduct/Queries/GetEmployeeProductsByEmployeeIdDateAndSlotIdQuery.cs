using Contract.Abstractions.Messages;
using Contract.Services.EmployeeProduct.ShareDto;

namespace Contract.Services.EmployeeProduct.Queries;

public record GetEmployeeProductsByEmployeeIdDateAndSlotIdQuery
    (
    GetEmployeeProductsByEmployeeIdDateAndSlotIdRequest getRequest,
    string RoleName,
    string UserIdClaim,
    Guid CompanyIdClaim) : IQuery<List<EmployeeProductResponse>>;


