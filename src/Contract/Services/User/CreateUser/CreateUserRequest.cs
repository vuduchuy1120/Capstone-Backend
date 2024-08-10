using Contract.Services.SalaryHistory.Creates;

namespace Contract.Services.User.CreateUser;

public record CreateUserRequest(
        string Id,
        string FirstName,
        string LastName,
        string? Avatar,
        string Phone,
        string Address,
        string Gender,
        string DOB,
        SalaryByDayRequest SalaryByDayRequest,
        SalaryOverTimeRequest SalaryOverTimeRequest,
        Guid CompanyId,
        int RoleId);
