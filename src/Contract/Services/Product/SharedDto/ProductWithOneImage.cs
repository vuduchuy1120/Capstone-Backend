using Contract.Services.ProductPhase.ShareDto;
using Contract.Services.ProductPhaseSalary.ShareDtos;

namespace Contract.Services.Product.SharedDto;

public record ProductWithOneImage(
    Guid Id,
    string Name,
    string Code,
    decimal Price,
    string Size,
    string Description,
    bool IsInProcessing,
    string ImageUrl);

public record ProductWithQuantityInformation(
    Guid Id,
    string Name,
    string Code,
    decimal Price,
    string Size,
    string Description,
    bool IsInProcessing,
    string ImageUrl,
    Guid PhaseId,
    Guid CompanyId,
    int Quantity,
    int AvailableQuantity,
    int ErrorQuantity,
    int ErrorAvailableQuantity,
    int FailureQuantity,
    int FailureAvailabeQuantity,
    int BrokenQuantity,
    int BrokenAvailableQuantity
    );

public record ProductWithOneImageWithSalary(Guid Id,
    string Name,
    string Code,
    decimal PriceFinished,
    List<ProductPhaseSalaryResponse> ProductPhaseSalaryResponses,
    List<ProductPhaseWithCompanyResponse> ProductPhaseWithCompanyResponses,
    string Size,
    string Description,
    bool IsInProcessing,
    string ImageUrl);
