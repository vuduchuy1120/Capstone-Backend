using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class SalaryHistory : EntityBase<Guid>
{
    public string UserId { get; private set; }
    public decimal Salary {  get; private set; }
    public DateTime StartDate { get; private set; }
    public int SalaryType { get; private set; }
    public User User { get; private set; }
}
