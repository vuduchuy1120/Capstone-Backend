using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.MaterialHistory.Deletes;
using Domain.Exceptions.MaterialHistories;

namespace Application.UserCases.Commands.MaterialHistories;

public sealed class DeleteMaterialHistoryCommandHandler
    (IMaterialHistoryRepository _materialHistoryRepository,
    IMaterialRepository _materialRepository,
    IUnitOfWork _unitOfWork
    ) : ICommandHandler<DeleteMaterialHistoryByIdCommand>
{
    public async Task<Result.Success> Handle(DeleteMaterialHistoryByIdCommand request, CancellationToken cancellationToken)
    {
        var materialHistory = await _materialHistoryRepository.GetMaterialHistoryByIdAsync(request.MaterialHistoryId);
        if (materialHistory == null)
        {
            throw new MaterialHistoryNotFoundException(request.MaterialHistoryId);
        }
        var material = await _materialRepository.GetMaterialByIdAsync(materialHistory.MaterialId);

        if (materialHistory.Quantity > material.AvailableQuantity)
        {
            throw new MaterialHistoryCannotBeDeleteException();
        }
        _materialHistoryRepository.DeleteMaterialHistory(materialHistory);
        material.UpdateQuantityInStock1(-materialHistory.Quantity);
        _materialRepository.UpdateMaterial(material);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success.Delete();
    }
}
