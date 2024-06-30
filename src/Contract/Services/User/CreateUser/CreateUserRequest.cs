namespace Contract.Services.User.CreateUser;

public record CreateUserRequest(
        string Id,
        string FirstName,
        string LastName,
        string? Avatar,
        string Phone,
        string Address,
        string Password,
        string Gender,
        string DOB,
        decimal SalaryByDay,
        Guid CompanyId,
        int RoleId);
