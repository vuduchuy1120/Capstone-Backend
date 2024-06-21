using Contract.Abstractions.Messages;
using Contract.Services.User.SharedDto;

namespace Contract.Services.User.GetUserById;

public record GetUserByIdQuery(string Id) : IQueryHandler<UserResponse>;
