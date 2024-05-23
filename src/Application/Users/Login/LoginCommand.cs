using Application.Shared.Dtos;
using MediatR;

namespace Application.Users.Login;

public class LoginCommand : IRequest<SuccessResponse<LoginResponse>>
{
    public string Id { get; set; }  
    public string Password { get; set; }
}
