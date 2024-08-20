using Contract.Services.MonthlyCompanySalary.Creates;
using Contract.Services.MonthlyCompanySalary.ShareDtos;
using Contract.Services.MonthlyCompanySalary.Updates;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class MonthlyCompanySalary : EntityBase<Guid>
{
    public Guid CompanyId { get; private set; }
    public int Month { get; private set; }
    public int Year { get; private set; }
    public decimal Salary { get; private set; }
    public StatusSalary Status { get; private set; }
    public string? Note { get; private set; }
    public Company Company { get; private set; }

    public static MonthlyCompanySalary Create(CreateMonthlyCompanySalaryRequest request)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            CompanyId = request.CompanyId,
            Month = request.Month,
            Year = request.Year,
            Salary = request.Salary,
            Status = StatusSalary.UNPAID,
        };
    }

    public void UpdateSatus(UpdateMonthlyCompanySalaryRequest request)
    {
        Status = request.Status;
        Note = request.Note;
    }
}
