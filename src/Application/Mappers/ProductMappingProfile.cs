using AutoMapper;
using Contract.Services.Product.SharedDto;
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
