using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Slot.Create;
using Domain.Entities;

namespace Application.UserCases.Commands.Slots.CreateSlot
{
    internal sealed class CreateSlotCommandHandler(ISlotRepository _slotRepository, IUnitOfWork _unitOfWork)
        : ICommandHandler<CreateSlotCommand>
    {
        public async Task<Result.Success> Handle(CreateSlotCommand request, CancellationToken cancellationToken)
        {
            var slot = Slot.Create(request);
            _slotRepository.AddSlot(slot);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success.Create();
        }

    }
}
