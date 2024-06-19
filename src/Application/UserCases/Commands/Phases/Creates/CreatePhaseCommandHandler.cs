using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Phase.Creates;
using Domain.Abstractions.Exceptions;
using Domain.Entities;

namespace Application.UserCases.Commands.Phases.Creates;

public sealed class CreatePhaseCommandHandler
    (IPhaseRepository _phaseRepository,
    IUnitOfWork _unitOfWork
    ) : ICommandHandler<CreatePhaseCommand>
{
    public async Task<Result.Success> Handle(CreatePhaseCommand request, CancellationToken cancellationToken)
    {
        var createPhase = request.createPhaseRequest;
        if (string.IsNullOrEmpty(createPhase.Name)) throw new MyValidationException("Name is required");
        var phase = Phase.Create(createPhase);
        _phaseRepository.AddPhase(phase);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success.Create();
    }
}
