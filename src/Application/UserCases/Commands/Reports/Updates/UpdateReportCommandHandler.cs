using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Report.Updates;
using Domain.Abstractions.Exceptions;
using Domain.Exceptions.Reports;
using Domain.Exceptions.Users;
using FluentValidation;
using MediatR;

namespace Application.UserCases.Commands.Reports.Updates;

public sealed class UpdateReportCommandHandler
    (IReportRepository _reportRepository,
    IValidator<UpdateReportRequest> _validator,
    IUnitOfWork _unitOfWork) : ICommandHandler<UpdateReportCommand>
{
    public async Task<Result.Success> Handle(UpdateReportCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request.updateRequest.updateReportRequest);
        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }

        var isCanUpdate = await _reportRepository.IsCanUpdateReport(request.updateRequest.updateReportRequest.Id, request.updateRequest.comapnyIdClaim);
        if (request.updateRequest.roleNameClaim != "MAIN_ADMIN"
            && !isCanUpdate)
        {
            throw new UserNotPermissionException("Bạn không có quyền sửa báo cáo của nhân viên cơ sở khác");
        }

        var report = await _reportRepository.GetReportByIdAsync(request.updateRequest.updateReportRequest.Id);
        if (report == null)
        {
            throw new ReportNotFoundException(request.updateRequest.updateReportRequest.Id);
        }
        report.Update(request.updateRequest.updateReportRequest, request.UpdatedBy);
        _reportRepository.Update(report);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success.Update();
    }
}

