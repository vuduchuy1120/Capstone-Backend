using Contract.Services.User.GetUsers;
using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(string id);
    Task<User?> GetUserActiveByIdAsync(string id);
    Task<bool> IsUserExistAsync(string id);
    Task<bool> IsUserActiveAsync(string id);
    void AddUser(User user);
    Task<(List<User>?, int)> SearchUsersAsync(GetUsersQuery request);
    void Update(User user);
}
