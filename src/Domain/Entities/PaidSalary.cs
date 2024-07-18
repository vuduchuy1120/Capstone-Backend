using Contract.Services.PaidSalary.Creates;
using Contract.Services.PaidSalary.Updates;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class PaidSalary : EntityAuditBase<Guid>
{
    public string UserId { get; private set; }
    public decimal Salary { get; private set; }
    public string? Note { get; private set; }
    public User User { get; private set; }

    public static PaidSalary Create(CreatePaidSalaryRequest request, string CreatedBy)
    {
        return new()
        {
            UserId = request.UserId,
            Salary = request.Salary,
            Note = request.Note,
            CreatedBy = CreatedBy,
            CreatedDate = DateTime.UtcNow.AddHours(7)
        };
    }
    public void Update(UpdatePaidSalaryRequest request, string updatedBy)
    {
        Salary = request.Salary;
        Note = request.Note;
        UpdatedBy = updatedBy;
        UpdatedDate = DateTime.UtcNow.AddHours(7);
    }
}
