using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Attendance.ShareDto;

namespace Contract.Services.Attendance.Query;

public record GetAttendancesQuery(
    string? SearchTerm,
    string? Date,
    int SlotId,
    int PageIndex = 1,
    int PageSize = 10) : IQueryHandler<SearchResponse<List<AttendanceResponse>>>;
