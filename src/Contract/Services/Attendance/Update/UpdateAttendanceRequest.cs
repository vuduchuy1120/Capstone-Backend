using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Services.Attendance.Update;

public record UpdateAttendanceRequest
(
    string UserId,
    int SlotId,
    string Date,
    double HourOverTime,
    bool IsAttendance,
    bool IsOverTime,
    bool IsSalaryByProduct
    );
