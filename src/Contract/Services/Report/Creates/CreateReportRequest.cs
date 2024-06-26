namespace Contract.Services.Report.Creates;

public record CreateReportRequest
(
    string Description,
    string Status,
    string ReportType);