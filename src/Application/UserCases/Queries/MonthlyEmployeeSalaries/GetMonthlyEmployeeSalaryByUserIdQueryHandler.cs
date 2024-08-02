using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.MonthEmployeeSalary.Queries;
using Contract.Services.MonthEmployeeSalary.ShareDtos;
using Domain.Exceptions.MonthlyEmployeeSalaries;
using Domain.Exceptions.Users;

namespace Application.UserCases.Queries.MonthlyEmployeeSalaries;

public sealed class GetMonthlyEmployeeSalaryByUserIdQueryHandler
    (IMonthlyEmployeeSalaryRepository _monthlyEmployeeSalaryRepository,
    IEmployeeProductRepository _employeeProductRepository,
    IAttendanceRepository _attendanceRepository,
    ICloudStorage _cloudStorage
    ) : IQueryHandler<GetMonthlyEmployeeSalaryByUserIdQuery, MonthlyEmployeeSalaryResponse>
{
    public async Task<Result.Success<MonthlyEmployeeSalaryResponse>> Handle(GetMonthlyEmployeeSalaryByUserIdQuery request, CancellationToken cancellationToken)
    {
        await CheckUserPermission(request);

        var monthlyEmployeeSalary = await _monthlyEmployeeSalaryRepository
            .GetMonthlyEmployeeSalaryByUserIdAsync(request.UserId, request.Month, request.Year);
        if (monthlyEmployeeSalary == null)
        {
            throw new MonthlyEmployeeSalaryNotFoundException(request.Month, request.Year);
        }
        var monthPre = request.Month == 1 ? 12 : request.Month - 1;
        var yearPre = request.Month == 1 ? request.Year - 1 : request.Year;
        var monthlyEmployeeSalaryPre = await _monthlyEmployeeSalaryRepository
            .GetMonthlyEmployeeSalaryByUserIdAsync(request.UserId, monthPre, yearPre);
        var SalaryPre = (double)(monthlyEmployeeSalaryPre?.Salary ?? 0);

        var employeeProductDetails = await _employeeProductRepository
            .GetEmployeeProductsByMonthAndYearAndUserId(request.Month, request.Year, request.UserId);

        var productWorkingResponses = employeeProductDetails
            .GroupBy(ep => new {ProductId = ep.ProductId,ProductName = ep.Product.Name, ep.PhaseId, ep.Phase.Name, ep.Phase.Description, ep.Product.Images.FirstOrDefault().ImageUrl })
            .Select(async g => new ProductWorkingResponse(
                ProductId: g.Key.ProductId,
                ProductName: g.Key.ProductName,
                ProductImage: await _cloudStorage.GetSignedUrlAsync(g.Key.ImageUrl ?? "ImageNotFound"),
                PhaseId: g.Key.PhaseId,
                PhaseName: g.Key.Name,
                PhaseDescription: g.Key.Description,
                Quantity: g.Sum(ep => ep.Quantity),
                SalaryPerProduct: g.FirstOrDefault().Product.ProductPhaseSalaries
                                    .Where(pps => pps.PhaseId == g.Key.PhaseId)
                                    .FirstOrDefault()?.SalaryPerProduct ?? 0
            ));


        var attendanceDetails = await _attendanceRepository
            .GetAttendanceByMonthAndUserIdAsync(request.Month, request.Year, request.UserId);

        var attendanceDetailsPreMonth = await _attendanceRepository
            .GetAttendanceByMonthAndUserIdAsync(monthPre, yearPre, request.UserId);

        var totalWorkingDays = (double)attendanceDetails.Count(a => a.IsAttendance && !a.IsSalaryByProduct && (a.SlotId == 1 || a.SlotId == 2)) / 2;
        var totalWorkingDaysPre = (double)attendanceDetailsPreMonth.Count(a => a.IsAttendance && !a.IsSalaryByProduct && (a.SlotId == 1 || a.SlotId == 2)) / 2;

        var totalHourOverTime = attendanceDetails.Where(a => a.IsAttendance && !a.IsSalaryByProduct && a.HourOverTime > 0).Sum(a => a.HourOverTime);
        var totalHourOverTimePre = attendanceDetailsPreMonth.Where(a => a.IsAttendance && !a.IsSalaryByProduct && a.HourOverTime > 0).Sum(a => a.HourOverTime);

        var totalSalaryProduct = productWorkingResponses.Sum(p => p.Result.Quantity * p.Result.SalaryPerProduct);
        double currentSalary = (double)(monthlyEmployeeSalary?.Salary ?? 0);

        double rate = SalaryPre != 0 ? (currentSalary - SalaryPre) * 100 / SalaryPre : -999999999;

        double rateWorkingDays = totalWorkingDaysPre != 0 ? (totalWorkingDays - totalWorkingDaysPre) * 100 / totalWorkingDaysPre : -999999999;
        double rateHourOverTime = totalHourOverTimePre != 0 ? (totalHourOverTime - totalHourOverTimePre) * 100 / totalHourOverTimePre : -999999999;

        rate = Math.Round(rate * 100.0) / 100.0;
        rateWorkingDays = Math.Round(rateWorkingDays * 100.0) / 100.0;
        rateHourOverTime = Math.Round(rateHourOverTime * 100.0) / 100.0;

        var response = new MonthlyEmployeeSalaryResponse(
            Month: request.Month,
            Year: request.Year,
            Salary: monthlyEmployeeSalary.Salary,
            AccountBalance: monthlyEmployeeSalary.User.AccountBalance ?? 0,
            TotalWorkingDays: totalWorkingDays,
            TotalWorkingHours: totalHourOverTime,
            TotalSalaryProduct: totalSalaryProduct,
            Rate: rate,
            RateOverTime: rateHourOverTime,
            RateWorkingDay: rateWorkingDays,
            ProductWorkingResponses: productWorkingResponses.Select(p => p.Result).ToList()
            );

        return Result.Success<MonthlyEmployeeSalaryResponse>.Get(response);
    }

    private async Task CheckUserPermission(GetMonthlyEmployeeSalaryByUserIdQuery request)
    {
        if (request.RoleNameClaim != "MAIN_ADMIN" && request.UserId != request.UserId)
        {
            throw new UserNotPermissionException("Bạn không có quyền xem thông tin lương của nhân viên khác!");
        }
    }
}
