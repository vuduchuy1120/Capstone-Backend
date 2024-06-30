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
        decimal SalaryByDay,
        Guid CompanyId,
        int RoleId);
