using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class Order : EntityAuditBase<Guid>
{
    public string Status { get; set; }
    public Guid CompanyId { get; set; }
    public Company Company { get; set; }
    public List<OrderDetail>? OrderDetails { get; set; }
    public List<ShipOrder>? ShipOrders { get; set; }
}
