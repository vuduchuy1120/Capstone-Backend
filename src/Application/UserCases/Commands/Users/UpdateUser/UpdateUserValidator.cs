using Application.Abstractions.Data;
using Application.Utils;
using Contract.Services.User.UpdateUser;
using FluentValidation;

namespace Application.UserCases.Commands.Users.UpdateUser;

public class UpdateUserValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserValidator(IUserRepository userRepository, ICompanyRepository companyRepository)
    {
        RuleFor(req => req.Id)
            .NotEmpty().WithMessage("Id không được trống")
            .Matches(@"^\d{9}$|^\d{12}$").WithMessage("ID nhân viên phải là 9 hoặc 12 chữ số")
            .MustAsync(async (id, _) =>
            {
                return await userRepository.IsUserExistAsync(id);
            }).WithMessage("Không tìm thấy id nhân viên");

        RuleFor(req => req.CompanyId)
            .NotNull().WithMessage("Cơ sở của nhân viên không được trống")
            .MustAsync(async (companyId, _) =>
            {
                return await companyRepository.IsCompanyFactoryExistAsync(companyId);
            }).WithMessage("Cơ sở không tồn tại");

        RuleFor(req => req.FirstName)
            .NotEmpty().WithMessage("Họ không được trống")
            .Matches(@"^[a-zA-ZàáảãạâầấẩẫậăằắẳẵặèéẻẽẹêềếểễệìíỉĩịòóỏõọôồốổỗộơờớởỡợùúủũụưừứửữựỳýỷỹỵđÀÁẢÃẠÂẦẤẨẪẬĂẰẮẲẴẶÈÉẺẼẸÊỀẾỂỄỆÌÍỈĨỊÒÓỎÕỌÔỒỐỔỖỘƠỜỚỞỠỢÙÚỦŨỤƯỪỨỬỮỰỲÝỶỸỴĐ\s]*$")
            .WithMessage("Họ chỉ chứa các chữ cái, dấu cách và ký tự tiếng Việt");

        RuleFor(req => req.LastName)
            .NotEmpty().WithMessage("Tên không được trống")
            .Matches(@"^[a-zA-ZàáảãạâầấẩẫậăằắẳẵặèéẻẽẹêềếểễệìíỉĩịòóỏõọôồốổỗộơờớởỡợùúủũụưừứửữựỳýỷỹỵđÀÁẢÃẠÂẦẤẨẪẬĂẰẮẲẴẶÈÉẺẼẸÊỀẾỂỄỆÌÍỈĨỊÒÓỎÕỌÔỒỐỔỖỘƠỜỚỞỠỢÙÚỦŨỤƯỪỨỬỮỰỲÝỶỸỴĐ\s]*$")
            .WithMessage("Tên chỉ chứa các chữ cái, dấu cách và ký tự tiếng Việt");

        RuleFor(req => req.Phone)
            .NotEmpty().WithMessage("Số điện thoại không được trống")
            .Matches(@"^\d{10}$").WithMessage("Số điện thoại phải có đúng 10 chữ số")
            .MustAsync(async (req, phone, _) =>
            {
                return !await userRepository.IsUpdatePhoneNumberExistAsync(phone, req.Id);
            }).WithMessage("Số điện thoại đã được sử dụng");

        RuleFor(req => req.Gender)
                .NotEmpty().WithMessage("Giới tính không được trống")
                .Must(gender => gender == "Male" || gender == "Female")
                .WithMessage("Giới tính phải là 'Nam' hoặc 'Nữ'");

        RuleFor(req => req.DOB)
                 .NotEmpty().WithMessage("Ngày sinh không được trống")
                 .Matches(@"^\d{2}/\d{2}/\d{4}$").WithMessage("Ngày sinh phải đúng định dạng dd/MM/yyyy")
                 .Must(DateUtil.BeAValidDate).WithMessage("Ngày sinh phải là một ngày hợp lệ theo định dạng dd/MM/yyyy")
                 .Must(DateUtil.BeLessThanCurrentDate).WithMessage("Ngày sinh phải nhỏ hơn ngày hiện tại")
                 .Must(DateUtil.BeMoreThanMinDate).WithMessage("Ngày sinh phải lớn hơn 01/01/1900");

        RuleFor(req => req.SalaryByDayRequest)
            .NotEmpty().WithMessage("SalaryByDayRequest không được trống");
        RuleFor(req => req.SalaryOverTimeRequest)
            .NotEmpty().WithMessage("SalaryOverTimeRequest không được trống");
        RuleFor(req => req.SalaryByDayRequest.Salary)
            .NotEmpty().WithMessage("Salary không được trống")
            .GreaterThanOrEqualTo(0).WithMessage("Lương phải lớn hơn hoặc bằng 0");
        RuleFor(req => req.SalaryByDayRequest.StartDate)
            .Must(DateUtil.BeAValidDate).WithMessage("Ngày bắt đầu tính lương phải là một ngày hợp lệ theo định dạng dd/MM/yyyy")
            .Must(DateUtil.BeLessThanOrEqualCurrentDate).WithMessage("Ngày bắt đầu tính lương phải nhỏ hơn ngày hiện tại")
            .Must(DateUtil.BeMoreThanMinDate).WithMessage("Ngày bắt đầu tính lương phải lớn hơn 01/01/1900");
        RuleFor(req => req.SalaryOverTimeRequest.Salary)
            .NotEmpty().WithMessage("Salary không được trống")
            .GreaterThanOrEqualTo(0).WithMessage("Lương phải lớn hơn hoặc bằng 0");
        RuleFor(req => req.SalaryOverTimeRequest.StartDate)
            .Must(DateUtil.BeAValidDate).WithMessage("Ngày bắt đầu tính lương phải là một ngày hợp lệ theo định dạng dd/MM/yyyy")
            .Must(DateUtil.BeLessThanOrEqualCurrentDate).WithMessage("Ngày bắt đầu tính lương phải nhỏ hơn ngày hiện tại")
            .Must(DateUtil.BeMoreThanMinDate).WithMessage("Ngày bắt đầu tính lương phải lớn hơn 01/01/1900");
    }

}
