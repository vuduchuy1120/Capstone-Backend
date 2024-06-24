namespace Contract.Services.Shipment.Share;

public static class StatusExtensions
{
    private static readonly Dictionary<Status, string> _statusDescriptions = new Dictionary<Status, string>
    {
        { Status.WAIT_FOR_SHIP, "Đang đợi giao" },
        { Status.SHIPPING, "Đang được giao" },
        { Status.SHIPPED, "Đã giao thành công" },
        { Status.CANCEL, "Đã hủy" },
    };

    public static string GetDescription(this Status status)
    {
        return _statusDescriptions[status];
    }
}
