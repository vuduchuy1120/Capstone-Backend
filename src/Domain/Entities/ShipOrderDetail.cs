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

    public static ShipOrderDetail CreateShipProductOrder(Guid productId, Guid shipOrderId, int quantity)
    {
        return new ShipOrderDetail()
        {
            ShipOrderId = shipOrderId,
            ProductId = productId,
            Quantity = quantity,
            SetId = null
        };
    }

    public static ShipOrderDetail CreateShipSetOrder(Guid setId, Guid shipOrderId, int quantity)
    {
        return new ShipOrderDetail()
        {
            ShipOrderId = shipOrderId,
            ProductId = null,
            Quantity = quantity,
            SetId = setId
        };
    }
}
