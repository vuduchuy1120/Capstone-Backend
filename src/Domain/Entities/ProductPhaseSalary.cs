namespace Domain.Entities;

public class ProductPhaseSalary
{
    public Guid ProductId { get; private set; }
    public Guid PhaseId { get; private set; }
    public decimal SalaryPerProduct { get; private set; }
    public Product Product { get; private set; }
    public Phase Phase { get; private set; }
}
