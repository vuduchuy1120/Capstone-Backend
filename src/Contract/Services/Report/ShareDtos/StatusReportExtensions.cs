using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Services.Report.ShareDtos;

public static class StatusReportExtensions
{
    private static readonly Dictionary<StatusReport, string> _StatusReportDescriptions = new Dictionary<StatusReport, string>
    {
        {StatusReport.Pending, "Đang chờ xử lý"},
        {StatusReport.Accepted, "Đã xử lý"},
        {StatusReport.Rejected, "Đã từ chối"},

    };

    public static string GetDescription(this StatusReport type)
    {
        return _StatusReportDescriptions[type];
    }
}
