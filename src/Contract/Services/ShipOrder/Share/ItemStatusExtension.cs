namespace Contract.Services.ShipOrder.Share;

public static class ItemStatusExtension
{
    private static readonly Dictionary<ItemKind, string> _itemStatusDescriptions = new()
    {
        { ItemKind.PRODUCT, "sản phẩm đơn" },
        { ItemKind.SET, "sản phẩm bộ" }
    };

    public static string GetDescription(this ItemKind status)
    {
        return _itemStatusDescriptions[status];
    }
}
