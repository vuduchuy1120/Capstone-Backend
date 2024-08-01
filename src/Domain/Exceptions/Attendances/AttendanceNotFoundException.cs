using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Attendances;

public class AttendanceNotFoundException : MyException
{
    public AttendanceNotFoundException() : base(
        (int)HttpStatusCode.NotFound, "Không tìm thấy điểm danh")
    {
    }
}

