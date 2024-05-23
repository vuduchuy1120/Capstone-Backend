using Application.Users.GetUsers;
using Domain.Users;

namespace Application.Abstractions.Data;

public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(string id);
    Task<bool> IsUserExistAsync(string id);
    void AddUser(User user);
    Task<(List<User>?, int)> SearchUsersAsync(GetUsersQuery request);
}
