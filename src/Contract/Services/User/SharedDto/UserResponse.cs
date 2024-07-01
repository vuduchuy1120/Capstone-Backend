﻿namespace Contract.Services.User.SharedDto;

public record UserResponse(
        string Id,
        string FirstName,
        string LastName,
        string Phone,
        string Address,
        string Gender,
        DateOnly DOB,
        bool IsActive,
        int RoleId,
        string RoleDescription,
        string CompanyName,
        Guid CompanyId);
