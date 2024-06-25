using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Attendance.Queries;
using Contract.Services.Attendance.ShareDto;

namespace Contract.Services.Attendance.Query;

public record GetAttendancesQuery(
    GetAttendanceRequest GetAttendanceRequest,
    Guid CompanyIdClaim,
    string RoleName) : IQueryHandler<SearchResponse<List<AttendanceResponse>>>;
