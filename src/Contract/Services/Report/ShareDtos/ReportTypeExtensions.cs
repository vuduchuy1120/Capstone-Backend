using Contract.Services.Order.ShareDtos;

namespace Contract.Services.Report.ShareDtos;

public static class ReportTypeExtensions
{
    private static readonly Dictionary<ReportType, string> _ReportTypeDescriptions = new Dictionary<ReportType, string>
    {
        {ReportType.DiemDanh, "Báo cáo về điểm danh"},
        {ReportType.ChamCong, "Báo cáo về chấm công"},
        {ReportType.Luong, "Báo cáo về lương"},
        {ReportType.DonKhac, "Các loại đơn khác"}

    };

    public static string GetDescription(this ReportType type)
    {
        return _ReportTypeDescriptions[type];
    }
}
