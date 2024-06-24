using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.OrderDetail.Creates;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using FluentValidation;

namespace Application.UserCases.Commands.OrderDetails.Creates;

public sealed class CreateListOrderDetailsCommandHandler
    (IOrderDetailRepository _orderDetailRepository,
    IValidator<CreateListOrderDetailsRequest> _validator,
    IUnitOfWork _unitOfWork
    ) : ICommandHandler<CreateOrderDetailsCommand>

{
    public async Task<Result.Success> Handle(CreateOrderDetailsCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request.CreateListOrderDetailsRequest);
        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }
        var orderDetailsDelete = await _orderDetailRepository.GetOrderDetailsByOrderIdAsync(request.CreateListOrderDetailsRequest.OrderId);
        if (orderDetailsDelete != null)
        {
            _orderDetailRepository.DeleteRange(orderDetailsDelete);
        }

        var orderDetails = request.CreateListOrderDetailsRequest.OrderDetailRequests
            .Select(orderDetailRequest => OrderDetail.Create(request.CreateListOrderDetailsRequest.OrderId, orderDetailRequest))
            .ToList();
        _orderDetailRepository.AddRange(orderDetails);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success.Create();
    }
}
