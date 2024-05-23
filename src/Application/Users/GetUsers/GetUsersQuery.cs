using Application.Shared.Dtos;
using Application.Shared.Dtos.Common;
using Application.Shared.Dtos.Users;
using MediatR;

namespace Application.Users.GetUsers;

public class GetUsersQuery : IRequest<SuccessResponse<SearchResponse<List<UserResponse>>>>
{
    public string SearchTerm { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}
