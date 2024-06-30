using Contract.Services.ProductPhase.Creates;
using Contract.Services.ProductPhase.Updates;

namespace Domain.Entities;

public class ProductPhase
{
    public Guid ProductId { get; private set; }
    public Guid PhaseId { get; private set; }
    public int Quantity { get; set; } = 0;
    public int ErrorQuantity { get; private set; } = 0;
    public int ErrorAvailableQuantity { get; private set; } = 0;
    public int AvailableQuantity { get; set; } = 0;
    public int FailureQuantity { get; private set; } = 0;
    public int BrokenQuantity { get; private set; } = 0;
    public Guid CompanyId { get; private set; }
    public Company Company { get; private set; }
    public Product Product { get; private set; }
    public Phase Phase { get; private set; }

    public static ProductPhase Create(CreateProductPhaseRequest request)
    {
        return new ProductPhase
        {
            ProductId = request.ProductId,
            PhaseId = request.PhaseId,
            Quantity = request.Quantity,
            AvailableQuantity = request.Quantity,
            CompanyId = request.CompanyId
        };
    }

    public void Update(UpdateProductPhaseRequest request)
    {
        Quantity = request.Quantity;
        AvailableQuantity = request.Quantity;
        //SalaryPerProduct = request.SalaryPerProduct;
    }

    public void UpdateAvailableQuantity(int quantity)
    {
        AvailableQuantity = quantity;
    }

    public void UpdateQuantity(int quantity)
    {
        Quantity = quantity;
    }

    public void UpdateErrorQuantity(int quantity)
    {
        ErrorQuantity = quantity;
    }

    public void UpdateFailureQuantity(int quantity)
    {
        FailureQuantity = quantity;
    }

}
