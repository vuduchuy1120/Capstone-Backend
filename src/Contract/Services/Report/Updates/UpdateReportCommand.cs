using Contract.Abstractions.Messages;

namespace Contract.Services.Report.Updates;

public record UpdateReportCommand(UpdateReportRequest updateRequest, string UpdatedBy) : ICommand;

