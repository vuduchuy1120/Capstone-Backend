namespace Domain.Entities;

public class SetProduct
{
    public Guid SetId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public Set Set {  get; set; }
    public Product Product { get; set; }
}
