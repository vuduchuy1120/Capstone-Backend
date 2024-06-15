using Contract.Abstractions.Exceptions;

namespace Contract.Abstractions.Shared.Utils;

public class DateUtils
{
    // plus 7 hour for Datetime.UtcNow
    public static DateTime GetNow()
    {
        return DateTime.UtcNow.AddHours(7);
    }

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
}
