using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Set.GetSet;
using Domain.Exceptions.Sets;

namespace Application.UserCases.Queries.Sets.GetSet;

internal sealed class GetSetQueryHandler(
    ISetRepository _setRepository,
    IMapper _mapper) : IQueryHandler<GetSetQuery, SetsWithProductSalaryResponse>
{
    public async Task<Result.Success<SetsWithProductSalaryResponse>> Handle(GetSetQuery request, CancellationToken cancellationToken)
    {
        var set = await _setRepository.GetByIdAsync(request.setId)
            ?? throw new SetNotFoundException();

        var result = _mapper.Map<SetsWithProductSalaryResponse>(set);

        return Result.Success<SetsWithProductSalaryResponse>.Get(result);
    }
}
