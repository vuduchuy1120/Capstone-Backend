using Contract.Services.Report.Creates;
using Contract.Services.Report.ShareDtos;
using Contract.Services.Report.Updates;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class Report : EntityAuditBase<Guid>
{
    public string Description { get; private set; }
    public StatusReport Status { get; private set; }
    public ReportType ReportType { get; private set; }
    public string? ReplyMessage { get; private set; }
    public User User { get; private set; }
    public string UserId { get; private set; }

    public static Report Create(CreateReportRequest request, string CreatedBy)
    {
        return new Report
        {
            Id = Guid.NewGuid(),
            Description = request.Description,
            Status = 0,
            ReportType = request.ReportType,
            UserId = CreatedBy,
            CreatedDate = DateTime.UtcNow.AddHours(7),
            CreatedBy = CreatedBy
        };

    }
    public void Update(UpdateReportRequest request, string updatedBy)
    {
        Status = request.Status;
        ReplyMessage = request.ReplyMessage;
        UpdatedDate = DateTime.UtcNow.AddHours(7);
        UpdatedBy = updatedBy;
    }

}
