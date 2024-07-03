namespace Contract.Services.Report.Updates;

public record UpdateReportRequestWithClaims
(
    Guid comapnyIdClaim,
    string roleNameClaim,
    UpdateReportRequest updateReportRequest
    ) ;