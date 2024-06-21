using Contract.Abstractions.Messages;

namespace Contract.Services.Slot.GetSlots;

public record GetAllSlotsQuery : IQueryHandler<List<SlotResponse>>;

