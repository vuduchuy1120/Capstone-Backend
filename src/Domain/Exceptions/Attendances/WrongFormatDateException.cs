using Domain.Abstractions.Exceptions.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.Attendances;

public class WrongFormatDateException : MyException
{
    public WrongFormatDateException() : base(
        (int)HttpStatusCode.BadRequest, "Dob is wrong format")
    {
    }
}