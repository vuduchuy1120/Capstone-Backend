using Contract.Abstractions.Messages;
using Contract.Services.Report.ShareDtos;

namespace Contract.Services.Report.Queries;

public record GetReportByIdQuery(
    Guid id,
    Guid companyIdClaims,
    string userIdClaims,
    string roleNameClaims) : IQuery<ReportResponse>;
