using Contract.Abstractions.Messages;

namespace Contract.Services.Report.Creates;

public record CreateReportCommand(CreateReportRequest CreateReportRequest, string CreatedBy) : ICommand;

