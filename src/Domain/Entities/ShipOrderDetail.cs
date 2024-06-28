using Contract.Services.ShipOrder.Share;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class ShipOrderDetail : EntityBase<Guid>
{
    public Guid ShipOrderId { get; private set; }
    public Guid? ProductId { get; private set; }
    public Guid? SetId { get; private set; }
    public ShipOrder ShipOrder { get; private set; }
    public Product? Product { get; private set; }
    public Set? Set { get; private set; }
    public int Quantity { get; private set; }
    public ItemStatus ItemStatus { get; private set; }
}
