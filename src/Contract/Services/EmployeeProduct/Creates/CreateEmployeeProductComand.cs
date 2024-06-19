using Contract.Abstractions.Messages;

namespace Contract.Services.EmployeeProduct.Creates;

public record CreateEmployeeProductComand(CreateEmployeeProductRequest createEmployeeProductRequest, string createdBy) : ICommand;
