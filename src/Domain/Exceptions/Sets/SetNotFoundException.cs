using Contract.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Sets;

public class SetNotFoundException : MyException
{
    public SetNotFoundException(Guid id) : base(
        (int) HttpStatusCode.NotFound, 
        $"Set has id: {id} is not found")
    {
    }

    public SetNotFoundException() : base(
        (int)HttpStatusCode.NotFound,
        "Set not found")
    {
    }
}
