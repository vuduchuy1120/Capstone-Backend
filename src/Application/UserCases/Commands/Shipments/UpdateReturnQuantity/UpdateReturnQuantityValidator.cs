using Application.Abstractions.Data;
using Contract.Services.Shipment.UpdateReturnQuantity;
using FluentValidation;

namespace Application.UserCases.Commands.Shipments.UpdateReturnQuantity;

public class UpdateReturnQuantityValidator : AbstractValidator<UpdateShipmentReturnQuantityCommand>
{
    public UpdateReturnQuantityValidator(
        IShipmentRepository shipmentRepository,
        IShipmentDetailRepository shipmentDetailRepository)
    {
        RuleFor(req => req.ShipmentId)
            .NotEmpty().WithMessage("Mã giao hàng không được trống")
            .Must((req, shipmentId) =>
            {
                return shipmentId == req.ShipmentId;
            }).WithMessage("Mã giao hàng không đồng nhất")
            .MustAsync(async (shipmentId, _) =>
            {
                return await shipmentRepository.IsShipmentIdExistAsync(shipmentId);
            }).WithMessage("Mã giao hàng không tồn tại");

        RuleFor(req => req.UpdateReturnQuantityRequest)
            .Must((req, updateRequest) =>
            {
                return updateRequest.UpdateQuantityRequests.Any(updateRequest => updateRequest.Quantity < 0);
            }).WithMessage("Đang tồn tại số lượng nhỏ hơn 0")
            .MustAsync(async (updateRequest, _) =>
            {
                var shipmentDetailIds = updateRequest.UpdateQuantityRequests
                .Select(request => request.ShipDetailId)
                .ToList();

                return await shipmentDetailRepository.IsAllShipDetailIdAndShipmentIdValidAsync(
                    updateRequest.ShipmentId,
                    shipmentDetailIds);
            }).WithMessage("Đang tồn tại mã chi tiết giao hàng không tồn tại");
    }
}
