using Contract.Services.Report.Creates;
using Contract.Services.Report.Updates;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class Report : EntityAuditBase<Guid>
{
    public string Description { get; private set; }
    public string Status { get; private set; }
    public string ReportType { get; private set; }
    public string? ReplyMessage { get; private set; }
    public User User { get; private set; }
    public string UserId { get; private set; }

    public static Report Create(CreateReportRequest request, string CreatedBy)
    {
        return new Report
        {
            Id = Guid.NewGuid(),
            Description = request.Description,
            Status = "Pending",
            ReportType = request.ReportType,
            UserId = CreatedBy,
            CreatedDate = DateTime.UtcNow.AddHours(7),
            CreatedBy = CreatedBy
        };

    }
    public void Update(UpdateReportRequest request, string UpdatedBy)
    {
        Status = request.Status;
        ReplyMessage = request.ReplyMessage;
        UpdatedDate = DateTime.UtcNow.AddHours(7);
        UpdatedBy = UpdatedBy;
    }
}
