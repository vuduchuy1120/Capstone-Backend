using Contract.Abstractions.Messages;

namespace Contract.Services.EmployeeProduct.Deletes;

public record DeleteEmployeeProductCommand(DeleteEmployeeProductRequest DeleteEmployeeProductRequest) : ICommand;