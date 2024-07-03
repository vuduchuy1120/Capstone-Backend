namespace Contract.Services.Report.ShareDtos;

public record ReportResponse
(
    Guid Id,
    string UserId,
    string FullName,
    string Description,
    StatusReport Status,
    string StatusName,
    string StatusDesscription,
    ReportType ReportType,
    string ReportTypeName,
    string ReportTypeDescription,
    string? ReplyMessage,
    Guid CompanyId,
    DateOnly CreatedDate
    );
