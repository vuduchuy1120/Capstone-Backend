using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Report.Creates;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using Domain.Exceptions.Users;
using FluentValidation;

namespace Application.UserCases.Commands.Reports.Creates;

public sealed class CreateReportCommandHandler
    (IReportRepository _reportRepository,
    IUserRepository _userRepository,
    IValidator<CreateReportRequest> _validator,
    IUnitOfWork _unitOfWork
    ) : ICommandHandler<CreateReportCommand>
{
    public async Task<Result.Success> Handle(CreateReportCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request.CreateReportRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }

        var user = await _userRepository.IsUserActiveAsync(request.CreatedBy);
        if (user == null)
        {
            throw new UserNotFoundException(request.CreatedBy);
        }

        var reportRequest = request.CreateReportRequest;
        var report = Report.Create(reportRequest, request.CreatedBy);
        _reportRepository.Add(report);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success.Create();
    }
}

