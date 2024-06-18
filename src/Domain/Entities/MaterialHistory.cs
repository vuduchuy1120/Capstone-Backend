using Contract.Services.MaterialHistory.Create;
using Contract.Services.MaterialHistory.Update;
using Domain.Abstractions.Entities;
using Domain.Abstractions.Exceptions;
using static System.Net.Mime.MediaTypeNames;

namespace Domain.Entities;

public class MaterialHistory : EntityBase<Guid>
{
    public int MaterialId { get; private set; }
    public double Quantity { get; private set; }
    public double? QuantityPerUnit { get; private set; }
    public decimal Price { get; private set; }
    public string? Description { get; private set; }
    public double? QuantityInStock { get; private set; }
    public DateOnly ImportDate { get; private set; }
    public Material? Material { get; private set; }
    public List<ShipmentDetail>? ShipmentDetails { get; set; }

    public static MaterialHistory Create(CreateMaterialHistoryRequest createMaterialHistoryRequest)
    {
        return new MaterialHistory
        {
            MaterialId = createMaterialHistoryRequest.MaterialId,
            Quantity = createMaterialHistoryRequest.Quantity,
            QuantityPerUnit = createMaterialHistoryRequest.QuantityPerUnit,
            Price = createMaterialHistoryRequest.Price,
            Description = createMaterialHistoryRequest.Description,
            QuantityInStock = createMaterialHistoryRequest.Quantity,
            ImportDate = ConvertStringToDateTimeOnly(createMaterialHistoryRequest.ImportDate)
        };
    }
    public void Update(UpdateMaterialHistoryRequest updateMaterialHistoryRequest)
    {
        Quantity = updateMaterialHistoryRequest.Quantity;
        QuantityPerUnit = updateMaterialHistoryRequest.QuantityPerUnit;
        Price = updateMaterialHistoryRequest.Price;
        Description = updateMaterialHistoryRequest.Description;
        ImportDate = ConvertStringToDateTimeOnly(updateMaterialHistoryRequest.ImportDate);
        QuantityInStock = updateMaterialHistoryRequest.QuantityInStock;
    }
    public static DateOnly ConvertStringToDateTimeOnly(string dateString)
    {
        string format = "dd/MM/yyyy";

        DateTime dateTime;
        if (DateTime.TryParseExact(dateString, format, null, System.Globalization.DateTimeStyles.None, out dateTime))
        {
            return new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);
        }
        else
        {
            throw new MyValidationException("Date is wrong format");
        }
    }

}
