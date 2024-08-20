namespace Contract.Services.SalaryHistory.Creates;

public record CreateSalaryHistoryRequest
(
    string UserId,
    SalaryByDayRequest SalaryByDayRequest,
    SalaryOverTimeRequest SalaryOverTimeRequest
    );
