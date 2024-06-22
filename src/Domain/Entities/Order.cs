using Contract.Abstractions.Shared.Utils;
using Contract.Services.Order.Creates;
using Contract.Services.Order.Updates;
using Domain.Abstractions.Entities;
using Domain.Exceptions.Common;

namespace Domain.Entities;

public class Order : EntityAuditBase<Guid>
{
    public Guid CompanyId { get; set; }
    public string Status { get; set; }
    public DateOnly? StartOrder { get; set; }
    public DateOnly? EndOrder { get; set; }
    public Company Company { get; set; }    
    public List<OrderDetail>? OrderDetails { get; set; }
    public List<ShipOrder>? ShipOrders { get; set; }

    public static Order Create(CreateOrderRequest request)
    {
        return new Order
        {
            Id = Guid.NewGuid(),
            CompanyId = request.CompanyId,
            Status = request.Status,
            StartOrder = ConvertStringToDateTimeOnly(request.StartOrder),
            EndOrder = ConvertStringToDateTimeOnly(request.EndOrder)
        };
    }
    public void Update(UpdateOrderRequest request)
    {
        CompanyId = request.CompanyId;
        Status = request.Status;
        StartOrder = ConvertStringToDateTimeOnly(request.StartOrder);
        EndOrder = ConvertStringToDateTimeOnly(request.EndOrder);
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
