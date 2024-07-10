using Contract.Services.ProductPhaseSalary.ShareDtos;

namespace Contract.Services.Product.SharedDto;

public record ProductResponse(
    Guid Id,
    string Name,
    string Code,
    decimal Price,
    string Size,
    string Description,
    bool IsInProcessing,
    List<ImageResponse> ImageResponses);

public record ProductWithSalaryResponse(
    Guid Id,
    string Name,
    string Code,
    decimal PriceFinished,
    List<ProductPhaseSalaryResponse> ProductPhaseSalaries,
    string Size,
    string Description,
    bool IsInProcessing,
    List<ImageResponse> ImageResponses);