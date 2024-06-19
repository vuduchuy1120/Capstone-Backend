using Domain.Abstractions.Exceptions.Base;

namespace Domain.Exceptions.Phases;

public class PhaseNotFoundException : MyException
{
    public PhaseNotFoundException(Guid id)
       : base(400, $"Can not found phase has id: {id}")
    {
    }

    public PhaseNotFoundException()
        : base(400, $"Search phase not found")
    {
    }
}
