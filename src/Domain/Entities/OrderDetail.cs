using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class OrderDetail : EntityBase<Guid>
{
    public Guid? OrderId { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? SetId { get; set; }
    public Order? Order { get; set; }
    public Product? Product { get; set; }
    public Set? Set { get; set; }
    public int Quantity { get; set; }
}
