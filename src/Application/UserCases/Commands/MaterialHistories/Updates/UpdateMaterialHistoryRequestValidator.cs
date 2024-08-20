using Application.Abstractions.Data;
using Application.Utils;
using Contract.Services.MaterialHistory.Update;
using FluentValidation;

namespace Application.UserCases.Commands.MaterialHistories.Update;

public sealed class UpdateMaterialHistoryRequestValidator : AbstractValidator<UpdateMaterialHistoryRequest>
{
    public UpdateMaterialHistoryRequestValidator(IMaterialHistoryRepository _materialHistoryRepository, IMaterialRepository _materialRepository)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id là bắt buộc")
            .MustAsync(async (id, cancellationToken) =>
            {
                return await _materialHistoryRepository.IsMaterialHistoryExist(id);
            })
            .WithMessage("Lịch sử vật liệu không tồn tại");

        RuleFor(x => x.MaterialId)
            .NotEmpty()
            .WithMessage("Mã vật liệu là bắt buộc")
            .MustAsync(async (materialId, cancellationToken) =>
            {
                return await _materialRepository.IsMaterialExist(materialId);
            })
            .WithMessage("Vật liệu không tồn tại");

        RuleFor(x => x.Quantity)
            .NotEmpty()
            .WithMessage("Số lượng là bắt buộc")
            .GreaterThan(0)
            .WithMessage("Số lượng phải lớn hơn 0");

        RuleFor(x => x.ImportDate)
            .NotEmpty().WithMessage("Ngày nhập không được để trống")
            .Matches(@"^\d{2}/\d{2}/\d{4}$").WithMessage("Ngày nhập phải theo định dạng dd/MM/yyyy")
            .Must(BeAValidDate).WithMessage("Ngày không hợp lệ");

        RuleFor(x => x.ImportDate)
            .Must(ImportDate =>
            {
                var formatedDate = DateUtil.ConvertStringToDateTimeOnly(ImportDate);
                return formatedDate.Year > 1900 && formatedDate <= DateOnly.FromDateTime(DateTime.Now.Date);
            }).WithMessage("Ngày nhập phải lớn hơn năm 1900 và nhỏ hơn hoặc bằng ngày hiện tại");

        RuleFor(x => x.Price)
            .NotEmpty()
            .WithMessage("Giá là bắt buộc")
            .GreaterThan(0)
            .WithMessage("Giá phải lớn hơn 0");
    }

    private bool BeAValidDate(string dob)
    {
        return DateTime.TryParseExact(dob, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out _);
    }
}
