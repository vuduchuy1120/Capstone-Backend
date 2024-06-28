using Domain.Abstractions.Exceptions.Base;

namespace Domain.Exceptions.Slots;

public class SlotNotFoundException : MyException
{
    public SlotNotFoundException() : base(400, $"Search slot not found")
    {
    }
}
