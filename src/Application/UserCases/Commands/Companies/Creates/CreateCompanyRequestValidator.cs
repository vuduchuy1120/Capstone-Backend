using Contract.Services.Company.Create;
using Contract.Services.Company.Shared;
using FluentValidation;

namespace Application.UserCases.Commands.Companies.Creates;

public sealed class CreateCompanyRequestValidator : AbstractValidator<CreateCompanyRequest>
{
    public CreateCompanyRequestValidator()
    {
        RuleFor(x => x.CompanyRequest.Name)
            .NotEmpty().WithMessage("Name is required.")
            .NotNull().WithMessage("Name must be not null")
            .MaximumLength(100)
            .WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.CompanyRequest.Address)
            .NotEmpty().WithMessage("Address is required.")
            .NotNull().WithMessage("Address must be not null")
            .MaximumLength(100)
            .WithMessage("Address cannot exceed 100 characters.");

        RuleFor(x => x.CompanyRequest.DirectorName)
            .NotEmpty().WithMessage("DirectorName is required.")
             .NotNull().WithMessage("DirectorName must be not null")
            .Matches(@"^[a-zA-Z\s]*$").WithMessage("DirectorName must be only characters.")
            .MaximumLength(100)
            .WithMessage("DirectorName cannot exceed 100 characters.");

        RuleFor(x => x.CompanyRequest.DirectorPhone)
            .NotEmpty().WithMessage("Phone is required.")
            .NotNull().WithMessage("Phone must be not null")
            .Matches(@"^0\d{9}$")
            .WithMessage("Phone must be exactly 10 chracters.");

        RuleFor(x => x.CompanyRequest.Email)
            .Matches(@"^$|^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$").WithMessage("Email is not valid.")
            .MaximumLength(100)
            .WithMessage("Email cannot exceed 100 characters.");
        RuleFor(x => x.CompanyRequest.CompanyType)
            .Must(BeCompanyType1).WithMessage("Company type must be 1.");
    }
    private bool BeCompanyType1(CompanyType companyType)
    {
        return companyType == CompanyType.CUSTOMER_COMPANY;
    }

}
