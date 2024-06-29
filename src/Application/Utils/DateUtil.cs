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

    public static DateTime FromDateTimeClientToDateTimeUtc(DateTime dateTimeClient)
    {
        return dateTimeClient.AddHours(-7).ToUniversalTime();
    }

    public static DateTime GetDateTimeForClient(DateTime dateTime)
    {
        return dateTime.AddHours(7);
    }

    public static bool BeAValidDate(string dob)
    {
        return DateTime.TryParseExact(dob, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out _);
    }

    public static bool BeLessThanCurrentDate(string dob)
    {
        if (DateTime.TryParseExact(dob, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDob))
        {
            return parsedDob < DateTime.Today;
        }
        return false;
    }

    public static bool BeMoreThanMinDate(string dob)
    {
        if (DateTime.TryParseExact(dob, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDob))
        {
            return parsedDob > new DateTime(1900, 1, 1);
        }
        return false;
    }
}
