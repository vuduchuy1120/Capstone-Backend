using Contract.Services.MonthEmployeeSalary.Creates;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class MonthlyEmployeeSalary : EntityBase<Guid>
{
    public string UserId { get; private set; }
    public int Month { get; private set; }
    public int Year { get; private set; }
    public decimal Salary { get; private set; }
    public string? Note { get; private set; }
    public User User { get; private set; }

    public static MonthlyEmployeeSalary Create(CreateMonthlyEmployeeSalaryRequest request)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Month = request.Month,
            Year = request.Year,
            Salary = request.Salary
        };
    }

}
