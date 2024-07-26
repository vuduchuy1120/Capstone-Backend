using Contract.Services.MonthlyCompanySalary.Queries;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Data;

public interface IMonthlyCompanySalaryRepository
{
    void AddRange(List<MonthlyCompanySalary> monthlyCompanySalaries);
    Task<MonthlyCompanySalary> GetByIdAsync(Guid id);
    Task<bool> IsExistAsync(Guid id);
    void Update(MonthlyCompanySalary monthlyCompanySalary);
    Task<(List<MonthlyCompanySalary>, int)> SearchMonthlyCompanySalary(GetMonthlyCompanySalaryQuery request);
}
