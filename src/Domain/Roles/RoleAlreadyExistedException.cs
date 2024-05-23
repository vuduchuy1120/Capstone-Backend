using Domain.Abstractions.Exceptions.Base;
using System.Globalization;
using System.Net;

namespace Domain.Roles;

public class RoleAlreadyExistedException : MyException
{
    public RoleAlreadyExistedException(string roleName) 
        : base((int) HttpStatusCode.Conflict, $"Role: {roleName} is already exist")
    {
    }
}
