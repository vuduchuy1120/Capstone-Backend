using Domain.Abstractions.Entities;
using Domain.Users;

namespace Domain.Roles;

public class Role : EntityBase<int>
{
    private Role() { }
    public string RoleName { get; private set; }
    public string Decription {  get; private set; }
    public List<User>? Users { get; private set; }
    public static Role Create(string roleName, string description)
    {
        return new()
        {
            RoleName = roleName,
            Decription = description,
        };
    }

}
