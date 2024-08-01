using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Roles;

public class RoleAlreadyExistedException : MyException
{
    public RoleAlreadyExistedException(string roleName)
        : base((int)HttpStatusCode.Conflict, $"Vai trò: {roleName} đã tồn tại")
    {
    }
}
