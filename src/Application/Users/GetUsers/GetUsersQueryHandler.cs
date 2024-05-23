using Application.Abstractions.Data;
using Application.Shared.Dtos;
using Application.Shared.Dtos.Common;
using Application.Shared.Dtos.Users;
using AutoMapper;
using MediatR;
using System.Net;

namespace Application.Users.GetUsers;

public class GetUsersQueryHandler
    : IRequestHandler<GetUsersQuery, SuccessResponse<SearchResponse<List<UserResponse>>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<SuccessResponse<SearchResponse<List<UserResponse>>>> Handle(
        GetUsersQuery request, 
        CancellationToken cancellationToken)
    {
        var searchResult = await _userRepository.SearchUsersAsync(request);

        var users = searchResult.Item1;
        var totalPage = searchResult.Item2;

        if(users is not null && users.Count > 0 && totalPage > 0)
        {
            var data = users.ConvertAll(user => _mapper.Map<UserResponse>(user));

            var searchResponse = new SearchResponse<List<UserResponse>>()
            {
                TotalPages = totalPage,
                CurrentPage = request.PageIndex,
                Data = data
            };

            return new()
            {
                Status = (int) HttpStatusCode.OK,
                Message = "Search user success",
                Data = searchResponse
            };
        }

        return new()
        {
            Status = (int)HttpStatusCode.NotFound,
            Message = "Search users not found",
            Data = null
        };
    }
}
