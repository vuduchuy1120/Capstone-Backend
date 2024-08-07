using Contract.Services.User.ChangePassword;
using FluentValidation;

namespace Application.UserCases.Commands.Users.ChangePassword;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordValidator()
    {
        RuleFor(req => req.newPassword)
            .NotEmpty().WithMessage("Mật khẩu không được để trống")
            .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự")
            .Matches(@"[a-z]").WithMessage("Mật khẩu phải chứa ít nhất một chữ cái thường")
            .Matches(@"[A-Z]").WithMessage("Mật khẩu phải chứa ít nhất một chữ cái hoa")
            .Matches(@"[\W_]").WithMessage("Mật khẩu phải chứa ít nhất một ký tự đặc biệt");

    }
}
