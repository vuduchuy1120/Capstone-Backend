using Contract.Services.Product.SharedDto;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class ProductImage : EntityBase<Guid>
{
    public string ImageUrl { get; private set; }
    public bool IsBluePrint { get; private set; }
    public bool IsMainImage { get; private set; }
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; }
    private ProductImage()
    {
    }
    public static ProductImage Create(Guid productId, ImageRequest request)
    {
        return new()
        {
            ProductId = productId,
            ImageUrl = request.ImageUrl,
            IsBluePrint = request.IsBluePrint,
            IsMainImage = request.IsMainImage,
        };
    }
}
