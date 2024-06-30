using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class SalaryPay : EntityAuditBase<Guid>
{
    public string UserId { get; private set; }
    public decimal Salary {  get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int Status { get; private set; }
    public User User { get; private set; }
}
