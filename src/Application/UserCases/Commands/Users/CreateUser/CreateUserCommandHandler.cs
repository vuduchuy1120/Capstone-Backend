using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Application.Utils;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.SalaryHistory.ShareDtos;
using Contract.Services.User.Command;
using Contract.Services.User.CreateUser;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using FluentValidation;

namespace Application.UserCases.Commands.Users.CreateUser;

internal sealed class CreateUserCommandHandler(
    IUserRepository _userRepository,
    ISalaryHistoryRepository _salaryHistoryRepository,
    IUnitOfWork _unitOfWork,
    IPasswordService _passwordService,
    ISpeedSMSAPI _speedSMSAPI,
    IValidator<CreateUserRequest> _validator) : ICommandHandler<CreateUserCommand>
{

    public async Task<Result.Success> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var createUserRequest = request.CreateUserRequest;

        var validationResult = await _validator.ValidateAsync(createUserRequest);

        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }

        var password = PasswordGenerator.GenerateRandomPassword();

        string hashPassword = _passwordService.Hash(password);
        var user = User.Create(createUserRequest, hashPassword, request.CreatedBy);
        var salaryByDay = createUserRequest.SalaryByDayRequest;
        var salaryOverTime = createUserRequest.SalaryOverTimeRequest;
        var salaryhistories = new List<SalaryHistory>
        {
            SalaryHistory.Create(user.Id, salaryByDay.Salary,DateUtil.ConvertStringToDateTimeOnly(salaryByDay.StartDate), SalaryType.SALARY_BY_DAY),
            SalaryHistory.Create(user.Id, salaryOverTime.Salary,DateUtil.ConvertStringToDateTimeOnly(salaryOverTime.StartDate), SalaryType.SALARY_OVER_TIME)
        };
        _userRepository.AddUser(user);
        _salaryHistoryRepository.AddRangeSalaryHistory(salaryhistories);

        await _unitOfWork.SaveChangesAsync();

        _speedSMSAPI.sendSMS([user.Phone], $"Bạn đã được tạo tài khoản đăng nhập hệ thống TienHuyBamboo với mật khẩu: {password}", 2);

        return Result.Success.Create();
    }
}
