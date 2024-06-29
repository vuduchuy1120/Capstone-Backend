using Application.Abstractions.Data;
using Application.Utils;
using Contract.Services.MaterialHistory.Update;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UserCases.Commands.MaterialHistories.Update;

public sealed class UpdateMaterialHistoryRequestValidator : AbstractValidator<UpdateMaterialHistoryRequest>
{
    public UpdateMaterialHistoryRequestValidator(IMaterialHistoryRepository _materialHistoryRepository, IMaterialRepository _materialRepository)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required")
            .MustAsync(async (id, cancellationToken) =>
            {
                return await _materialHistoryRepository.IsMaterialHistoryExist(id);
            })
            .WithMessage("Material history not found");

        RuleFor(x => x.MaterialId)
            .NotEmpty()
            .WithMessage("MaterialId is required")
            .MustAsync(async (materialId, cancellationToken) =>
            {
                return await _materialRepository.IsMaterialExist(materialId);
            })
            .WithMessage("Material not found");

        RuleFor(x => x.Quantity)
            .NotEmpty()
            .WithMessage("Quantity is required")
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");
        RuleFor(x => x.ImportDate)
           .NotEmpty().WithMessage("Import Date cannot be empty")
           .Matches(@"^\d{2}/\d{2}/\d{4}$").WithMessage("Import Date must be in the format dd/MM/yyyy")
           .Must(BeAValidDate).WithMessage("Date is invalid.");
        RuleFor(x => x.ImportDate)
            .Must(ImportDate =>
            {
                var formatedDate = DateUtil.ConvertStringToDateTimeOnly(ImportDate);
                return formatedDate.Year > 1900 && formatedDate <= DateOnly.FromDateTime(DateTime.Now.Date);
            }).WithMessage("Import Date must be greater than 1900 and less than or equal date now");
        RuleFor(x => x.Price)
            .NotEmpty()
            .WithMessage("Price is required")
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0");

    }
    private bool BeAValidDate(string dob)
    {
        return DateTime.TryParseExact(dob, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out _);
    }
}
