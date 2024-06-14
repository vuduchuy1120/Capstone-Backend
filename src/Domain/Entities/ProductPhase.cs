namespace Domain.Entities;

public class ProductPhase
{
    public Guid ProductId { get; set; }
    public Guid PhaseId { get; set; }
    public int Quantity { get; set; }
    public decimal SalaryPerProduct { get; set; }
    public Product Product { get; set; }
    public Phase Phase { get; set; }
}
