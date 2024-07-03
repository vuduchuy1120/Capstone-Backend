namespace Contract.Services.SalaryHistory.ShareDtos;

public record SalaryHistoryResponse
(
    SalaryByDayResponse SalaryByDayResponses,
    SalaryByOverTimeResponse SalaryByOverTimeResponses
    );