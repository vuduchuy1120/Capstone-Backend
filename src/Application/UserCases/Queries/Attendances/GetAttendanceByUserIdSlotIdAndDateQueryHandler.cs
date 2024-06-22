using Application.Abstractions.Data;
using Application.Utils;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Attendance.Queries;
using Contract.Services.Attendance.ShareDtos;
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
        var query = await _attendanceRepository.GetAttendanceByUserIdSlotIdAndDate(request.UserId, request.SlotId, formatedDate);
        if (query == null)
        {
            throw new AttendanceNotFoundException();
        }
        var attendance = _mapper.Map<AttendanceUserDetailResponse>(query);
        return Result.Success<AttendanceUserDetailResponse>.Get(attendance);
    }
}
