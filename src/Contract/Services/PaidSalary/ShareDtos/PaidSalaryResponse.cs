namespace Contract.Services.PaidSalary.ShareDtos;

public record PaidSalaryResponse
(
    Guid Id,
    string UserId,
    decimal Salary,
    string? Note,
    DateOnly CreatedAt,
    string CreatedBy
    );
