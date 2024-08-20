using Contract.Services.Report.ShareDtos;

namespace Contract.Services.Report.Creates;

public record CreateReportRequest
(
    string Description,
    ReportType ReportType);