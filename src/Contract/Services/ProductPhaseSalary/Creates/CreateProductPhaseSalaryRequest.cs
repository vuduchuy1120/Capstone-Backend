namespace Contract.Services.ProductPhaseSalary.Creates;

public record CreateProductPhaseSalaryRequest
(
    Guid ProductId,
    Guid PhaseId,
    decimal SalaryPerProduct
);
