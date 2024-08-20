using Contract.Services.ProductPhase.ShareDto;
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

public record ProductWithOneImageResponse(
    Guid Id,
    string Name,
    string Code,
    decimal Price,
    string Size,
    string Description,
    bool IsInProcessing,
    string Image);

public record ProductWithQuantityResponse(
    Guid Id,
    string Name,
    string Code,
    decimal PriceFinished,
    List<ProductPhaseSalaryResponse> ProductPhaseSalaries,
    List<ProductPhaseWithCompanyResponse> ProductPhaseWithCompanyReponses,
    string Size,
    string Description,
    bool IsInProcessing,
    List<ImageResponse> ImageResponses);

public record ProductWithTotalQuantityResponse(
    Guid Id,
    string Name,
    string Code,
    decimal PriceFinished,
    List<ProductPhaseSalaryResponse> ProductPhaseSalaries,
    List<ProductTotalQuantityResponse> ProductTotalQuantityResponses,
    string Size,
    string Description,
    bool IsInProcessing,
    List<ImageResponse> ImageResponses);