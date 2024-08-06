using Contract.Services.OrderDetail.Creates;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class OrderDetail : EntityBase<Guid>
{
    public Guid OrderId { get; private set; }
    public Guid? ProductId { get; private set; }
    public Guid? SetId { get; private set; }
    public Order Order { get; private set; }
    public Product? Product { get; private set; }
    public Set? Set { get; private set; }
    public int Quantity { get; private set; }
    public int ShippedQuantity { get; private set; } = 0;
    public decimal UnitPrice { get; private set; }
    public string? Note { get; private set; }

    public static OrderDetail Create(Guid orderId, int shippedQuantity, OrderDetailRequest request)
    {
        return new OrderDetail
        {
            OrderId = orderId,
            ProductId = request.isProductId ? request.ProductIdOrSetId : null,
            SetId = request.isProductId ? null : request.ProductIdOrSetId,
            Quantity = request.Quantity,
            UnitPrice = request.UnitPrice,
            Note = request.Note.Trim(),
            ShippedQuantity = shippedQuantity
        };
    }

    public void UpdateShippedQuantity(int shippedQuantity)
    {
        ShippedQuantity = shippedQuantity;
    }

}
