namespace Contract.Services.Report.Queries;

public record SearchReportsQuery
(
    string? UserId,
    string? Status,
    string? ReportType,
    string? Description,
    string? StartDate,
    Guid CompanyId,
    string? EndDate,
    int PageIndex = 1,
    int PageSize = 10
    );
