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
    ITokenRepository _tokenRepository,
    IUnitOfWork _unitOfWork,
    IUserRepository _userRepository,
    IMapper _mapper) : ICommandHandler<RefreshTokenCommand, LoginResponse>
{
    public async Task<Result.Success<LoginResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var token = await CheckRefreshTokenAndGetToken(request);
        var userId = request.userId;

        var user = await _userRepository.GetUserActiveByIdAsync(userId)
            ?? throw new UserNotFoundException(userId);

        var loginResponse = await CreateLoginResponseAsync(user);

        await CacheLoginResponseAsync(token, loginResponse, cancellationToken);

        return Result.Success<LoginResponse>.Login(loginResponse);
    }

    private async Task<Token> CheckRefreshTokenAndGetToken(RefreshTokenCommand request)
    {
        //var userIdFromToken = _jwtService.GetUserIdFromToken(request.refreshToken);

        //if (userIdFromToken is null || userIdFromToken != request.userId)
        //{
        //    throw new RefreshTokenNotValidException();
        //}

        var token = await _tokenRepository.GetByUserIdAsync(request.userId);

        //var loginResponse = await _redisService.GetAsync<LoginResponse>(ConstantUtil.User_Redis_Prefix + request.userId);

        if (token is null || token.RefreshToken != request.refreshToken)
        {
            throw new RefreshTokenNotValidException("Không tìm thấy thông tin đăng nhập hoặc sai token");
        }

        return token;
    }

    private async Task<LoginResponse> CreateLoginResponseAsync(User user)
    {
        var accessToken = await _jwtService.CreateAccessToken(user);
        //var refreshToken = await _jwtService.CreateRefreshToken(user);
        var refreshToken = PasswordGenerator.GenerateRandomPassword(20);

        var userResponse = _mapper.Map<UserResponse>(user);
        return new LoginResponse(userResponse, accessToken, refreshToken);
    }

    private async Task CacheLoginResponseAsync(Token token, LoginResponse loginResponse, CancellationToken cancellationToken)
    {
        token.Update(loginResponse.AccessToken, loginResponse.RefreshToken);
        _tokenRepository.Update(token);
        await _unitOfWork.SaveChangesAsync();
        //await _redisService.SetAsync($"{ConstantUtil.User_Redis_Prefix}{userId}", loginResponse, TimeSpan.FromMinutes(100));
    }
}
