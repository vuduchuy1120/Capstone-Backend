using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class ProductImage : EntityBase<Guid>
{
    public string ImageUrl { get; set; }
    public bool IsBluePrint { get; set; }
    public bool IsMainImage { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; } 
}
