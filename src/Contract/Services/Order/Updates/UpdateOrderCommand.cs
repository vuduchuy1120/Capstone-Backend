using Contract.Abstractions.Messages;

namespace Contract.Services.Order.Updates;

public record UpdateOrderCommand(UpdateOrderRequest UpdateOrderRequest, string UpdatedBy) : ICommand;

