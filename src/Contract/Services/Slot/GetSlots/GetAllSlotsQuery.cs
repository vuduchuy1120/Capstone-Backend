using Contract.Abstractions.Messages;

namespace Contract.Services.Slot.GetSlots;

public record GetAllSlotsQuery : IQuery<List<SlotResponse>>;

