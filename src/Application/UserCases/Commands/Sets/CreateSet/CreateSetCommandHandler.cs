using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Set.CreateSet;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using FluentValidation;

namespace Application.UserCases.Commands.Sets.CreateSet;

internal sealed class CreateSetCommandHandler(
    ISetRepository _setRepository,
    IUnitOfWork _unitOfWork,
    IValidator<CreateSetRequest> _validator) : ICommandHandler<CreateSetCommand>
{
    public async Task<Result.Success> Handle(CreateSetCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request.CreateSetRequest);

        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }

        var set = Set.Create(request);
        _setRepository.Add(set);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success.Create();
    }
}
