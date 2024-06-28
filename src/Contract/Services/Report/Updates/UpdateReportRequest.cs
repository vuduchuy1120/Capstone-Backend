namespace Contract.Services.Report.Updates;

public record UpdateReportRequest
(
    Guid Id,
    string ReplyMessage,
    string Status
    );
