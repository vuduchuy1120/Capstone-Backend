using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Services.Attendance.ShareDto;

public record AttendanceOverallResponse
(
    DateOnly? Date,
    List<AttendanceStatisticResponse> AttendanceStatisticResponses
    );