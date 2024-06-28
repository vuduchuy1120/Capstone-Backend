namespace Contract.Services.Company.Shared;

public static class CompanyTypeExtensions
{
    private static readonly Dictionary<CompanyType, string> _companyTypeDescriptions = new Dictionary<CompanyType, string>
    {
        { CompanyType.FACTORY, "Nhà xưởng" },
        { CompanyType.CUSTOMER_COMPANY, "Công ty mua đặt hàng" },
        { CompanyType.THIRD_PARTY_COMPANY, "Công ty hợp tác sản xuất" },
    };

    public static string GetDescription(this CompanyType type)
    {
        return _companyTypeDescriptions[type];
    }
}
