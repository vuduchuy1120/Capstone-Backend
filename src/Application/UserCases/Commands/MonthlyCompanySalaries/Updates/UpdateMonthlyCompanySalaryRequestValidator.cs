using Application.Abstractions.Data;
using Contract.Services.MonthlyCompanySalary.Updates;
using FluentValidation;

namespace Application.UserCases.Commands.MonthlyCompanySalaries.Updates;

public class UpdateMonthlyCompanySalaryRequestValidator : AbstractValidator<UpdateMonthlyCompanySalaryRequest>
{
    public UpdateMonthlyCompanySalaryRequestValidator(
        IMonthlyCompanySalaryRepository _monthlyCompanySalaryRepository,
        ICompanyRepository _companyRepository)
    {
        RuleFor(x => x.CompanyId)
            .MustAsync(async (CompanyId, cancellation) => await _companyRepository.IsThirdPartyCompanyAsync(CompanyId))
            .WithMessage("CompanyId không tìm thấy hoặc không phải là công ty thứ 3.");
        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12)
            .WithMessage("Tháng phải nằm trong khoảng từ 1 đến 12.");
        RuleFor(x => x.Year)
            .InclusiveBetween(2000, 2100)
            .WithMessage("Năm phải nằm trong khoảng từ 2000 đến 2100.");
        RuleFor(x => new { x.CompanyId, x.Month, x.Year })
            .MustAsync(async (request, cancellation) => await _monthlyCompanySalaryRepository.IsExistMonthlyCompanySalary(request.CompanyId, request.Month, request.Year))
            .WithMessage("Không tìm thấy bảng lương tháng này.");
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Trạng thái chỉ có thể là Chưa thanh toán (0) hoặc Đã thanh toán (1).");
    }
}
