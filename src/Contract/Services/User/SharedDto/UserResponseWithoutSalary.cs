
namespace Contract.Services.User.SharedDto;

public record UserResponseWithoutSalary(
    string Id,
        string FirstName,
        string LastName,
        string Phone,
        string Address,
        string Avatar,
        string Gender,
        DateOnly DOB,
        bool IsActive,
        int RoleId,
        string RoleDescription,
        string CompanyName,
        Guid CompanyId);
