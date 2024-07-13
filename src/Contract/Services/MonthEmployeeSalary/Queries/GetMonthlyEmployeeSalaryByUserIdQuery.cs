using Contract.Abstractions.Messages;

namespace Contract.Services.MonthEmployeeSalary.Queries;

public record GetMonthlyEmployeeSalaryByUserIdQuery(string UserId) : IQuery<MonthlyEmployeeSalaryResponse>;