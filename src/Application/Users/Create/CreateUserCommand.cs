using Application.Shared.Dtos;
using Application.Shared.Dtos.Users;
using MediatR;

namespace Application.Users.Create;

public class CreateUserCommand : IRequest<SuccessResponse<UserResponse>>
{
    public string Id { get; set; }
    public string Fullname { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string Password { get; set; }
    public int RoleId { get; set; }
}
