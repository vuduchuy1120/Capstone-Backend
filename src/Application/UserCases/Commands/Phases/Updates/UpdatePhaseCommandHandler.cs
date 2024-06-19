using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Phase.Updates;
using Domain.Abstractions.Exceptions;
using FluentValidation;

namespace Application.UserCases.Commands.Phases.Updates;

public sealed class UpdatePhaseCommandHandler
    (IPhaseRepository _phaseRepository,
    IUnitOfWork _unitOfWork,
    IValidator<UpdatePhaseRequest> _validator
    ) : ICommandHandler<UpdatePhaseCommand>
{
    public async Task<Result.Success> Handle(UpdatePhaseCommand request, CancellationToken cancellationToken)
    {
        var validator = await _validator.ValidateAsync(request.updatePhaseRequest, cancellationToken);
        if (!validator.IsValid)
        {
            throw new MyValidationException(validator.ToDictionary());
        }
        var phase = await _phaseRepository.GetPhaseById(request.updatePhaseRequest.Id);
        phase.Update(request.updatePhaseRequest);
        _phaseRepository.UpdatePhase(phase);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success.Update();
    }
}

