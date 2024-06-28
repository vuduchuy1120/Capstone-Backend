using Contract.Abstractions.Messages;

namespace Contract.Services.Slot.Create;

public record CreateSlotCommand(string SlotTitle) : ICommand;
