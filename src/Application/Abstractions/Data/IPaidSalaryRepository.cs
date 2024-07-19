using Contract.Services.PaidSalary.Queries;
using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IPaidSalaryRepository
{
    void AddPaidSalary(PaidSalary paidSalary);
    void UpdatePaidSalary(PaidSalary paidSalary);
    Task<PaidSalary> GetPaidSalaryById(Guid id);
    Task<bool> IsPaidSalaryExistsAsync(Guid Id);
    Task<(List<PaidSalary>, int)> GetPaidSalariesByUserIdAsync(GetPaidSalaryByUserIdQuery request);
    void DeletePaidSalary(PaidSalary paidSalary);
}
