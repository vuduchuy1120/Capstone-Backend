using Contract.Abstractions.Messages;
using Contract.Services.EmployeeProduct.ShareDto;

namespace Contract.Services.EmployeeProduct.Queries;

public record GetEmployeeProductsByEmployeeIdDateAndSlotIdQuery
    (int slotId, string userId, string date) : IQueryHandler<List<EmployeeProductResponse>>;


