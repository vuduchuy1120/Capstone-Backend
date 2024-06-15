using Contract.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.SetProducts;

public class SetProductNotFoundException : MyException
{
    public SetProductNotFoundException() : base(
        (int) HttpStatusCode.NotFound, 
        "Set product not found")
    {
    }
}
