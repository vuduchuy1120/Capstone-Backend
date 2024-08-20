using Contract.Services.SalaryHistory.Creates;

namespace Contract.Services.User.UpdateUser;

public record UpdateUserRequest(
        string Id,
        string FirstName,
        string LastName,
        string Phone,
        string? Avatar,
        string Address,
        string Gender,
        string DOB,
        SalaryByDayRequest SalaryByDayRequest,
        SalaryOverTimeRequest SalaryOverTimeRequest,
        Guid CompanyId,
        int RoleId);
