namespace Domain.Entities;

public class ProductPhaseKey
{
    public Guid ProductId { get; set; }
    public Guid PhaseId { get; set; }
    public Guid CompanyId { get; set; }

    public override bool Equals(object obj)
    {
        if (obj is ProductPhaseKey other)
        {
            return ProductId == other.ProductId && PhaseId == other.PhaseId && CompanyId == other.CompanyId;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ProductId, PhaseId, CompanyId);
    }
}
