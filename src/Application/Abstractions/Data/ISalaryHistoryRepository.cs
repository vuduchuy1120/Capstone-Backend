using Contract.Services.SalaryHistory.ShareDtos;
using Domain.Entities;

namespace Application.Abstractions.Data;

public interface ISalaryHistoryRepository
{
    void AddSalaryHistory(SalaryHistory salaryHistory);
    void AddRangeSalaryHistory(List<SalaryHistory> salaryHistories);
    void UpdateRangeSalaryHistory(List<SalaryHistory> salaryHistories);
    Task<SalaryHistory> GetSalaryHistoryByUserIdDateAndSalaryType(string userId, DateOnly date, SalaryType salaryType);
    Task<(List<SalaryHistory>, int)> GetSalaryHistoryByUserId(string userId, SalaryType salaryType, int pageIndex, int pageSize);

}
