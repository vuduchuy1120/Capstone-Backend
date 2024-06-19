using Contract.Abstractions.Messages;

namespace Contract.Services.EmployeeProduct.Updates;

public record UpdateEmployeeProductCommand(UpdateEmployeeProductRequest UpdateEmployeeProductRequest, string UpdatedBy) : ICommand;