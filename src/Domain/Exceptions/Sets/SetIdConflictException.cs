using Contract.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Sets;

public class SetIdConflictException : MyException
{
    public SetIdConflictException() : base(
        (int) HttpStatusCode.Conflict, 
        "SetId in router is not same with setId in body")
    {
    }
}
