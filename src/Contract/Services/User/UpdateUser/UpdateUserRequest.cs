namespace Contract.Services.User.UpdateUser;

public record UpdateUserRequest(
        string Id,
        string FirstName,
        string LastName,
        string Phone,
        string Address,
        string Gender,
        string DOB,
        decimal SalaryByDay,
        int RoleId);
