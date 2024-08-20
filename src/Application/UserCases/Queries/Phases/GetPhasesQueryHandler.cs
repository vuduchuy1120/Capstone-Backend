using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Phase.Queries;
using Contract.Services.Phase.ShareDto;
using Domain.Exceptions.Phases;

namespace Application.UserCases.Queries.Phases;

public sealed class GetPhasesQueryHandler
    (IPhaseRepository _phaseRepository,
    IMapper _mapper) : IQueryHandler<GetPhasesQuery, List<PhaseResponse>>
{
    public async Task<Result.Success<List<PhaseResponse>>> Handle(GetPhasesQuery request, CancellationToken cancellationToken)
    {
        var phases = await _phaseRepository.GetPhases();
        if (phases == null)
        {
            throw new PhaseNotFoundException();
        }
        var phaseResponses = _mapper.Map<List<PhaseResponse>>(phases);
        return Result.Success<List<PhaseResponse>>.Get(phaseResponses);
    }
}
