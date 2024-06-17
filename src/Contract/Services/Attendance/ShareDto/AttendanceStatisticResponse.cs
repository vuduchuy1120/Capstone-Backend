using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Services.Attendance.ShareDto;

public record AttendanceStatisticResponse
(
    int SlotId,
    int TotalAttendance,
    int TotalManufacture,
    int TotalSalaryByProduct,
    double TotalHourOverTime,
    int NumberOfPresent
    );
