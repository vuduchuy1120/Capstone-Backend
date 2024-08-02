using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.User.GetUsers;
using Contract.Services.User.SharedDto;
using Domain.Exceptions.Users;

namespace Application.UserCases.Queries.Users;

public class GetUsersByCompanyIdQueryHandler
    (IUserRepository _userRepository,
    IMapper _mapper
    ) : IQueryHandler<GetUsersByCompanyIdQuery, List<UserResponse>>
{
    public async Task<Result.Success<List<UserResponse>>> Handle(GetUsersByCompanyIdQuery request, CancellationToken cancellationToken)
    {
        if (IsHavePermission(request))
        {
            throw new UserNotPermissionException("You don't have permission get other user companyId");
        }

        var users = await _userRepository.GetUsersByCompanyId(request.GetUsersRequest.CompanyId);

        var userResponses = _mapper.Map<List<UserResponse>>(users);

        return Result.Success<List<UserResponse>>.Get(userResponses);
    }

    private bool IsHavePermission(GetUsersByCompanyIdQuery request)
    {
        return request.RoleNameClaim != "MAIN_ADMIN" && request.CompanyIdClaim != request.GetUsersRequest.CompanyId;
    }
}
