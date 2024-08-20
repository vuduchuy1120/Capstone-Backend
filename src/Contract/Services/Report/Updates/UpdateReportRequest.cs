using Contract.Services.Report.ShareDtos;

namespace Contract.Services.Report.Updates;

public record UpdateReportRequest
(
    Guid Id,
    string ReplyMessage,
    StatusReport Status
    );
