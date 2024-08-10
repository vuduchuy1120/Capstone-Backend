using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Application.Utils;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.User.Login;
using Contract.Services.User.SharedDto;
using Domain.Entities;
using Domain.Exceptions.Users;

namespace Application.UserCases.Commands.Users.Login;

internal sealed class LoginCommandHandler(
    IUserRepository _userRepository,
    IJwtService _jwtService,
    IPasswordService _passwordService,
    IMapper _mapper,
    IRedisService _redisService)
    : ICommandHandler<LoginCommand, LoginResponse>
{
    public async Task<Result.Success<LoginResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var user = await GetUserAndVerifyPasswordAsync(request.Id, request.Password);

        var loginResponse = await CreateLoginResponseAsync(user);

        await CacheLoginResponseAsync(user.Id, loginResponse, cancellationToken);

        return Result.Success<LoginResponse>.Login(loginResponse);
    }

    private async Task<User> GetUserAndVerifyPasswordAsync(string userId, string password)
    {
        var user = await _userRepository.GetUserByPhoneNumberOrIdAsync(userId) ?? throw new UserNotFoundException(userId);

        var isPasswordValid = _passwordService.IsVerify(user.Password, password);

        if (!isPasswordValid)
        {
            throw new WrongIdOrPasswordException();
        }

        return user;
    }

    private async Task<LoginResponse> CreateLoginResponseAsync(User user)
    {
        var accessToken = await _jwtService.CreateAccessToken(user);
        var refreshToken = await _jwtService.CreateRefreshToken(user);
        var userResponse = _mapper.Map<UserResponse>(user);
        return new LoginResponse(userResponse, accessToken, refreshToken);
    }

    private async Task CacheLoginResponseAsync(string userId, LoginResponse loginResponse, CancellationToken cancellationToken)
    {
        await _redisService.SetAsync($"{ConstantUtil.User_Redis_Prefix}{userId}", loginResponse, TimeSpan.FromMinutes(100));
    }
}
