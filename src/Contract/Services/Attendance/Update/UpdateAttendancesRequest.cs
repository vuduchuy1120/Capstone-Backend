using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Services.Attendance.Update;

public record UpdateAttendancesRequest
(
    int SlotId,
    List<UpdateAttendanceWithoutSlotIdRequest> UpdateAttendances
    );
