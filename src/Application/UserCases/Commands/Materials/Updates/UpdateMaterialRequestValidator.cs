using Application.Abstractions.Data;
using Contract.Services.Material.Update;
using FluentValidation;

namespace Application.UserCases.Commands.Materials.Update;
public sealed class UpdateMaterialRequestValidator : AbstractValidator<UpdateMaterialRequest>
{
    public UpdateMaterialRequestValidator(IMaterialRepository _materialRepository)
    {
        RuleFor(m => m.Id)
            .NotEmpty().WithMessage("Id không được để trống!")
            .NotNull().WithMessage("Id không được là null!")
            .MustAsync(async (id, _) =>
            {
                return await _materialRepository.IsMaterialExist(id);
            }).WithMessage("Vật liệu không tồn tại!");
        RuleFor(m => m.Name)
            .NotEmpty().WithMessage("Tên không được để trống!")
            .MaximumLength(200).WithMessage("Tên không được dài quá 200 ký tự!")
            .NotNull().WithMessage("Tên không được là null!")
            .MustAsync(async (Name, _) =>
            {
                return !await _materialRepository.IsMaterialNameExistedAsync(Name);
            }).WithMessage("Tên đã tồn tại!");

        RuleFor(m => m.Description)
            .MaximumLength(750)
            .WithMessage("Mô tả không được dài quá 750 ký tự!");
        RuleFor(m => m.Unit)
            .NotNull().WithMessage("Đơn vị không được là null")
            .NotEmpty().WithMessage("Đơn vị không được để trống");
        RuleFor(m => m.QuantityPerUnit)
            .Must(quantityPerUnit => !quantityPerUnit.HasValue || quantityPerUnit > 0)
            .WithMessage("Số lượng mỗi đơn vị phải lớn hơn 0");
        RuleFor(m => m.QuantityInStock)
            .NotNull().WithMessage("Số lượng tồn kho không được là null")
            .Must(QuantityInStock =>
            {
                return QuantityInStock >= 0;
            }).WithMessage("Số lượng tồn kho phải lớn hơn hoặc bằng 0");
    }
}
