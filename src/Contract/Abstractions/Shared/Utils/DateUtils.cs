namespace Contract.Abstractions.Shared.Utils;

public class DateUtils
{
    public static DateTime GetNow()
    {
        return DateTime.UtcNow.AddHours(7);
    }

}
