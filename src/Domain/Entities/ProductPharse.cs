namespace Domain.Entities;

public class ProductPharse
{
    public Guid ProductId { get; set; }
    public Guid PharseId { get; set; }
    public int Quantity { get; set; }
    public decimal SalaryPerProduct { get; set; }
    public Product Product { get; set; }
    public Pharse Pharse { get; set; }
}
