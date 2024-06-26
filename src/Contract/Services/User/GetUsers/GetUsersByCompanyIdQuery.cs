using Contract.Abstractions.Messages;
using Contract.Services.User.SharedDto;

namespace Contract.Services.User.GetUsers;

public record GetUsersByCompanyIdQuery
(GetUsersByCompanyIdRequest GetUsersRequest, Guid CompanyIdClaim, string RoleNameClaim) : IQuery<List<UserResponse>>;
