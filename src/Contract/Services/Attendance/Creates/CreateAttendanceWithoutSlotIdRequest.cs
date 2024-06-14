using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Services.Attendance.Create;

public record CreateAttendanceWithoutSlotIdRequest
(
    string UserId,
    double HourOverTime,
    bool IsAttendance,
    bool IsOverTime,
    bool IsSalaryByProduct
    );