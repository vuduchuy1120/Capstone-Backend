using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.MaterialHistory.Create;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using FluentValidation;

namespace Application.UserCases.Commands.MaterialHistories.Create;

public sealed class CreateMaterialHistoryCommandHandler(
    IMaterialHistoryRepository _materialHistoryRepository,
    IUnitOfWork _unitOfWork,
    IValidator<CreateMaterialHistoryRequest> _validator
    ) : ICommandHandler<CreateMaterialHistoryCommand>
{
    public async Task<Result.Success> Handle(CreateMaterialHistoryCommand request, CancellationToken cancellationToken)
    {
        var createMaterialHistoryCommand = request.createMaterialHistoryRequest;
        var validationResult = await _validator.ValidateAsync(createMaterialHistoryCommand);
        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }
        var materialHistory = MaterialHistory.Create(createMaterialHistoryCommand);
        _materialHistoryRepository.AddMaterialHistory(materialHistory);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success.Create();
    }
}
