using Application.Abstractions.Data;
using Contract.Services.User.ConfirmVerifyCode;
using FluentValidation;

namespace Application.UserCases.Commands.Users.ConfirmVerifyCode;

public class ConfirmVerifyCodeValidator : AbstractValidator<ConfirmVerifyCodeCommand>
{
    public ConfirmVerifyCodeValidator()
    {
        RuleFor(req => req.Password)
            .NotEmpty().WithMessage("Password cannot be empty")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[\W_]").WithMessage("Password must contain at least one special character");

    }
}
