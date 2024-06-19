using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Services.Attendance.Create;

public record CreateAttendanceDefaultRequest
(
    int slotId,
    List<CreateAttendanceWithoutSlotIdRequest> CreateAttendances
    );

