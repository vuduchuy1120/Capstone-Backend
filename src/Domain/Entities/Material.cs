using Contract.Abstractions.Shared.Utils;
using Contract.Services.Material.Create;
using Contract.Services.Material.Update;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class Material : EntityBase<Guid>
{
    public string Name { get; private set; }
    public string NameUnaccent { get; private set; }
    public string? Description { get; private set; }
    public string Unit { get; private set; }
    public double? QuantityPerUnit { get; private set; }
    public string? Image { get; private set; }
    public double QuantityInStock { get; private set; }
    public double AvailableQuantity { get; private set; }
    public List<MaterialHistory>? MaterialHistories { get; private set; }
    public List<ShipmentDetail>? ShipmentDetails { get; set; }


    public static Material Create(CreateMaterialRequest createMaterialRequest)
    {
        return new Material
        {
            Id = Guid.NewGuid(),
            Name = createMaterialRequest.Name,
            NameUnaccent = StringUtils.RemoveDiacritics(createMaterialRequest.Name),
            Description = createMaterialRequest.Description,
            Unit = createMaterialRequest.Unit,
            QuantityPerUnit = createMaterialRequest.QuantityPerUnit,
            Image = createMaterialRequest.Image,
            QuantityInStock = createMaterialRequest.QuantityInStock
        };
    }

    public void Update(UpdateMaterialRequest updateMaterialRequest)
    {
        Name = updateMaterialRequest.Name.Trim();
        NameUnaccent = StringUtils.RemoveDiacritics(updateMaterialRequest.Name.Trim()).ToLower();
        Description = updateMaterialRequest.Description.Trim();
        Unit = updateMaterialRequest.Unit.Trim();
        QuantityPerUnit = updateMaterialRequest.QuantityPerUnit;
        Image = updateMaterialRequest.Image;
        QuantityInStock = updateMaterialRequest.QuantityInStock;
    }

    public void UpdateAvailableQuantity(double quantity)
    {
        AvailableQuantity = quantity;
    }

    public void UpdateQuantityInStock(double quantity)
    {
        QuantityInStock = quantity;
    }

    public void UpdateQuantityInStock1(double quantity)
    {
        QuantityInStock += quantity;
    }
}

