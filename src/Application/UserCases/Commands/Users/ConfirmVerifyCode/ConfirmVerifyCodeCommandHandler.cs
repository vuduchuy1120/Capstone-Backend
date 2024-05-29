using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Application.Utils;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.User.ConfirmVerifyCode;
using Contract.Services.User.ForgetPassword;
using Domain.Abstractions.Exceptions;
using Domain.Exceptions.Users;
using FluentValidation;

namespace Application.UserCases.Commands.Users.ConfirmVerifyCode;

internal sealed class ConfirmVerifyCodeCommandHandler(
    IUserRepository _userRepository,
    IUnitOfWork _unitOfWork,
    IPasswordService _passwordService,
    IRedisService _redisService,
    IValidator<ConfirmVerifyCodeCommand> _validator) : ICommandHandler<ConfirmVerifyCodeCommand>
{
    public async Task<Result.Success> Handle(ConfirmVerifyCodeCommand request, CancellationToken cancellationToken)
    {
        await CheckVerifyCodeWithCodeInRedis(request.VerifyCode, request.UserId, cancellationToken);

        CheckPasswordValid(request);

        await FindAndUpdatePassword(request.UserId, request.Password);

        await RemoveVerifyCodeInRedis(request.UserId);

        return Result.Success.Update();
    }

    private void CheckPasswordValid(ConfirmVerifyCodeCommand confirmCodeRequest)
    {
        var validationResult = _validator.Validate(confirmCodeRequest);

        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }
    }

    private async Task CheckVerifyCodeWithCodeInRedis(
        string verifyCode, 
        string userId, 
        CancellationToken cancellationToken)
    {
        var forgetPasswordRedis = await _redisService.GetAsync<ForgetPasswordRedis>(
            ConstantUtil.ForgetPassword_Prefix + userId,
            cancellationToken);

        if(forgetPasswordRedis is null)
        {
            throw new VerifyCodeNotValidException();
        }

        forgetPasswordRedis.UpdateVerifyCodeNumberUse();
        await UpdateVerifyCodeInRedis(userId, forgetPasswordRedis);

        if (forgetPasswordRedis.IsExpired() || !forgetPasswordRedis.IsVerifyCodeValid(verifyCode))
        {
            throw new VerifyCodeNotValidException();
        }
    }

    private async Task FindAndUpdatePassword(string userId, string password)
    {
        var user = await _userRepository.GetUserActiveByIdAsync(userId)
        ?? throw new UserNotFoundException();

        var hashPassword = _passwordService.Hash(password);
        user.UpdatePassword(hashPassword);

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task RemoveVerifyCodeInRedis(string userId)
    {
        await _redisService.RemoveAsync(ConstantUtil.ForgetPassword_Prefix + userId);
    }

    private async Task UpdateVerifyCodeInRedis(string userId, ForgetPasswordRedis forgetPasswordRedis)
    {
        await _redisService.SetAsync(ConstantUtil.ForgetPassword_Prefix + userId, forgetPasswordRedis);
    }
}
