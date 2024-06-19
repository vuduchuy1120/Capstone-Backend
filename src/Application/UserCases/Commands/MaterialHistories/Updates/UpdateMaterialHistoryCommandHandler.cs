using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.MaterialHistory.Update;
using Domain.Abstractions.Exceptions;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Application.UserCases.Commands.MaterialHistories.Update;

public sealed class UpdateMaterialHistoryCommandHandler(
       IMaterialHistoryRepository _materialHistoryRepository,
       IUnitOfWork _unitOfWork,
       IValidator<UpdateMaterialHistoryRequest> _validator
       ) : ICommandHandler<UpdateMaterialHistoryCommand>
{
    public async Task<Result.Success> Handle(UpdateMaterialHistoryCommand request, CancellationToken cancellationToken)
    {
        var updateMaterialHistoryRequest = request.updateMaterialHistoryRequest;
        var validationResult = await _validator.ValidateAsync(updateMaterialHistoryRequest);

        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }

        if (!string.IsNullOrEmpty(updateMaterialHistoryRequest.ImportDate))
        {
            // Validate format using regex
            if (!Regex.IsMatch(updateMaterialHistoryRequest.ImportDate, @"^\d{2}/\d{2}/\d{4}$"))
            {
                throw new MyValidationException("Import Date must be in the format dd/MM/yyyy");
            }
        }

        var materialHistory = await _materialHistoryRepository.GetMaterialHistoryByIdAsync(updateMaterialHistoryRequest.Id);
        if (materialHistory is null)
        {
            throw new Domain.Exceptions.MaterialHistories.MaterialHistoryNotFoundException();
        }

        materialHistory.Update(updateMaterialHistoryRequest);
        _materialHistoryRepository.UpdateMaterialHistory(materialHistory);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success.Update();

    }
}
