using Domain.Entities;

namespace Application.Abstractions.Data;

public interface ISalaryHistoryRepository
{
    void AddSalaryHistory(SalaryHistory salaryHistory);
}
