using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Abstractions.Shared.Utils;

public class DateUtils
{
    // plus 7 hour for Datetime.UtcNow
    public static DateTime GetNow()
    {
        return DateTime.UtcNow.AddHours(7);
    }

   
}
