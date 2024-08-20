using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.MaterialHistory.Update;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Application.UserCases.Commands.MaterialHistories.Update;

public sealed class UpdateMaterialHistoryCommandHandler : ICommandHandler<UpdateMaterialHistoryCommand>
{
    private readonly IMaterialHistoryRepository _materialHistoryRepository;
    private readonly IMaterialRepository _materialRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateMaterialHistoryRequest> _validator;

    public UpdateMaterialHistoryCommandHandler(
        IMaterialHistoryRepository materialHistoryRepository,
        IMaterialRepository materialRepository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateMaterialHistoryRequest> validator)
    {
        _materialHistoryRepository = materialHistoryRepository;
        _materialRepository = materialRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result.Success> Handle(UpdateMaterialHistoryCommand request, CancellationToken cancellationToken)
    {
        var updateMaterialHistoryRequest = request.updateMaterialHistoryRequest;
        var validationResult = await _validator.ValidateAsync(updateMaterialHistoryRequest, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }

        ValidateImportDateFormat(updateMaterialHistoryRequest.ImportDate);

        var materialHistory = await GetMaterialHistory(updateMaterialHistoryRequest.Id);
        var material = await GetMaterial(updateMaterialHistoryRequest.MaterialId);

        UpdateMaterialQuantities(material, materialHistory, updateMaterialHistoryRequest.Quantity);

        materialHistory.Update(updateMaterialHistoryRequest);
        _materialHistoryRepository.UpdateMaterialHistory(materialHistory);
        _materialRepository.UpdateMaterial(material);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success.Update();
    }

    private static void ValidateImportDateFormat(string? importDate)
    {
        if (!string.IsNullOrEmpty(importDate) && !Regex.IsMatch(importDate, @"^\d{2}/\d{2}/\d{4}$"))
        {
            throw new MyValidationException("Import Date must be in the format dd/MM/yyyy");
        }
    }

    private async Task<MaterialHistory> GetMaterialHistory(Guid id)
    {
        var materialHistory = await _materialHistoryRepository.GetMaterialHistoryByIdAsync(id);
        if (materialHistory is null)
        {
            throw new Domain.Exceptions.MaterialHistories.MaterialHistoryNotFoundException();
        }
        return materialHistory;
    }

    private async Task<Material> GetMaterial(Guid id)
    {
        var material = await _materialRepository.GetMaterialByIdAsync(id);
        if (material is null)
        {
            throw new Domain.Exceptions.Materials.MaterialNotFoundException();
        }
        return material;
    }

    private void UpdateMaterialQuantities(Material material, MaterialHistory materialHistory, double newQuantity)
    {
        material.UpdateQuantityInStockAndAvailableQuantity(material.QuantityInStock - materialHistory.Quantity);
        material.UpdateQuantityInStockAndAvailableQuantity(material.QuantityInStock + newQuantity);
        _materialRepository.UpdateMaterial(material);
    }
}
