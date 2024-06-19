using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Attendance.ShareDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Services.Attendance.Queries;

public record GetAttendanceOverallQuery(
    string? StartDate,
    string? EndDate,
    int PageIndex = 1,
    int PageSize = 10) : IQuery<SearchResponse<List<AttendanceOverallResponse>>>;