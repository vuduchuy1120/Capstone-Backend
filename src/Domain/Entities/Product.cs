﻿using Contract.Services.Product.CreateProduct;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class Product : EntityAuditBase<Guid>
{
    public string Name { get; private set; }
    public string Code { get; private set; }
    public decimal Price { get; private set; }
    public string Size { get; private set; }
    public string Description { get; private set; }
    public bool IsGroup { get; private set; }
    public bool IsInProcessing { get; private set; }
    public List<ProductImage> Images { get; private set; }
    public List<ProductUnit> ProductUnits { get; private set; }
    public List<ProductUnit> SubProductUnits { get; private set; }
    public List<ProductPharse> ProductPharses { get; private set; }
    private Product()
    {
    }

    public static Product Create(CreateProductRequest request, string createdBy)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Code = request.Code,
            Size = request.Size,
            Description = request.Description,
            Price = request.Price,
            IsGroup = request.IsGroup,
            IsInProcessing = true,
            CreatedBy = createdBy,
            CreatedDate = DateTime.UtcNow
        };
    }

    // Cart             ( Id, GroupName, Size )
    // Cart_Detail      ( CartId, ProductId, QuantityPerGroup) 
    // Product          ( Id, Price, Size) 
    // ProductImage     ( Id, ProductId, FileName, IsMainImage, Issdfdsf)
}
