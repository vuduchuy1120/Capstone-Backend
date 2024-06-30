namespace Contract.Services.ShipOrder.Share;

public static class KindOfShipOrderExtension
{
    private static readonly Dictionary<KindOfShipOrder, string> _kindOfShipDescriptions = new()
    {
        { KindOfShipOrder.SHIP_ORDER, "Giao trả khách sản phẩm" },
        { KindOfShipOrder.RETURN_PRODUCT, "Khách hàng trả hàng lỗi" }
    };

    public static string GetDescription(this KindOfShipOrder status)
    {
        return _kindOfShipDescriptions[status];
    }
}
