using Contract.Services.OrderDetail.Creates;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class OrderDetail : EntityBase<Guid>
{
    public Guid OrderId { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? SetId { get; set; }
    public Order Order { get; set; }
    public Product? Product { get; set; }
    public Set? Set { get; set; }
    public int Quantity { get; set; }

    public static OrderDetail Create(Guid orderId, OrderDetailRequest request)
    {
        return new OrderDetail
        {
            OrderId = orderId,
            ProductId = request.ProductId != null ? request.ProductId : null,
            SetId = request.SetId != null ? request.SetId : null,
            Quantity = request.Quantity
        };
    }
}
