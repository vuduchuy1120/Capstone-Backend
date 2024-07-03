using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Material.Create;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using FluentValidation;

namespace Application.UserCases.Commands.Materials.Create;

public sealed class CreateMaterialCommandHandler
    (IMaterialRepository _materialRepository,
    IUnitOfWork _unitOfWork,
    IValidator<CreateMaterialRequest> validator
    ) : ICommandHandler<CreateMaterialCommand>
{
    public async Task<Result.Success> Handle(CreateMaterialCommand request, CancellationToken cancellationToken)
    {
        var createMaterialRequest = request;
        var validationResult =await validator.ValidateAsync(createMaterialRequest.createMaterialRequest);
        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }

        var material = Material.Create(createMaterialRequest.createMaterialRequest);

        _materialRepository.AddMaterial(material);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success.Create();
    }
}
