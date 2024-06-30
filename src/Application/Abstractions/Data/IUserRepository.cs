using Contract.Services.User.GetUsers;
using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(string id);
    Task<User?> GetUserActiveByIdAsync(string id);
    Task<bool> IsUserExistAsync(string id);
    Task<bool> IsUserActiveAsync(string id);
    Task<bool> IsAllUserActiveAsync(List<string> userIds);
    void AddUser(User user);
    Task<(List<User>?, int)> SearchUsersAsync(GetUsersQuery request);
    void Update(User user);
    Task<bool> IsAllUserActiveByCompanyId(List<string> userIds, Guid companyId);
    Task<List<User>> GetUsersByCompanyId(Guid companyId);
    Task<bool> IsPhoneNumberExistAsync(string phoneNumber);
    Task<bool> IsUpdatePhoneNumberExistAsync(string phone, string userId);
    Task<User> GetUserByPhoneNumberOrIdAsync(string search);
    Task<bool> IsShipperExistAsync(string id);
}
