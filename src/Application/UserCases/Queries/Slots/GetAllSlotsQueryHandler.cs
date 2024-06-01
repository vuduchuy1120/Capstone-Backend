using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Slot.GetSlots;
using Domain.Exceptions.Slots;

namespace Application.UserCases.Queries.Slots;

internal sealed class GetAllSlotsQueryHandler(ISlotRepository _slotRepository, IMapper _mapper)
    : IQueryHandler<GetAllSlotsQuery, List<SlotResponse>>
{
    public async Task<Result.Success<List<SlotResponse>>> Handle(
        GetAllSlotsQuery request, 
        CancellationToken cancellationToken)
    {
        var slots = await _slotRepository.GetAllSlotsAsync();
        if(slots == null)
        {
            throw new SlotNotFoundException();
        }

        var result = _mapper.Map<List<SlotResponse>>(slots);

        return Result.Success<List<SlotResponse>>.Get(result);
    }
}
