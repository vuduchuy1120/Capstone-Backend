using Application.Abstractions.Data;
using Application.Utils;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Attendance.Queries;
using Contract.Services.Attendance.ShareDto;
using Contract.Services.Attendance.ShareDtos;
using Contract.Services.EmployeeProduct.ShareDto;
using Domain.Entities;
using Domain.Exceptions.Attendances;

namespace Application.UserCases.Queries.Attendances;

public sealed class GetAttendanceByUserIdSlotIdAndDateQueryHandler
    (IAttendanceRepository _attendanceRepository,
    IMapper _mapper
    ) : IQueryHandler<GetAttendanceByUserIdSlotIdAndDateQuery, AttendanceUserDetailResponse>
{
    public async Task<Result.Success<AttendanceUserDetailResponse>> Handle(GetAttendanceByUserIdSlotIdAndDateQuery request, CancellationToken cancellationToken)
    {
        var formatedDate = DateUtil.ConvertStringToDateTimeOnly(request.Date);
        var attendance = await _attendanceRepository.GetAttendanceByUserIdSlotIdAndDate(request.UserId, request.SlotId, formatedDate);
        if (attendance == null)
        {
            throw new AttendanceNotFoundException();
        }

        var response = new AttendanceUserDetailResponse
        (
            SlotId: attendance.SlotId,
            Date: request.Date,
            HourOverTime: attendance.HourOverTime,
            IsAttendance: attendance.IsAttendance,
            IsOvertime: attendance.IsOverTime,
            IsSalaryByProduct: attendance.IsSalaryByProduct,
            EmployeeProductResponses: attendance.User.EmployeeProducts
                  .Where(ep => ep.Date == attendance.Date && ep.SlotId == attendance.SlotId && ep.UserId == attendance.UserId)
                  .Select(ep => new EmployeeProductResponse
                  (
                      ImageUrl: ep.Product.Images.FirstOrDefault()?.ImageUrl ?? string.Empty,
                      ProductName: ep.Product.Name,
                      ProductId: ep.Product.Id,
                      PhaseId: ep.Phase.Id,
                      PhaseName: ep.Phase.Name,
                      Quantity: ep.Quantity
                  ))
                  .ToList()
        );
        return Result.Success<AttendanceUserDetailResponse>.Get(response);
    }
}
