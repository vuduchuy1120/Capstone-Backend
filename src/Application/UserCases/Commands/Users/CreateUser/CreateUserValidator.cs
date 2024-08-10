using Application.Abstractions.Data;
using Application.Utils;
using Contract.Services.User.CreateUser;
using FluentValidation;

namespace Application.UserCases.Commands.Users.CreateUser;

public sealed class CreateUserValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserValidator(IUserRepository userRepository, ICompanyRepository companyRepository)
    {
        RuleFor(req => req.Id)
            .NotEmpty().WithMessage("Id không được để trống")
            .Matches(@"^\d{9}$|^\d{12}$").WithMessage("ID nhân viên phải là 9 hoặc 12 chữ số")
            .MustAsync(async (id, _) =>
            {
                return !await userRepository.IsUserExistAsync(id);
            }).WithMessage("Id nhân viên đã tồn tại");

        RuleFor(req => req.CompanyId)
            .NotNull().WithMessage("Cơ sở của nhân viên không được trống")
            .MustAsync(async (companyId, _) =>
            {
                return await companyRepository.IsCompanyFactoryExistAsync(companyId);
            }).WithMessage("Cơ sở không tồn tại");

        RuleFor(req => req.FirstName)
            .NotEmpty().WithMessage("Họ không được để trống")
            .Matches(@"^[a-zA-ZàáảãạâầấẩẫậăằắẳẵặèéẻẽẹêềếểễệìíỉĩịòóỏõọôồốổỗộơờớởỡợùúủũụưừứửữựỳýỷỹỵđÀÁẢÃẠÂẦẤẨẪẬĂẰẮẲẴẶÈÉẺẼẸÊỀẾỂỄỆÌÍỈĨỊÒÓỎÕỌÔỒỐỔỖỘƠỜỚỞỠỢÙÚỦŨỤƯỪỨỬỮỰỲÝỶỸỴĐ\s]*$")
            .WithMessage("Họ chỉ chứa các chữ cái, dấu cách và ký tự tiếng Việt");

        RuleFor(req => req.LastName)
            .NotEmpty().WithMessage("Tên không được để trống")
            .Matches(@"^[a-zA-ZàáảãạâầấẩẫậăằắẳẵặèéẻẽẹêềếểễệìíỉĩịòóỏõọôồốổỗộơờớởỡợùúủũụưừứửữựỳýỷỹỵđÀÁẢÃẠÂẦẤẨẪẬĂẰẮẲẴẶÈÉẺẼẸÊỀẾỂỄỆÌÍỈĨỊÒÓỎÕỌÔỒỐỔỖỘƠỜỚỞỠỢÙÚỦŨỤƯỪỨỬỮỰỲÝỶỸỴĐ\s]*$")
            .WithMessage("Tên chỉ chứa các chữ cái, dấu cách và ký tự tiếng Việt");


        RuleFor(req => req.Phone)
            .NotEmpty().WithMessage("Số điện thoại không được để trống")
            .Matches(@"^\d{10}$").WithMessage("Số điện thoại phải có đúng 10 chữ số")
            .MustAsync(async (phone, _) =>
            {
                return !await userRepository.IsPhoneNumberExistAsync(phone);
            }).WithMessage("Số điện thoại đã được sử dụng");

        //RuleFor(req => req.Password)
        //    .NotEmpty().WithMessage("Mật khẩu không được trống")
        //    .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự")
        //    .Matches(@"[a-z]").WithMessage("Mật khẩu phải chứa ít nhất một chữ cái thường")
        //    .Matches(@"[A-Z]").WithMessage("Mật khẩu phải chứa ít nhất một chữ cái hoa")
        //    .Matches(@"[\W_]").WithMessage("Mật khẩu phải chứa ít nhất một ký tự đặc biệt");

        RuleFor(req => req.Gender)
                .NotEmpty().WithMessage("Gender cannot be empty")
                .Must(gender => gender == "Male" || gender == "Female")
                .WithMessage("Giới tính phải là 'Male' hoặc 'Female'");

        RuleFor(req => req.DOB)
                 .NotEmpty().WithMessage("Ngày sinh không được trống")
                 .Matches(@"^\d{2}/\d{2}/\d{4}$").WithMessage("Ngày sinh phải đúng định dạng dd/MM/yyyy")
                 .Must(DateUtil.BeAValidDate).WithMessage("Ngày sinh phải là một ngày hợp lệ theo định dạng dd/MM/yyyy")
                 .Must(DateUtil.BeLessThanCurrentDate).WithMessage("Ngày sinh phải nhỏ hơn ngày hiện tại")
                 .Must(DateUtil.BeMoreThanMinDate).WithMessage("Ngày sinh phải lớn hơn 01/01/1900");

        RuleFor(req => req.SalaryByDayRequest)
            .NotEmpty().WithMessage("Lương theo ngày không được trống");
        RuleFor(req => req.SalaryOverTimeRequest)
            .NotEmpty().WithMessage("Lương tăng ca không được trống");
        RuleFor(req => req.SalaryByDayRequest.Salary)
            .NotEmpty().WithMessage("Lương không được trống")
            .GreaterThanOrEqualTo(0).WithMessage("Lương phải lớn hơn hoặc bằng 0");
        RuleFor(req => req.SalaryByDayRequest.StartDate)
            .Must(DateUtil.BeAValidDate).WithMessage("Ngày bắt đầu tính lương phải là một ngày hợp lệ theo định dạng dd/MM/yyyy")
            .Must(DateUtil.BeLessThanOrEqualCurrentDate).WithMessage("Ngày bắt đầu tính lương phải nhỏ hơn ngày hiện tại")
            .Must(DateUtil.BeMoreThanMinDate).WithMessage("Ngày bắt đầu tính lương phải lớn hơn 01/01/1900");
        RuleFor(req => req.SalaryOverTimeRequest.Salary)
            .NotEmpty().WithMessage("Lương không được trống")
            .GreaterThanOrEqualTo(0).WithMessage("Lương phải lớn hơn hoặc bằng 0");
        RuleFor(req => req.SalaryOverTimeRequest.StartDate)
            .Must(DateUtil.BeAValidDate).WithMessage("Ngày bắt đầu tính lương phải là một ngày hợp lệ theo định dạng dd/MM/yyyy")
            .Must(DateUtil.BeLessThanOrEqualCurrentDate).WithMessage("Ngày bắt đầu tính lương phải nhỏ hơn ngày hiện tại")
            .Must(DateUtil.BeMoreThanMinDate).WithMessage("Ngày bắt đầu tính lương phải lớn hơn 01/01/1900");
    }

}
