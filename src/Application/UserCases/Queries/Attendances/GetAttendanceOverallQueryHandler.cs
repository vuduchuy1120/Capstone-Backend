using Application.Abstractions.Data;
using Application.Utils;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Attendance.Queries;
using Contract.Services.Attendance.ShareDto;

namespace Application.UserCases.Queries.Attendances
{
    public sealed class GetAttendanceOverallQueryHandler : IQueryHandler<GetAttendanceOverallQuery, SearchResponse<List<AttendanceOverallResponse>>>
    {
        private readonly IAttendanceRepository _attendanceRepository;

        public GetAttendanceOverallQueryHandler(IAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

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

            var (attendances, totalPages) = await _attendanceRepository.GetAttendanceOverallAsync(startDate, endDate, request.PageIndex, request.PageSize);

            if (attendances == null)
            {
                attendances = new List<AttendanceOverallResponse>();
            }

            var searchResponse = new SearchResponse<List<AttendanceOverallResponse>>(request.PageIndex, totalPages, attendances);
            return Result.Success<SearchResponse<List<AttendanceOverallResponse>>>.Get(searchResponse);
        }
    }
}
