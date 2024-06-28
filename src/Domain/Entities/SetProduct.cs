namespace Domain.Entities;

public class SetProduct
{
    public Guid SetId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Set Set {  get; private set; }
    public Product Product { get; private set; }
    public static SetProduct Create(Guid setId, Guid productId, int quantity)
    {
        return new()
        {
            SetId = setId,
            ProductId = productId,
            Quantity = quantity
        };
    }

    public void Update(int  quantity)
    {
        Quantity = quantity;
    }
}
