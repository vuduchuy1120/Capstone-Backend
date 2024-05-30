using Contract.Abstractions.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Services.Slot.Create;

public record CreateSlotCommand(string SlotTitle) : ICommand;
