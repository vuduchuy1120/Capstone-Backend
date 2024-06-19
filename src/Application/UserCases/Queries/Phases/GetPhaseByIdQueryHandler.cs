using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Phase.Queries;
using Contract.Services.Phase.ShareDto;
using Domain.Exceptions.Phases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UserCases.Queries.Phases;

public sealed class GetPhaseByIdQueryHandler
    (
        IPhaseRepository _phaseRepository,
        IMapper _mapper
    ) : IQueryHandler<GetPhaseByIdQuery, PhaseResponse>
{
    public async Task<Result.Success<PhaseResponse>> Handle(GetPhaseByIdQuery request, CancellationToken cancellationToken)
    {
        var phase = await _phaseRepository.GetPhaseById(request.Id);
        if (phase == null)
        {
            throw new PhaseNotFoundException(request.Id);
        }
        var phaseResponse = _mapper.Map<PhaseResponse>(phase);
        return Result.Success<PhaseResponse>.Get(phaseResponse);
    }
}
