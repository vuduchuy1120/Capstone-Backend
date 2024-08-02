using Domain.Abstractions.Exceptions.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.EmployeeProducts;

public class EmployeeProductCannotCreateException : MyException
{
    public EmployeeProductCannotCreateException() : base(
        (int)HttpStatusCode.BadRequest, "Không thể tạo hoặc sửa sản phẩm cho nhân viên này do tháng này đã được tính lương")
    {
    }
}
