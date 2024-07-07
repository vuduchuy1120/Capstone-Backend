using Application.Abstractions.Data;
using Application.Abstractions.Services;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Report.Queries;
using Contract.Services.Report.ShareDtos;
using Contract.Services.User.SharedDto;
using Domain.Entities;
using Domain.Exceptions.Users;

namespace Application.UserCases.Queries.Reports;

public sealed class GetReportByIdQueryHandler
    (IReportRepository _reportRepository,
    ICloudStorage _cloudStorage,
    IMapper _mapper
    ) : IQueryHandler<GetReportByIdQuery, ReportResponse>
{
    public async Task<Result.Success<ReportResponse>> Handle(GetReportByIdQuery request, CancellationToken cancellationToken)
    {

        var isCanView = await _reportRepository.IsCanGetReportByIdAsync(request.id, request.companyIdClaims, request.userIdClaims, request.roleNameClaims);

        if (!isCanView)
        {
            throw new UserNotPermissionException("Bạn không có quyền xem báo cáo của nhân viên này");
        }

        var report = await _reportRepository.GetReportByIdAsync(request.id);
        var avatarUrl = await _cloudStorage.GetSignedUrlAsync(report.User.Avatar);
        var reportResponse = _mapper.Map<ReportResponse>(report);
        reportResponse = reportResponse with { Avatar = avatarUrl };

        return Result.Success<ReportResponse>.Get(reportResponse);

    }
}
