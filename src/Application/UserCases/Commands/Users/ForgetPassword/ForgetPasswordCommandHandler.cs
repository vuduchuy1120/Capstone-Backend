using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Application.Utils;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.User.ForgetPassword;
using Domain.Entities;
using Domain.Exceptions.Users;

namespace Application.UserCases.Commands.Users.ForgetPassword;

internal sealed class ForgetPasswordCommandHandler(
    IRedisService _redisService, 
    IUserRepository _userRepository,
    ISpeedSMSAPI _smsApi)
    : ICommandHandler<ForgetPasswordCommand>
{
    public async Task<Result.Success> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await ValidateAndGetUser(request);

        var forgetPasswordRedis = await GetVerifyForgetPasswordRequest(user.Id, cancellationToken);
        await StoreVerifyCodeToRedis(user.Id, forgetPasswordRedis, cancellationToken);
        await SendVerifyCodeToUser(forgetPasswordRedis.VerifyCode, user.Phone);

        return Result.Success.RequestForgetPassword();
    }

    private async Task<User> ValidateAndGetUser(ForgetPasswordCommand request)
    {
        var user = await _userRepository.GetByPhoneOrIdAsync(request.userId);
        if (user is null)
        {
            throw new UserNotFoundException();
        }

        return user;
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
        await _redisService.SetAsync(ConstantUtil.ForgetPassword_Prefix + userId, forgetPasswordRedis);
    }

    private async Task SendVerifyCodeToUser(string verifyCode, string phone)
    {
        await Task.Run(() =>
        {
            _smsApi.sendSMS(new[] { phone }, $"Mã xác thực của bạn là: {verifyCode}", 5);
        });
    }
}
