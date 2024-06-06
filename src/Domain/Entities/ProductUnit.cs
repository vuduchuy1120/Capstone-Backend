namespace Domain.Entities;

public class ProductUnit
{
    public Guid ProductId { get; private set; }
    public Guid SubProductId { get; private set; }
    public int QuantityPerUnit { get; private set; }
    public Product Product { get; private set; }
    public Product SubProduct { get; private set; }
    private ProductUnit()
    {
    }

    public static ProductUnit Create(Guid ProductGroupId, Guid SubProductId, int QuantityPerUnit)
    {
        return new ProductUnit()
        {
            ProductId = ProductGroupId,
            SubProductId = SubProductId,
            QuantityPerUnit = QuantityPerUnit
        };
    }

    public void Update(int quantityPerUnit)
    {
        QuantityPerUnit = quantityPerUnit;
    }
}
