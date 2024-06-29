using Contract.Abstractions.Shared.Utils;
using Contract.Services.Order.Creates;
using Contract.Services.Order.ShareDtos;
using Contract.Services.Order.Updates;
using Domain.Abstractions.Entities;
using Domain.Exceptions.Common;

namespace Domain.Entities;

public class Order : EntityAuditBase<Guid>
{
    public Guid CompanyId { get; private set; }
    public StatusOrder Status { get; private set; }
    public DateOnly? StartOrder { get; private set; }
    public DateOnly? EndOrder { get; private set; }
    public double VAT { get; private set; }
    public Company Company { get; private set; }
    public List<OrderDetail>? OrderDetails { get; private set; }
    public List<ShipOrder>? ShipOrders { get; private set; }

    public static Order Create(CreateOrderRequest request, string CreatedBy)
    {
        return new Order
        {
            Id = Guid.NewGuid(),
            CompanyId = request.CompanyId,
            Status = request.Status,
            StartOrder = ConvertStringToDateTimeOnly(request.StartOrder),
            EndOrder = ConvertStringToDateTimeOnly(request.EndOrder),
            VAT = request.VAT,
            CreatedBy = CreatedBy,
            CreatedDate = DateTime.UtcNow
        };
    }
    public void Update(UpdateOrderRequest request, string UpdatedBy)
    {
        CompanyId = request.CompanyId;
        Status = request.Status;
        StartOrder = ConvertStringToDateTimeOnly(request.StartOrder);
        EndOrder = ConvertStringToDateTimeOnly(request.EndOrder);
        VAT = request.VAT;
        UpdatedBy = UpdatedBy;
        UpdatedDate = DateTime.UtcNow;
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
