using Application.Abstractions.Data;
using Application.Utils;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Order.Creates;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;

namespace Application.UserCases.Commands.Orders.Creates;

public sealed class CreateOrderCommandHandler
    (IOrderRepository _orderRepository,
    IOrderDetailRepository _orderDetailRepository,
    IValidator<CreateOrderRequest> _validator,
    IUnitOfWork _unitOfWork
    ) : ICommandHandler<CreateOrderCommand>
{
    public async Task<Result.Success> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request.CreateOrderRequest);
        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }
        var startDate = DateUtil.ConvertStringToDateTimeOnly(request.CreateOrderRequest.StartOrder);
        var endDate = DateUtil.ConvertStringToDateTimeOnly(request.CreateOrderRequest.EndOrder);
        if (startDate > endDate)
        {
            throw new MyValidationException("Ngày kết thúc đơn hàng phải lớn hơn ngày bắt đầu đơn hàng.");
        }

        var order = Order.Create(request.CreateOrderRequest, request.CreatedBy);
        _orderRepository.AddOrder(order);

        var orderDetails = request.CreateOrderRequest.OrderDetailRequests
            .Select(orderDetailRequest => OrderDetail.Create(order.Id, 0, orderDetailRequest))
            .ToList();
        _orderDetailRepository.AddRange(orderDetails);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success.Create();
    }
}
