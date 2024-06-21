using Application.Abstractions.Data;
using Contract.Services.Company.Updates;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UserCases.Commands.Companies.Updates;

public sealed class UpdateCompanyRequestValidator : AbstractValidator<UpdateCompanyRequest>
{
    public UpdateCompanyRequestValidator(ICompanyRepository _companyRepository)
    {
        RuleFor(x => x.CompanyRequest.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage("Name cannot exceed 100 characters.");
        RuleFor(x => x.CompanyRequest.Address)
            .NotEmpty().WithMessage("Address is required.")
            .MaximumLength(100)
            .WithMessage("Address cannot exceed 100 characters.");
        RuleFor(x => x.CompanyRequest.DirectorName)
            .NotEmpty().WithMessage("DirectorName is required.")
            .MaximumLength(100)
            .WithMessage("DirectorName cannot exceed 100 characters.");
        RuleFor(x => x.CompanyRequest.DirectorPhone)
            .NotEmpty().WithMessage("Phone is required.")
            .MaximumLength(10)
            .Matches(@"^0\d{9}$")
            .WithMessage("Phone cannot exceed 10 chracters.");
        RuleFor(x => x.CompanyRequest.Email)
            .EmailAddress().WithMessage("Email is not valid.")
            .MaximumLength(100)
            .WithMessage("Email cannot exceed 100 characters.");
        RuleFor(x => x.CompanyRequest.CompanyType)
            .NotEmpty().WithMessage("CompanyType is required.")
            .MaximumLength(100).WithMessage("CompanyType cannot exceed 100 characters.");
        RuleFor(x => x.Id)
            .MustAsync(async (id, _) => await _companyRepository.IsExistAsync(id))
            .WithMessage("CompanyId not found!");

    }
}
