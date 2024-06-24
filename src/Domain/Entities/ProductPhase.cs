using Contract.Services.ProductPhase.Creates;
using Contract.Services.ProductPhase.Updates;

namespace Domain.Entities;

public class ProductPhase
{
    public Guid ProductId { get; set; }
    public Guid PhaseId { get; set; }
    public int Quantity { get; set; }
    public decimal SalaryPerProduct { get; set; }
    public Guid CompanyId { get; set; }
    public Company Company { get; set; }
    public Product Product { get; set; }
    public Phase Phase { get; set; }

    public static ProductPhase Create(CreateProductPhaseRequest request)
    {
        return new ProductPhase
        {
            ProductId = request.ProductId,
            PhaseId = request.PhaseId,
            Quantity = request.Quantity,
            SalaryPerProduct = request.SalaryPerProduct
        };
    }

    public void Update(UpdateProductPhaseRequest request)
    {
        Quantity = request.Quantity;
        SalaryPerProduct = request.SalaryPerProduct;
    }

}
