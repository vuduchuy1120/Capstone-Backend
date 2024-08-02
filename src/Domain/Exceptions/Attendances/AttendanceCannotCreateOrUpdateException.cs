using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Attendances;

public class AttendanceCannotCreateOrUpdateException : MyException
{
    public AttendanceCannotCreateOrUpdateException() : base(
        (int)HttpStatusCode.BadRequest, "Không thể tạo hoặc sửa điểm danh do tháng này đã được tính lương.")
    {
    }
}
