using Contract.Services.Company.Create;
using Contract.Services.Company.Shared;
using FluentValidation;

namespace Application.UserCases.Commands.Companies.Creates;

public sealed class CreateCompanyRequestValidator : AbstractValidator<CreateCompanyRequest>
{
    public CreateCompanyRequestValidator()
    {
        RuleFor(x => x.CompanyRequest.Name)
            .NotEmpty().WithMessage("Tên công ty là bắt buộc.")
            .NotNull().WithMessage("Tên công ty không được bỏ trống.")
            .MaximumLength(100)
            .WithMessage("Tên công ty không được vượt quá 100 ký tự.");

        RuleFor(x => x.CompanyRequest.Address)
            .NotEmpty().WithMessage("Địa chỉ là bắt buộc.")
            .NotNull().WithMessage("Địa chỉ không được bỏ trống.")
            .MaximumLength(100)
            .WithMessage("Địa chỉ không được vượt quá 100 ký tự.");

        RuleFor(x => x.CompanyRequest.DirectorName)
            .NotEmpty().WithMessage("Tên giám đốc là bắt buộc.")
            .NotNull().WithMessage("Tên giám đốc không được bỏ trống.")
            .Matches(@"^[a-zA-ZÀ-ỹ\s]*$").WithMessage("Tên giám đốc chỉ được chứa ký tự và khoảng trắng.")
            .MaximumLength(100)
            .WithMessage("Tên giám đốc không được vượt quá 100 ký tự.");

        RuleFor(x => x.CompanyRequest.DirectorPhone)
            .NotEmpty().WithMessage("Số điện thoại là bắt buộc.")
            .NotNull().WithMessage("Số điện thoại không được bỏ trống.")
            .Matches(@"^0\d{9}$")
            .WithMessage("Số điện thoại phải đúng 10 ký tự.");

        RuleFor(x => x.CompanyRequest.Email)
            .Matches(@"^$|^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$").WithMessage("Email không hợp lệ.")
            .MaximumLength(100)
            .WithMessage("Email không được vượt quá 100 ký tự.");

        RuleFor(x => x.CompanyRequest.CompanyType)
            .IsInEnum().WithMessage("Loại công ty phải là 0, 1, hoặc 2.");
    }

}
