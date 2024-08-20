using Application.Abstractions.Data;
using Contract.Services.Material.Create;
using FluentValidation;

namespace Application.UserCases.Commands.Materials.Create;

public sealed class CreateMaterialRequestValidator : AbstractValidator<CreateMaterialRequest>
{
    public CreateMaterialRequestValidator(IMaterialRepository materialRepository)
    {
        RuleFor(m => m.Name)
            .NotEmpty().WithMessage("Tên không được để trống!")
            .NotNull().WithMessage("Tên không được là null!");
        // tên phải là duy nhất
        RuleFor(m => m.Name)
        .MustAsync(async (Name, _) =>
        {
            return !await materialRepository.IsMaterialNameExistedAsync(Name);
        }).WithMessage("Tên đã tồn tại!");
        RuleFor(m => m.Description)
            .MaximumLength(750)
            .WithMessage("Mô tả không được dài quá 750 ký tự!");

        RuleFor(m => m.Unit)
            .NotEmpty().WithMessage("Đơn vị không được để trống")
            .NotNull().WithMessage("Đơn vị không được là null")
            ;
        RuleFor(m => m.QuantityPerUnit)
            .Must(QuantityPerUnit =>
            {
                return QuantityPerUnit > 0;
            }).WithMessage("Số lượng mỗi đơn vị phải lớn hơn 0");
        RuleFor(m => m.QuantityInStock)
            .NotNull().WithMessage("Số lượng tồn kho không được là null!")
            .Must(QuantityInStock =>
            {
                return QuantityInStock >= 0;
            }).WithMessage("Số lượng tồn kho phải lớn hơn hoặc bằng 0");
    }
}
