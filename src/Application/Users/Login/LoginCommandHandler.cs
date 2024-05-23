using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Application.Shared.Dtos;
using Application.Shared.Dtos.Users;
using AutoMapper;
using Domain.Users;
using MediatR;
using System.Net;

namespace Application.Users.Login;

internal sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, SuccessResponse<LoginResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordService _passwordService;
    private readonly IMapper _mapper;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IJwtService jwtService, 
        IPasswordService passwordService, 
        IMapper mapper)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _passwordService = passwordService;
        _mapper = mapper;
    }

    public async Task<SuccessResponse<LoginResponse>> Handle(
        LoginCommand request, 
        CancellationToken cancellationToken)
    {
        var user = await GetUserById(request.Id);

        var isPasswordValid = _passwordService.IsVerify(user.Password, request.Password);

        if (!isPasswordValid)
        {
            throw new WrongIdOrPasswordException();
        }

        var accessToken = _jwtService.GenerateToken(user);
        var refreshToken = _jwtService.GenerateToken(user);

        var userResponse = _mapper.Map<UserResponse>(user);

        var loginResponse = new LoginResponse()
        {
            User = userResponse,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        return new SuccessResponse<LoginResponse>
        {
            Status = (int)HttpStatusCode.OK,
            Message = "Login success",
            Data = loginResponse
        };
    }

    private async Task<User> GetUserById(string id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);

        if(user is null)
        {
            throw new UserNotFoundException(id);
        }

        return user;
    }
}
