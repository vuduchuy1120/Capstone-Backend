using Contract.Services.MonthEmployeeSalary.ShareDtos;

namespace Contract.Services.MonthEmployeeSalary.Queries;

public record MonthlyEmployeeSalaryResponse
(
    int Month,
    int Year,    
    decimal Salary,
    decimal AccountBalance,
    double TotalWorkingDays,
    double TotalWorkingHours,
    decimal TotalSalaryProduct,
    double Rate,
    List<ProductWorkingResponse> ProductWorkingResponses
    );
