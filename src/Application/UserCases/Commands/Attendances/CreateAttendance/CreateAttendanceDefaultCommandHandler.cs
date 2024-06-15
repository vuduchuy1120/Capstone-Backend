using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Attendance.Create;
using Contract.Abstractions.Exceptions;
using Domain.Entities;
using FluentValidation;

namespace Application.UserCases.Commands.Attendances.CreateAttendance;

internal sealed class CreateAttendanceDefaultCommandHandler(
    IAttendanceRepository _attendanceRepository,
    IValidator<CreateAttendanceDefaultRequest> _validator,
    IUnitOfWork _unitOfWork) : ICommandHandler<CreateAttendanceDefaultCommand>
{
    public async Task<Result.Success> Handle(CreateAttendanceDefaultCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request.CreateAttendanceDefaultRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }

        var emps = request.CreateAttendanceDefaultRequest.CreateAttendances;

        foreach (var emp in emps)
        {
            var attendance = Attendance.Create(emp, request.CreateAttendanceDefaultRequest.slotId, request.CreatedBy);
            _attendanceRepository.AddAttendance(attendance);
        }

        await _unitOfWork.SaveChangesAsync();
        return Result.Success.Create();
    }
}
