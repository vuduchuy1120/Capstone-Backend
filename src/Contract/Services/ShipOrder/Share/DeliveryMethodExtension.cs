namespace Contract.Services.ShipOrder.Share;

public static class DeliveryMethodExtension
{
    private static readonly Dictionary<DeliveryMethod, string> _kindOfShipDescriptions = new()
    {
        { DeliveryMethod.SHIP_ORDER, "Giao trả khách sản phẩm" },
        { DeliveryMethod.RETURN_PRODUCT, "Khách hàng trả hàng lỗi" }
    };

    public static string GetDescription(this DeliveryMethod status)
    {
        return _kindOfShipDescriptions[status];
    }
}
