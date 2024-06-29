using Contract.Services.Company.Create;
using FluentValidation;

namespace Application.UserCases.Commands.Companies.Creates;

public sealed class CreateCompanyRequestValidator : AbstractValidator<CreateCompanyRequest>
{
    public CreateCompanyRequestValidator()
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
            .Matches(@"^[a-zA-Z\s]*$").WithMessage("DirectorName must be only characters.")
            .MaximumLength(100)
            .WithMessage("DirectorName cannot exceed 100 characters.");

        RuleFor(x => x.CompanyRequest.DirectorPhone)
            .NotEmpty().WithMessage("Phone is required.")
            .Matches(@"^0\d{9}$")
            .WithMessage("Phone must be exactly 10 chracters.");

        RuleFor(x => x.CompanyRequest.Email)
            .Matches(@"^$|^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$").WithMessage("Email is not valid.")
            .MaximumLength(100)
            .WithMessage("Email cannot exceed 100 characters.");
        RuleFor(x => x.CompanyRequest.CompanyType)
            .IsInEnum().WithMessage("Invalid company type.");

    }

}
