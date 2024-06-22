using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Order.Updates;
using Domain.Abstractions.Exceptions;
using FluentValidation;

namespace Application.UserCases.Commands.Orders.Updates;

public sealed class UpdateOrderCommandHandler
    (IOrderRepository _orderRepository,
    IValidator<UpdateOrderRequest> _validator,
    IUnitOfWork _unitOfWork
    ) : ICommandHandler<UpdateOrderCommand>
{
    public async Task<Result.Success> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request.UpdateOrderRequest);

        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }

        var order = await _orderRepository.GetOrderByIdAsync(request.UpdateOrderRequest.OrderId);
        if(order == null)
        {
               throw new MyValidationException("Order does not exist.");
        }

        order.Update(request.UpdateOrderRequest);
        _orderRepository.UpdateOrder(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success.Update();
    }
}
