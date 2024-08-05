using Application.Abstractions.Data;
using Application.Utils;
using AutoMapper.Configuration.Conventions;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Shipment.Share;
using Contract.Services.ShipmentDetail.Share;
using Contract.Services.ShipOrder.ChangeStatus;
using Contract.Services.ShipOrder.Share;
using Domain.Entities;
using Domain.Exceptions.OrderDetails;
using Domain.Exceptions.ShipOrder;

namespace Application.UserCases.Commands.ShipOrders.ChangeStatus;

internal sealed class ChangeShipOrderStatusCommandHandler(
    IShipOrderRepository _shipOrderRepository,
    IUnitOfWork _unitOfWork) : ICommandHandler<ChangeShipOrderStatusCommand>
{
    public async Task<Result.Success> Handle(ChangeShipOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var changeStatusRequest = request.ChangeShipOrderStatusRequest;
        if(request.Id != changeStatusRequest.ShipOrderId)
        {
            throw new ShipOrderIdConflictException();
        }

        var shipOrder = await _shipOrderRepository.GetByIdAndStatusIsNotDoneAsync(changeStatusRequest.ShipOrderId)
            ?? throw new ShipOrderNotFoundException($"Không tìm thấy đơn giao chưa được xác nhận có id: {request.Id}");

        shipOrder.UpdateStatus(changeStatusRequest.Status, request.UpdatedBy);

        _shipOrderRepository.Update(shipOrder);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success.Update();
    }

}
