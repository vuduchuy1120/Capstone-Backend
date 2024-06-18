using Application.Abstractions.Data;
using Contract.Services.Phase.Updates;
using FluentValidation;

namespace Application.UserCases.Commands.Phases.Updates;

public sealed class UpdatePhaseRequestValidator : AbstractValidator<UpdatePhaseRequest>
{
    public UpdatePhaseRequestValidator(IPhaseRepository _phaseRepository)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required")
            .MustAsync(async (id, cancellationToken) =>
            {
                return await _phaseRepository.IsExistById(id);
            })
            .WithMessage("Phase not found");
    }
}
