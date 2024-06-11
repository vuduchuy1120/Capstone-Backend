using AutoMapper;
using Contract.Services.Product.SharedDto;
using Contract.Services.Set.GetSet;
using Contract.Services.Set.GetSets;
using Contract.Services.Set.SharedDto;
using Domain.Entities;

namespace Application.Mappers
{
    public class SetMappingProfile : Profile
    {
        public SetMappingProfile()
        {
            CreateMap<Set, SetResponse>()
                .ConstructUsing(src => new SetResponse(
                    src.Id,
                    src.Code,
                    src.Name,
                    src.ImageUrl,
                    src.Description,
                    new List<SetProductResponse>() // Initialize with an empty list
                ))
                .ForMember(dest => dest.SetProducts, opt => opt.MapFrom(src => src.SetProducts != null ? src.SetProducts.Select(sp => new SetProductResponse(
                    sp.SetId,
                    sp.ProductId,
                    sp.Quantity,
                    sp.Product != null ? new ProductResponse(
                        sp.Product.Id,
                        sp.Product.Name,
                        sp.Product.Code,
                        sp.Product.Price,
                        sp.Product.Size,
                        sp.Product.Description,
                        sp.Product.IsInProcessing,
                        sp.Product.Images != null ? sp.Product.Images.Select(pi => new ImageResponse(
                            pi.Id,
                            pi.ImageUrl,
                            pi.IsBluePrint,
                            pi.IsMainImage
                        )).ToList() : new List<ImageResponse>()
                    ) : null
                )).ToList() : new List<SetProductResponse>()));

            CreateMap<Set, SetsResponse>();
        }
    }
}
