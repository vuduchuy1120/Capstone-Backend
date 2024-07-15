using Contract.Services.MonthEmployeeSalary.ShareDtos;

namespace Contract.Services.MonthEmployeeSalary.Queries;

public record MonthlyEmployeeSalaryResponse
(
    int Month,
    int Year,    
    decimal AccountBalance,
    double TotalWorkingDays,
    double TotalWorkingHours,
    decimal TotalSalaryProduct,
    List<ProductWorkingResponse> ProductWorkingResponses
    );
