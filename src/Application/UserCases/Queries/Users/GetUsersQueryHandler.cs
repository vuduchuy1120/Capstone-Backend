using Application.Abstractions.Data;
using Application.Abstractions.Services;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Abstractions.Shared.Search;
using Contract.Services.User.GetUsers;
using Contract.Services.User.SharedDto;
using Domain.Exceptions.Users;

namespace Application.UserCases.Queries.Users;

internal sealed class GetUsersQueryHandler(
    IUserRepository _userRepository,
    IMapper _mapper,
    ICloudStorage _cloudStorage)
    : IQueryHandler<GetUsersQuery, SearchResponse<List<UserResponse>>>
{
    public async Task<Result.Success<SearchResponse<List<UserResponse>>>> Handle(
        GetUsersQuery request, 
        CancellationToken cancellationToken)
    {
        var (users, totalPage) = await _userRepository.SearchUsersAsync(request);

        if (users is null || users.Count <= 0 || totalPage <= 0)
        {
            throw new UserNotFoundException();
        }
        var data = new List<UserResponse>();
        foreach (var user in users)
        {
            var userResponse = _mapper.Map<UserResponse>(user);
            var avatarUrl = await _cloudStorage.GetSignedUrlAsync(user.Avatar);
            userResponse = userResponse with { Avatar = avatarUrl };
            data.Add(userResponse);
        }

        var searchResponse = new SearchResponse<List<UserResponse>>(request.PageIndex, totalPage, data);

        return Result.Success<SearchResponse<List<UserResponse>>>.Get(searchResponse);
    }
}
