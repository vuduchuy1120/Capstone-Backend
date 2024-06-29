namespace Contract.Services.Order.ShareDtos;

public static class StatusExtensions
{
    private static readonly Dictionary<StatusOrder, string> _statusDescriptions = new Dictionary<StatusOrder, string>
    {
        {StatusOrder.SIGNED , "Đã nhận đơn hàng" },
        {StatusOrder.INPROGRESS , "Đang thực hiện" },
        {StatusOrder.COMPLETED , "Đã hoàn thành" },
        {StatusOrder.CANCELLED , "Đã hủy" }
    };

    public static string GetDescription(this StatusOrder type)
    {
        return _statusDescriptions[type];
    }
}
