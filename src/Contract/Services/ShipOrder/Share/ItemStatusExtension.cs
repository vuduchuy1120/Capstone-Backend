namespace Contract.Services.ShipOrder.Share;

public static class ItemStatusExtension
{
    private static readonly Dictionary<ItemStatus, string> _itemStatusDescriptions = new()
    {
        { ItemStatus.NO_PROBLEM, "Vật phẩm không lỗi" },
        { ItemStatus.ERROR, "Vật phẩm lỗi" }
    };

    public static string GetDescription(this ItemStatus status)
    {
        return _itemStatusDescriptions[status];
    }
}
