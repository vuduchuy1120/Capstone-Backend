using Application.Shared.Dtos;
using Application.Shared.Dtos.Users;
using MediatR;

namespace Application.Users.GetById;

public class GetByIdQuery : IRequest<SuccessResponse<UserResponse>>
{
    public string Id { get; set; }
}
