using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Set.GetSet;
using Contract.Services.Set.GetSets;
using Domain.Exceptions.Sets;

namespace Application.UserCases.Queries.Sets.GetSet;

internal sealed class GetSetQueryHandler(
    ISetRepository _setRepository,
    IMapper _mapper) : IQueryHandler<GetSetQuery, SetResponse>
{
    public async Task<Result.Success<SetResponse>> Handle(GetSetQuery request, CancellationToken cancellationToken)
    {
        var set = await _setRepository.GetByIdAsync(request.setId)
            ?? throw new SetNotFoundException();

        var result = _mapper.Map<SetResponse>(set);

        return Result.Success<SetResponse>.Get(result);
    }
}
