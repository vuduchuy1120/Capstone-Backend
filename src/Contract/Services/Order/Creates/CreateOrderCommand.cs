using Contract.Abstractions.Messages;

namespace Contract.Services.Order.Creates;

public record CreateOrderCommand(CreateOrderRequest CreateOrderRequest, string CreatedBy) : ICommand;
