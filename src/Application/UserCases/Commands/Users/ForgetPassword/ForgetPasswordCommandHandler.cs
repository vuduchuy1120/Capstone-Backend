using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Application.Utils;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.User.ForgetPassword;
using Domain.Exceptions.Users;

namespace Application.UserCases.Commands.Users.ForgetPassword;

internal sealed class ForgetPasswordCommandHandler(
    IRedisService _redisService, 
    IUserRepository _userRepository)
    : ICommandHandler<ForgetPasswordCommand>
{
    public async Task<Result.Success> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
    {
        await ValidateUserId(request);

        var forgetPasswordRedis = await GetVerifyForgetPasswordRequest(request.userId, cancellationToken);
        await StoreVerifyCodeToRedis(request.userId, forgetPasswordRedis, cancellationToken);

        return Result.Success.RequestForgetPassword();
    }

    private async Task ValidateUserId(ForgetPasswordCommand request)
    {
        if (request.userId != request.loggedInUserId)
        {
            throw new UserIdConflictException();
        }

        var isUserActive = await _userRepository.IsUserActiveAsync(request.userId);
        if (!isUserActive)
        {
            throw new UserNotFoundException();
        }
    }

    private async Task<ForgetPasswordRedis> GetVerifyForgetPasswordRequest(
        string userId,
        CancellationToken cancellationToken)
    {
        var forgetPasswordRedis = await _redisService.GetAsync<ForgetPasswordRedis>(
            ConstantUtil.ForgetPassword_Prefix + userId, 
            cancellationToken);
        if (forgetPasswordRedis is null || forgetPasswordRedis.IsExpired())
        {
            return ForgetPasswordRedis.Create(userId);
        }

        throw new ForgetPassworAlreadyRequestException();
    }

    private async Task StoreVerifyCodeToRedis(
        string userId, 
        ForgetPasswordRedis forgetPasswordRedis, 
        CancellationToken cancellationToken)
    {
        await _redisService.SetAsync(ConstantUtil.ForgetPassword_Prefix + userId, forgetPasswordRedis, cancellationToken);
    }

    private async Task SendVerifyCodeToUser(string verifyCode)
    {

    }
}
