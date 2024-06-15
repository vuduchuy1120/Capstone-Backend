using Application.Abstractions.Data;
using Contract.Services.User.UpdateUser;
using FluentValidation;

namespace Application.UserCases.Commands.Users.UpdateUser;

public class UpdateUserValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserValidator(IUserRepository userRepository)
    {
        RuleFor(req => req.Id)
            .NotEmpty().WithMessage("Id cannot be empty")
            .Matches(@"^\d{12}$").WithMessage("Id must be exactly 12 digits")
            .MustAsync(async (id, _) =>
            {
                return await userRepository.IsUserExistAsync(id);
            }).WithMessage("Id already exists");

        RuleFor(req => req.FirstName)
            .NotEmpty().WithMessage("First name cannot be empty")
            .Matches(@"^[a-zA-Z\s]*$").WithMessage("First name can only contain letters and spaces");

        RuleFor(req => req.LastName)
            .NotEmpty().WithMessage("Last name cannot be empty")
            .Matches(@"^[a-zA-Z\s]*$").WithMessage("Last name can only contain letters and spaces");

        RuleFor(req => req.Phone)
            .NotEmpty().WithMessage("Phone number cannot be empty")
            .Matches(@"^\d{10}$").WithMessage("Phone number must be exactly 10 digits");

        RuleFor(req => req.Gender)
                .NotEmpty().WithMessage("Gender cannot be empty")
                .Must(gender => gender == "Male" || gender == "Female")
                .WithMessage("Gender must be either 'Male' or 'Female'");

        RuleFor(req => req.DOB)
                .NotEmpty().WithMessage("Date of birth cannot be empty")
                .Matches(@"^\d{2}/\d{2}/\d{4}$").WithMessage("Date of birth must be in the format dd/MM/yyyy")
                .Must(BeAValidDate).WithMessage("Date of birth must be a valid date in the format dd/MM/yyyy");

        RuleFor(req => req.SalaryByDay)
               .GreaterThanOrEqualTo(0).WithMessage("Salary must be greater than or equal to 0");
    }

    private bool BeAValidDate(string dob)
    {
        return DateTime.TryParseExact(dob, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out _);
    }
}
