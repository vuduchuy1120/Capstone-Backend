namespace Contract.Services.SalaryHistory.ShareDtos;

public static class SalaryTypeExtensions
{
    private static readonly Dictionary<SalaryType, string> _salaryTypeDescriptions = new Dictionary<SalaryType, string>
    {
        { SalaryType.SALARY_BY_DAY, "Lương theo ngày" },
        { SalaryType.SALARY_OVER_TIME, "Lương tăng ca" },
    };
    public static string GetDescription(this SalaryType type)
    {
        return _salaryTypeDescriptions[type];
    }

}
