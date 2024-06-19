using Application.Abstractions.Data;
using Application.Utils;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Attendance.Queries;
using Contract.Services.Attendance.ShareDto;


namespace Application.UserCases.Queries.Attendances;

public sealed class GetAttendanceOverallQueryHandler
    (IAttendanceRepository attendanceRepository) : IQueryHandler<GetAttendanceOverallQuery, SearchResponse<List<AttendanceOverallResponse>>>
{
    public async Task<Result.Success<SearchResponse<List<AttendanceOverallResponse>>>> Handle(GetAttendanceOverallQuery request, CancellationToken cancellationToken)
    {
        DateOnly? startDate = null;
        DateOnly? endDate = null;
        if (request.StartDate != null)
        {
            startDate = DateUtil.ConvertStringToDateTimeOnly(request.StartDate);
        }
        if (request.EndDate != null)
        {
            endDate = DateUtil.ConvertStringToDateTimeOnly(request.EndDate);
        }
        var attendances = await attendanceRepository.GetAttendanceOverallAsync(startDate, endDate, request.PageIndex, request.PageSize);

        return Result.Success<SearchResponse<List<AttendanceOverallResponse>>>.Get(attendances);
    }
}
