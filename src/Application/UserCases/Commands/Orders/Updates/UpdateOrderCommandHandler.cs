using Application.Abstractions.Data;
using Application.Utils;
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
        var startDate = DateUtil.ConvertStringToDateTimeOnly(request.UpdateOrderRequest.StartOrder);
        var endDate = DateUtil.ConvertStringToDateTimeOnly(request.UpdateOrderRequest.EndOrder);
        if (startDate > endDate)
        {
            throw new MyValidationException("Ngày kết thúc đơn hàng phải lớn hơn ngày bắt đầu đơn hàng.");
        }

        var order = await _orderRepository.GetOrderByIdAsync(request.UpdateOrderRequest.OrderId);
        if(order == null)
        {
               throw new MyValidationException("Order does not exist.");
        }

        order.Update(request.UpdateOrderRequest,request.UpdatedBy);
        _orderRepository.UpdateOrder(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success.Update();
    }
}
