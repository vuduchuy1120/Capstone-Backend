using Contract.Services.Product.CreateProduct;
using Contract.Services.Product.UpdateProduct;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class Product : EntityAuditBase<Guid>
{
    public string Name { get; private set; }
    public string Code { get; private set; }
    public decimal Price { get; private set; }
    public string Size { get; private set; }
    public string? Description { get; private set; }
    public bool IsInProcessing { get; private set; }
    public List<ProductImage>? Images { get; private set; }
    public List<SetProduct>? SetProducts { get; private set; }
    public List<ProductPhase>? ProductPhases { get; private set; }
    public List<OrderDetail>? OrderDetails { get; set; }
    public List<ShipmentDetail>? ShipmentDetails { get; set; }
    public List<EmployeeProduct>? EmployeeProducts { get; private set; }
    public List<ProductPhaseSalary>? ProductPhaseSalaries { get; set; }

    public static Product Create(CreateProductRequest request, string createdBy)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Code = request.Code,
            Size = request.Size,
            Description = request.Description,
            Price = request.PriceFinished,
            IsInProcessing = true,
            CreatedBy = createdBy,
            CreatedDate = DateTime.UtcNow
        };
    }

    public void Update(UpdateProductRequest request, string updatedBy)
    {
        Code = request.Code;
        Name = request.Name;
        Price = request.PriceFinished;
        Size = request.Size;
        Description = request.Description;
        IsInProcessing = request.IsInProcessing;
        UpdatedBy = updatedBy;
        UpdatedDate = DateTime.UtcNow;
    }
}
