using AutoMapper;
using Contract.Services.Product.SharedDto;
using Contract.Services.ProductPhaseSalary.ShareDtos;
using Domain.Entities;

namespace Application.Mappers;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductResponse>()
            .ConstructUsing(src => new ProductResponse(
                src.Id,
                src.Name,
                src.Code,
                src.Price,
                src.ProductPhaseSalaries.Select(salary => new ProductPhaseSalaryResponse(
                    salary.PhaseId,
                    salary.Phase.Name,
                    salary.SalaryPerProduct
                    )).ToList(),
                src.Size,
                src.Description,
                src.IsInProcessing,
                src.Images.Select(image => new ImageResponse(
                    image.Id,
                    image.ImageUrl,
                    image.IsBluePrint,
                    image.IsMainImage)).ToList()
            ));

        CreateMap<ProductImage, ImageResponse>();
    }
}
