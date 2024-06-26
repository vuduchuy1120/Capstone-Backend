namespace Contract.Services.EmployeeProduct.ShareDto;

public record EmployeeProductResponse
(
    string ImageUrl,
    string ProductName,
    Guid ProductId,
    Guid PhaseId,
    string PhaseName,
    int Quantity
    );