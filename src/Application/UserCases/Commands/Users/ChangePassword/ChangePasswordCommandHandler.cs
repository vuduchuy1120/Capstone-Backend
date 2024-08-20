using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.User.ChangePassword;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using Domain.Exceptions.Users;
using FluentValidation;
using System.Net;

namespace Application.UserCases.Commands.Users.ChangePassword;

internal sealed class ChangePasswordCommandHandler(
    IUserRepository _userRepository,
    IUnitOfWork _unitOfWork,
    IPasswordService _passwordService,
    IValidator<ChangePasswordRequest> _validator) : ICommandHandler<ChangePasswordCommand>
{
    public async Task<Result.Success> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var changePasswordRequest = request.ChangePasswordRequest;
        var user = await GetUserAndVeriryPassword(request.LoggedInUserId, changePasswordRequest);

        var validationResult = _validator.Validate(changePasswordRequest);
        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }

        var hashPassword = _passwordService.Hash(changePasswordRequest.newPassword);
        user.UpdatePassword(hashPassword);

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success.Update();
    }

    private async Task<User> GetUserAndVeriryPassword(string userId, ChangePasswordRequest request)
    {
        if (!userId.Equals(request.userId))
        {
            throw new UserIdConflictException();
        }

        if (request.oldPassword.Equals(request.newPassword))
        {
            throw new NewPasswordNotChangeException();
        }

        var user = await _userRepository.GetUserActiveByIdAsync(userId)
            ?? throw new UserNotFoundException();

        var isPasswordValid = _passwordService.IsVerify(user.Password, request.oldPassword);

        if (!isPasswordValid)
        {
            throw new WrongIdOrPasswordException((int) HttpStatusCode.Conflict, "Mật khẩu hiện tại không chính xác");
        }

        return user;
    }
}
