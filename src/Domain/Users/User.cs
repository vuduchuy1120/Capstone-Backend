using Domain.Abstractions.Entities;
using Domain.Roles;

namespace Domain.Users;

public class User : EntityAuditBase<string>
{
    public string Fullname { get; private set; }
    public string Phone { get; private set; }
    public string Address { get; private set; }
    public string Password { get; private set; }
    public int RoleId { get; private set; }
    public Role Role { get; private set; }

    public static User Create(
        string id, 
        string fullname, 
        string phone, 
        string address, 
        string password, 
        int roleId,
        string createdBy)
    {
        return new()
        {
            Id = id,
            Fullname = fullname,
            Phone = phone,
            Address = address,
            Password = password,
            CreatedBy = createdBy,
            CreatedDate = DateTime.UtcNow,
            UpdatedBy = createdBy,
            UpdatedDate = DateTime.UtcNow,
            RoleId = roleId,
        };
    }
}
