using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.EmployeeProducts;

public class EmployeeProductCannotCreateException : MyException
{
    public EmployeeProductCannotCreateException() : base(
        (int)HttpStatusCode.BadRequest, "Không thể tạo hoặc sửa sản phẩm cho nhân viên này do tháng này đã được tính lương")
    {
    }
}
