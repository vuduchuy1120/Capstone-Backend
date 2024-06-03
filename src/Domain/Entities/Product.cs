using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class Product : EntityAuditBase<Guid>
{
    public string Name { get; set; }
    public string Code { get; set; }
    public decimal Price { get; set; }
    public string Size { get; set; }
    public string Description { get; set; }
    public bool IsGroup { get; set; }
    public bool IsInProcessing { get; set; }
    public List<ProductImage> Images { get; set; }
    public List<ProductUnit> ProductUnits { get; set; }
    public List<ProductUnit> SubProductUnits { get; set; }
    public List<ProductPharse> ProductPharses { get; set; }
}
