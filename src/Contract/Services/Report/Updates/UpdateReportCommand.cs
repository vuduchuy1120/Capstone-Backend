using Contract.Abstractions.Messages;

namespace Contract.Services.Report.Updates;

public record UpdateReportCommand(UpdateReportRequestWithClaims updateRequest, string UpdatedBy) : ICommand;

