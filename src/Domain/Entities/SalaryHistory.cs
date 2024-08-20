using Contract.Services.SalaryHistory.ShareDtos;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class SalaryHistory : EntityBase<Guid>
{
    public string UserId { get; private set; }
    public decimal Salary { get; private set; }
    public DateOnly StartDate { get; private set; }
    public SalaryType SalaryType { get; private set; }
    public User User { get; private set; }

    public static SalaryHistory Create(string userId, decimal salary, DateOnly startDate, SalaryType salaryType)
    {
        return new SalaryHistory
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Salary = salary,
            StartDate = startDate,
            SalaryType = salaryType
        };
    }

    public void Update(decimal salary)
    {
        Salary = salary;
    }
}
