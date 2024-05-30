using Contract.Services.Slot.Create;
using Domain.Abstractions.Entities;
using System.Security.Principal;

namespace Domain.Entities;

public class Slot : EntityBase<int>
{
    private Slot() { }

    public string SlotTitle { get; private set; }

    public List<Attendance>? Attendances { get; set; }

    public static Slot Create(CreateSlotCommand request)
    {
        return new Slot() { SlotTitle = request.SlotTitle };
    }


}
