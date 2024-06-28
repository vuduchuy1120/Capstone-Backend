using Contract.Abstractions.Messages;

namespace Contract.Services.OrderDetail.Creates;

public record CreateOrderDetailsCommand(CreateListOrderDetailsRequest CreateListOrderDetailsRequest) : ICommand;
