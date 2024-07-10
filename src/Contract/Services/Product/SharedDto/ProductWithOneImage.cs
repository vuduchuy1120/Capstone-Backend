using Contract.Services.ProductPhaseSalary.ShareDtos;

namespace Contract.Services.Product.SharedDto;

public record ProductWithOneImage(Guid Id,
    string Name,
    string Code,
    decimal PriceFinished,
    List<ProductPhaseSalaryResponse> ProductPhaseSalaryResponses,
    string Size,
    string Description,
    bool IsInProcessing,
    string ImageUrl);
