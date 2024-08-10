using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Application.Utils;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.User.Login;
using Contract.Services.User.RefreshToken;
using Contract.Services.User.SharedDto;
using Domain.Entities;
using Domain.Exceptions.Users;

namespace Application.UserCases.Commands.Users.RefreshToken;

internal sealed class RefreshTokenCommandHandler(
    IRedisService _redisService, 
    IJwtService _jwtService,
    IUserRepository _userRepository,
    IMapper _mapper) : ICommandHandler<RefreshTokenCommand, LoginResponse>
{
    public async Task<Result.Success<LoginResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var userIdFromToken = await CheckRefreshTokenAndGetUserIdFromToken(request);

        var user = await _userRepository.GetUserActiveByIdAsync(userIdFromToken)
            ?? throw new UserNotFoundException(userIdFromToken);

        var loginResponse = await CreateLoginResponseAsync(user);

        await CacheLoginResponseAsync(userIdFromToken, loginResponse, cancellationToken);

        return Result.Success<LoginResponse>.Login(loginResponse);
    }

    private  async Task<string> CheckRefreshTokenAndGetUserIdFromToken(RefreshTokenCommand request)
    {
        var userIdFromToken = _jwtService.GetUserIdFromToken(request.refreshToken);

        if (userIdFromToken is null || userIdFromToken != request.userId)
        {
            throw new RefreshTokenNotValidException();
        }

        var loginResponse = await _redisService.GetAsync<LoginResponse>(ConstantUtil.User_Redis_Prefix + userIdFromToken);

        if (loginResponse is null || loginResponse.RefreshToken != request.refreshToken)
        {
            throw new RefreshTokenNotValidException();
        }

        return userIdFromToken;
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
        await _redisService.SetAsync($"{ConstantUtil.User_Redis_Prefix}{userId}", loginResponse);
    }
}
