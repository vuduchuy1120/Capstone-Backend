using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Material.Update;
using Contract.Abstractions.Exceptions;
using Domain.Exceptions.Materials;
using FluentValidation;


namespace Application.UserCases.Commands.Materials.Update;

internal sealed class UpdateMaterialCommandHandler(
    IMaterialRepository _materialRepository,
    IUnitOfWork _unitOfWork,
    IValidator<UpdateMaterialRequest> _validator
    ) : ICommandHandler<UpdateMaterialCommand>
{
    public async Task<Result.Success> Handle(UpdateMaterialCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request.UpdateMaterialRequest);
        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }

        var material = await _materialRepository
            .GetMaterialByIdAsync(request.UpdateMaterialRequest.Id)
            ?? throw new MaterialNotFoundException();

        material.Update(request.UpdateMaterialRequest);
        _materialRepository.UpdateMaterial(material);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success.Update();
    }
}
