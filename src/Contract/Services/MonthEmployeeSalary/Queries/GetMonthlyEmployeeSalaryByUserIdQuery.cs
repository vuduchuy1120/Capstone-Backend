using Contract.Abstractions.Messages;

namespace Contract.Services.MonthEmployeeSalary.Queries;

public record GetMonthlyEmployeeSalaryByUserIdQuery
(
    string UserId,
    int Month,
    int Year,
    string UserIdClaim,
    string RoleNameClaim) : IQuery<MonthlyEmployeeSalaryResponse>;