using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Report.Updates;
using Domain.Abstractions.Exceptions;
using Domain.Exceptions.Reports;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UserCases.Commands.Reports.Updates;

public sealed class UpdateReportCommandHandler
    (IReportRepository _reportRepository,
    IValidator<UpdateReportRequest> _validator,
    IUnitOfWork _unitOfWork) : ICommandHandler<UpdateReportCommand>
{
    public async Task<Result.Success> Handle(UpdateReportCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request.updateRequest);
        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }
        var report = await _reportRepository.GetReportByIdAsync(request.updateRequest.Id);
        if (report == null)
        {
            throw new ReportNotFoundException(request.updateRequest.Id);
        }
        report.Update(request.updateRequest, request.UpdatedBy);
        _reportRepository.Update(report);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success.Update();
    }
}

