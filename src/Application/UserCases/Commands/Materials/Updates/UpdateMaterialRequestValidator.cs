using Application.Abstractions.Data;
using Contract.Services.Material.Update;
using FluentValidation;

namespace Application.UserCases.Commands.Materials.Update;
public sealed class UpdateMaterialRequestValidator : AbstractValidator<UpdateMaterialRequest>
{
    public UpdateMaterialRequestValidator(IMaterialRepository _materialRepository)
    {
        RuleFor(m => m.Id)
            .NotEmpty().WithMessage("Id must be not empty!")
            .NotNull().WithMessage("Id must be not null!")
            .MustAsync(async (id, _) =>
            {
                return await _materialRepository.IsMaterialExist(id);
            }).WithMessage("Material not found!");
        RuleFor(m => m.Name)
            .NotEmpty().WithMessage("Name must be not empty!")
            .MaximumLength(200).WithMessage("Name must be less than 200 characters!")
            .NotNull().WithMessage("Name must be not null!")
            .MustAsync(async (Name, _) =>
            {
                return !await _materialRepository.IsMaterialNameExistedAsync(Name);
            }).WithMessage("Name is existed!");

        RuleFor(m => m.Description)
            .MaximumLength(750)
            .WithMessage("Description should be less than 750 characters!");
        RuleFor(m => m.Unit)
            .NotNull().WithMessage("Unit must be not null")
            .NotEmpty().WithMessage("Unit must be not empty");
        RuleFor(m => m.QuantityPerUnit)
            .Must(quantityPerUnit => !quantityPerUnit.HasValue || quantityPerUnit > 0)
            .WithMessage("QuantityPerUnit must be greater than 0 if specified");
        RuleFor(m => m.QuantityInStock)
            .NotNull().WithMessage("QuantityInStock must be not null")
            .NotEmpty().WithMessage("QuantityInStock must be not empty!")
            .Must(QuantityInStock =>
            {
                return QuantityInStock >= 0;
            }).WithMessage("QuantityInStock must be greater than 0");
    }
}