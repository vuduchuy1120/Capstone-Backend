using Contract.Services.Attendance.Create;
using Domain.Abstractions.Exceptions;

namespace Application.Utils;

public class DateUtil
{
    public static DateOnly ConvertStringToDateTimeOnly(string dateString)
    {
        string format = "dd/MM/yyyy";

        DateTime dateTime;
        if (DateTime.TryParseExact(dateString, format, null, System.Globalization.DateTimeStyles.None, out dateTime))
        {
            return new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);
        }
        else
        {
            throw new MyValidationException("Date is wrong format");
        }
    }
    public static DateTime GetNow()
    {
        return DateTime.UtcNow.AddHours(7);
    }

    
}
