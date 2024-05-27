using Contract.Services.Role.Create;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class Role : EntityBase<int>
{
    private Role() { }
    public string RoleName { get; private set; }
    public string Decription { get; private set; }
    public List<User>? Users { get; private set; }
    public static Role Create(CreateRoleCommand request)
    {
        return new()
        {
            RoleName = request.RoleName,
            Decription = request.Decription
        };
    }

}
