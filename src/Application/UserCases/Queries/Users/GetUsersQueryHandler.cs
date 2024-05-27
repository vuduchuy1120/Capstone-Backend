using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Abstractions.Shared.Search;
using Contract.Services.User.GetUsers;
using Contract.Services.User.SharedDto;
using Domain.Exceptions.Users;

namespace Application.UserCases.Queries.Users;

internal sealed class GetUsersQueryHandler(IUserRepository _userRepository, IMapper _mapper)
    : IQueryHandler<GetUsersQuery, SearchResponse<List<UserResponse>>>
{
    public async Task<Result.Success<SearchResponse<List<UserResponse>>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var searchResult = await _userRepository.SearchUsersAsync(request);

        var users = searchResult.Item1;
        var totalPage = searchResult.Item2;

        if (users is null || users.Count > 0 || totalPage > 0)
        {
            throw new UserNotFoundException();
        }

        var data = users.ConvertAll(user => _mapper.Map<UserResponse>(user));

        var searchResponse = new SearchResponse<List<UserResponse>>(totalPage, request.PageIndex, data);

        return Result.Success<SearchResponse<List<UserResponse>>>.Get(searchResponse);
    }
}
