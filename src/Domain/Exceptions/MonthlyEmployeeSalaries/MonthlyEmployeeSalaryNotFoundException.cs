using Domain.Abstractions.Exceptions.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.MonthlyEmployeeSalaries;

public class MonthlyEmployeeSalaryNotFoundException : MyException
{

    public MonthlyEmployeeSalaryNotFoundException(int month, int year)
        : base(400, $"Không tìm thấy thông tin lương tháng {month}/{year} của nhân viên này!")
    {
    }
}
