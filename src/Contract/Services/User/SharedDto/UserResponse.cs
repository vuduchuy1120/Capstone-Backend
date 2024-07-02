using Contract.Services.SalaryHistory.ShareDtos;

namespace Contract.Services.User.SharedDto;

public record UserResponse(
        string Id,
        string FirstName,
        string LastName,
        string Phone,
        string Address,
        string Avatar,
        string Gender,
        DateOnly DOB,
        SalaryHistoryResponse SalaryHistoryResponse,
        bool IsActive,        
        int RoleId,
        string RoleDescription,
        string CompanyName,
        Guid CompanyId);
