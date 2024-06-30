using Domain.Abstractions.Exceptions.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.Common;

public class WrongFormatDateException : MyException
{
    public WrongFormatDateException() : base(
        (int)HttpStatusCode.BadRequest, "Date is wrong format dd/MM/yyyy or date is invalid")
    {
    }
}