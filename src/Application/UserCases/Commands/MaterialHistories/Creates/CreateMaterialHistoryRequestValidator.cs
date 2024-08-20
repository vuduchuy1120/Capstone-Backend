using Application.Abstractions.Data;
using Application.Utils;
using Contract.Services.MaterialHistory.Create;
using Domain.Entities;
using FluentValidation;

namespace Application.UserCases.Commands.MaterialHistories.Create;

public sealed class CreateMaterialHistoryRequestValidator : AbstractValidator<CreateMaterialHistoryRequest>
{
    public CreateMaterialHistoryRequestValidator(IMaterialRepository _materialRepository)
    {
        RuleFor(m => m.MaterialId)
            .NotEmpty().WithMessage("Mã vật liệu không được để trống!")
            .MustAsync(async (MaterialId, _) =>
            {
                return await _materialRepository.IsMaterialExist(MaterialId);
            }).WithMessage("Mã vật liệu không tồn tại!");

        RuleFor(m => m.Quantity)
            .NotEmpty().WithMessage("Số lượng không được để trống!")
            .Must(quantity => quantity > 0).WithMessage("Số lượng phải lớn hơn 0!");

        RuleFor(m => m.Price)
            .NotEmpty().WithMessage("Giá không được để trống!")
            .Must(price => price > 0).WithMessage("Giá phải lớn hơn 0!");

        RuleFor(x => x.ImportDate)
            .NotEmpty().WithMessage("Ngày nhập không được để trống!")
            .Matches(@"^\d{2}/\d{2}/\d{4}$").WithMessage("Ngày nhập phải theo định dạng dd/MM/yyyy")
            .Must(BeAValidDate).WithMessage("Ngày nhập phải là một ngày hợp lệ theo định dạng dd/MM/yyyy");

        RuleFor(x => x.ImportDate)
            .Must(ImportDate =>
            {
                var formatedDate = DateUtil.ConvertStringToDateTimeOnly(ImportDate);
                return formatedDate.Year > 1900 && formatedDate <= DateOnly.FromDateTime(DateTime.Now.Date);
            }).WithMessage("Ngày nhập phải lớn hơn năm 1900 và nhỏ hơn hoặc bằng ngày hiện tại");
    }

    private bool BeAValidDate(string dob)
    {
        return DateTime.TryParseExact(dob, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out _);
    }
}
