using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Services.Slot.GetSlots
{
    public record SlotResponse
    (
        int Id,
        string SlotTitle
    );
}
