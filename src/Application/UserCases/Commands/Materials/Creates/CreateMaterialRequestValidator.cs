
using Application.Abstractions.Data;
using Contract.Services.Material.Create;
using FluentValidation;

namespace Application.UserCases.Commands.Materials.Create;

public sealed class CreateMaterialRequestValidator : AbstractValidator<CreateMaterialRequest>
{
    public CreateMaterialRequestValidator(IMaterialRepository materialRepository)
    {
        RuleFor(m => m.Name)
            .NotEmpty().WithMessage("Name must be not empty!");

        RuleFor(m => m.Description)
            .MaximumLength(750)
            .WithMessage("Description should be less than 750 characters!");
        RuleFor(m => m.Unit)
            .NotEmpty().WithMessage("Unit must be not empty");
        RuleFor(m => m.QuantityPerUnit)
            .NotEmpty().WithMessage("QuantityPerUnit must be not empty!")
            .Must(QuantityPerUnit =>
            {
                return QuantityPerUnit > 0;
            }).WithMessage("QuantityPerUnit must be greater than 0");
        RuleFor(m => m.QuantityInStock)
            .NotEmpty().WithMessage("QuantityInStock must be not empty!")
            .Must(QuantityInStock =>
            {
                return QuantityInStock >= 0;
            }).WithMessage("QuantityInStock must be greater than 0");
    }
}
