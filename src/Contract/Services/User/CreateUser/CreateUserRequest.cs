namespace Contract.Services.User.CreateUser;

public record CreateUserRequest(
        string Id,
        string FirstName,
        string LastName,
        string Phone,
        string Address,
        string Password,
        string Gender,
        string DOB,
        decimal SalaryByDay,
        int RoleId);
