using Contract.Abstractions.Shared.Utils;
using Contract.Services.EmployeeProduct.Creates;
using Domain.Abstractions.Entities;
using Domain.Exceptions.Common;

namespace Domain.Entities;

public class EmployeeProduct : EntityAuditBaseWithoutId
{
    public Guid ProductId { get; set; }
    public string UserId { get; set; }
    public Guid PhaseId { get; set; }
    public int SlotId { get; set; }
    public DateOnly Date { get; set; }
    public int Quantity { get; set; }
    public bool IsMold { get; set; } = false;
    public User? User { get; set; }
    public Product? Product { get; set; }
    public Phase? Phase { get; set; }
    public Slot? Slot { get; set; }

    public static EmployeeProduct Create(CreateQuantityProductRequest createQuantityProductRequest, int slotId, string Date, string CreatedBy)
    {
        return new EmployeeProduct
        {
            ProductId = createQuantityProductRequest.ProductId,
            UserId = createQuantityProductRequest.UserId,
            PhaseId = createQuantityProductRequest.PhaseId,
            SlotId = slotId,
            Date = ConvertStringToDateTimeOnly(Date),
            Quantity = createQuantityProductRequest.Quantity,
            CreatedBy = CreatedBy,
            CreatedDate = DateUtils.GetNow(),
            IsMold = createQuantityProductRequest.IsMold,
        };
    }
    private static DateOnly ConvertStringToDateTimeOnly(string dateString)
    {
        string format = "dd/MM/yyyy";

        DateTime dateTime;
        if (DateTime.TryParseExact(dateString, format, null, System.Globalization.DateTimeStyles.None, out dateTime))
        {
            return new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);
        }
        else
        {
            throw new WrongFormatDateException();
        }
    }
}
