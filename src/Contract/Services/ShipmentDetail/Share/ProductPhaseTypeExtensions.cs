namespace Contract.Services.ShipmentDetail.Share;

public static class ProductPhaseTypeExtensions
{
    private static readonly Dictionary<ProductPhaseType, string> _phaseDescriptions = new Dictionary<ProductPhaseType, string>
    {
        { ProductPhaseType.NO_PROBLEM, "Sản phẩm không lỗi" },
        { ProductPhaseType.FACTORY_ERROR, "Sản phẩm lỗi do bên xưởng" },
        { ProductPhaseType.THIRD_PARTY_ERROR, "Sản phẩm bên thứ 3 làm lỗi" },
        { ProductPhaseType.THIRD_PARTY_NO_FIX_ERROR, "Sản phẩm bên thứ 3 làm hỏng trong khi gia công" }
    };

    public static string GetDescription(this ProductPhaseType phase)
    {
        return _phaseDescriptions[phase];
    }
}