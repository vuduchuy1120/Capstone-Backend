using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.User.GetUserById;
using Contract.Services.User.SharedDto;
using Domain.Exceptions.Users;

namespace Application.UserCases.Queries.Users;

internal sealed class GetUserByIdQueryHandler(IUserRepository _userRepository, IMapper _mapper)
    : IQueryHandler<GetUserByIdQuery, UserResponse>
{
    public async Task<Result.Success<UserResponse>> Handle(
        GetUserByIdQuery request, 
        CancellationToken cancellationToken)
    {
        string id = request.Id;
        var user = await _userRepository.GetUserByIdAsync(id);

        if (user is null)
        {
            throw new UserNotFoundException(id);
        }

        var userResponse = _mapper.Map<UserResponse>(user);

        return Result.Success<UserResponse>.Get(userResponse);
    }
}
