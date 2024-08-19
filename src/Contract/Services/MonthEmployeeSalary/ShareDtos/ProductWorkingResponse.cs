namespace Contract.Services.MonthEmployeeSalary.ShareDtos;

public record ProductWorkingResponse
(
    Guid ProductId,
    string ProductName,
    string ProductCode,
    string ProductImage,
    Guid PhaseId,
    string PhaseName,
    string PhaseDescription,
    int Quantity,
    decimal SalaryPerProduct
    );