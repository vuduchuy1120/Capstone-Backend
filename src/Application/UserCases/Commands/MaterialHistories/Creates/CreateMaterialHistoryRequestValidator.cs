using Application.Abstractions.Data;
using Application.Utils;
using Contract.Services.MaterialHistory.Create;
using Domain.Entities;
using FluentValidation;

namespace Application.UserCases.Commands.MaterialHistories.Create;

public sealed class CreateMaterialHistoryRequestValidator : AbstractValidator<CreateMaterialHistoryRequest>
{
    public CreateMaterialHistoryRequestValidator(IMaterialHistoryRepository _materialHistoryRepository, IMaterialRepository _materialRepository)
    {
        RuleFor(m => m.MaterialId)
            .NotEmpty().WithMessage("MaterialId must be not empty!")
            .MustAsync(async (MaterialId, _) =>
            {
                return await _materialRepository.IsMaterialExist(MaterialId);
            }).WithMessage("Material Id not found!");
        RuleFor(m => m.Quantity)
            .NotEmpty().WithMessage("Quantity must be not empty!")
            .Must(quantity => quantity > 0).WithMessage("Quantity must be greater than 0!");
        RuleFor(m => m.Price)
            .NotEmpty().WithMessage("Price must be not empty!")
            .Must(price => price > 0).WithMessage("Price must be greater than 0!");
        RuleFor(x => x.ImportDate)
            .NotEmpty().WithMessage("Import Date cannot be empty")
            .Matches(@"^\d{2}/\d{2}/\d{4}$").WithMessage("Import Date must be in the format dd/MM/yyyy")
            .Must(BeAValidDate).WithMessage("Import Date must be a valid date in the format dd/MM/yyyy");
        // date must > 1900
        RuleFor(x => x.ImportDate)
            .Must(ImportDate =>
            {
                var formatedDate = DateUtil.ConvertStringToDateTimeOnly(ImportDate);
                return formatedDate.Year > 1900 && formatedDate <= DateOnly.FromDateTime(DateTime.Now.Date);
            }).WithMessage("Import Date must be greater than 1900 and less than or equal date now");
    }
    private bool BeAValidDate(string dob)
    {
        return DateTime.TryParseExact(dob, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out _);
    }
}
