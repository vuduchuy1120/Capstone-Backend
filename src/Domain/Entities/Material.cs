using Contract.Abstractions.Shared.Utils;
using Contract.Services.Material.Create;
using Contract.Services.Material.Update;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class Material : EntityBase<int>
{
    public string Name { get; private set; }
    public string NameUnaccent { get; private set; }
    public string? Description { get; private set; }
    public string Unit { get; private set; }
    public double? QuantityPerUnit { get; private set; }
    public string? Image { get; private set; }
    public double QuantityInStock { get; private set; }
    public List<MaterialHistory>? MaterialHistories { get; private set; }

    public static Material Create(CreateMaterialRequest createMaterialRequest)
    {
        return new Material
        {
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
        Name = updateMaterialRequest.Name;
        NameUnaccent = StringUtils.RemoveDiacritics(updateMaterialRequest.Name).ToLower();
        Description = updateMaterialRequest.Description;
        Unit = updateMaterialRequest.Unit;
        QuantityPerUnit = updateMaterialRequest.QuantityPerUnit;
        Image = updateMaterialRequest.Image;
        QuantityInStock = updateMaterialRequest.QuantityInStock;
    }
}

