namespace Contract.Services.EmployeeProduct.Queries;

public record GetEmployeeProductsByEmployeeIdDateAndSlotIdRequest
    (int SlotId,
    string UserId,
    string Date,
    Guid CompanyId
    );