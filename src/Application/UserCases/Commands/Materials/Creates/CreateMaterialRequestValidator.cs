
using Application.Abstractions.Data;
using Contract.Services.Material.Create;
using FluentValidation;

namespace Application.UserCases.Commands.Materials.Create;

public sealed class CreateMaterialRequestValidator : AbstractValidator<CreateMaterialRequest>
{
    public CreateMaterialRequestValidator(IMaterialRepository materialRepository)
    {
        RuleFor(m => m.Name)
            .NotEmpty().WithMessage("Name must be not empty!")
            .NotNull().WithMessage("Name must be not null!");
        // name is unique
        RuleFor(m => m.Name)
        .MustAsync(async (Name, _) =>
        {
            return !await materialRepository.IsMaterialNameExistedAsync(Name);
        }).WithMessage("Name is existed!");
        RuleFor(m => m.Description)
            .MaximumLength(750)
            .WithMessage("Description should be less than 750 characters!");

        RuleFor(m => m.Unit)
            .NotEmpty().WithMessage("Unit must be not empty")
            .NotNull().WithMessage("Unit must be not null")
            ;
        RuleFor(m => m.QuantityPerUnit)
            .NotEmpty().WithMessage("QuantityPerUnit must be not empty!")
            .NotNull().WithMessage("QuantityPerUnit must be not null!")
            .Must(QuantityPerUnit =>
            {
                return QuantityPerUnit > 0;
            }).WithMessage("QuantityPerUnit must be greater than 0");
        RuleFor(m => m.QuantityInStock)
            .NotEmpty().WithMessage("QuantityInStock must be not empty!")
            .NotNull().WithMessage("QuantityInStock must be not null!")
            .Must(QuantityInStock =>
            {
                return QuantityInStock >= 0;
            }).WithMessage("QuantityInStock must be greater than 0");
    }
}
