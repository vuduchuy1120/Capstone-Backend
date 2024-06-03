namespace Domain.Entities;

public class ProductUnit
{
    public Guid ProductId { get; set; }
    public Guid SubProductId { get; set; }
    public int QuantityPerUnit { get; set; }
    public Product Product { get; set; }
    public Product SubProduct { get; set; }
}
