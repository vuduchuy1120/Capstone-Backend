using Application.Abstractions.Data;
using Contract.Services.User.ConfirmVerifyCode;
using FluentValidation;

namespace Application.UserCases.Commands.Users.ConfirmVerifyCode;

public class ConfirmVerifyCodeValidator : AbstractValidator<ConfirmVerifyCodeCommand>
{
    public ConfirmVerifyCodeValidator()
    {
        RuleFor(req => req.Password)
            .NotEmpty().WithMessage("Mật khẩu không được để trống")
            .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự")
            .Matches(@"[a-z]").WithMessage("Mật khẩu phải chứa ít nhất một chữ cái thường")
            .Matches(@"[A-Z]").WithMessage("Mật khẩu phải chứa ít nhất một chữ cái hoa")
            .Matches(@"[\W_]").WithMessage("Mật khẩu phải chứa ít nhất một ký tự đặc biệt");

    }
}
