using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Search;
using Contract.Services.User.SharedDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Services.Slot.GetSlots;

public record GetAllSlotsQuery : IQuery<List<SlotResponse>>;

