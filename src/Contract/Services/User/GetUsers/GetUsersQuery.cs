using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Search;
using Contract.Services.User.SharedDto;

namespace Contract.Services.User.GetUsers;

public record GetUsersQuery(
    string? SearchTerm,
    int RoleId,
    bool IsActive = true,
    int PageIndex = 1,
    int PageSize = 10) : IQuery<SearchResponse<List<UserResponse>>>;
