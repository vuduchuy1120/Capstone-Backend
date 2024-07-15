namespace Domain.Entities;

public class ProductPhaseSalary
{
    public Guid ProductId { get; private set; }
    public Guid PhaseId { get; private set; }
    public decimal SalaryPerProduct { get; private set; }
    public Product Product { get; private set; }
    public Phase Phase { get; private set; }

    public static ProductPhaseSalary Create(Guid productId, Guid phaseId, decimal salaryPerProduct)
    {
        return new()
        {
            ProductId = productId,
            PhaseId = phaseId,
            SalaryPerProduct = salaryPerProduct
        };
    }

    public void Update(Guid productId, Guid phaseId, decimal salaryPerProduct)
    {
        ProductId = productId;
        PhaseId = phaseId;
        SalaryPerProduct = salaryPerProduct;        
    }

}
