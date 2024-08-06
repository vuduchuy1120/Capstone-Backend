using Application.Abstractions.Data;
using Application.Utils;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Shipment.Share;
using Contract.Services.ShipOrder.AcceptShipOrder;
using Contract.Services.ShipOrder.Share;
using Domain.Entities;
using Domain.Exceptions.OrderDetails;
using Domain.Exceptions.ShipOrder;

namespace Application.UserCases.Commands.ShipOrders.AcceptShipOrder;

internal sealed class AcceptShipOrderCommandHandler(
    IShipOrderRepository _shipOrderRepository,
    IOrderDetailRepository _orderDetailRepository,
    IProductPhaseRepository _productPhaseRepository,
    ISetRepository _setRepository,
    IUnitOfWork _unitOfWork) : ICommandHandler<AcceptShipOrderCommand>
{
    public async Task<Result.Success> Handle(AcceptShipOrderCommand request, CancellationToken cancellationToken)
    {
        var shipOrder = await _shipOrderRepository.GetByIdAndStatusIsNotDoneAsync(request.shipOrderId)
            ?? throw new ShipOrderNotFoundException($"Không tìm thấy đơn giao chưa được xác nhận có id: {request.shipOrderId}");

        var shipOrderDetails = shipOrder.ShipOrderDetails;

        var shipOrderDetailRequests = ShipOrderUtil.GetShipOrderRequestFromShipOrder(shipOrder);
        var shipProductDetails = await ShipOrderUtil.GetProductDetailInShipOrder(shipOrderDetailRequests, _setRepository);
        var status = shipOrder.Status;


        if (status != Status.SHIPPED && status != Status.CANCEL)
        {
            throw new ShipOrderBadRequestException("Đơn giao chỉ được chốt khi trạng thái là giao thành công hoặc hủy!");
        }

        await UpdateQuantityInProductPhase(shipProductDetails, shipOrder.DeliveryMethod, status);

        if (status == Status.SHIPPED)
        {
            // update quantity in orderDetail
            await UpdateQuantityShippedInShipOrderDetails(shipOrder.OrderId, shipOrderDetails, shipOrder.DeliveryMethod);
        }

        shipOrder.UpdateAccepted(request.updatedBy);

        _shipOrderRepository.Update(shipOrder);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success.Update();
    }


    private async Task UpdateQuantityShippedInShipOrderDetails(Guid orderId, List<ShipOrderDetail>? shipOrderDetails, DeliveryMethod deliveryMethod)
    {
        var orderDetails = await _orderDetailRepository.GetOrderDetailsByOrderIdWithoutInclueAsync(orderId)
            ?? throw new OrderDetailNotFoundException();

        foreach (var shipOderDetail in shipOrderDetails)
        {
            OrderDetail orderDetail = null;
            if (shipOderDetail.ProductId is not null && shipOderDetail.SetId is null)
            {
                orderDetail = orderDetails.FirstOrDefault(od => od.ProductId == shipOderDetail.ProductId)
                   ?? throw new ShipOrderNotFoundException($"Không tìm thấy sản phẩm có id: {shipOderDetail.ProductId} trong chi tiết đơn hàng");
            }
            else if (shipOderDetail.SetId is not null && shipOderDetail.ProductId is null)
            {
                orderDetail = orderDetails.FirstOrDefault(od => od.SetId == shipOderDetail.SetId)
                   ?? throw new ShipOrderNotFoundException($"Không tìm thấy bộ sản phẩm có id: {shipOderDetail.SetId} trong chi tiết đơn hàng");
            }
            else
            {
                throw new ShipOrderBadRequestException("Chi tiết đơn hàng đang có cả bộ và sản phẩm");
            }

            var newShippedQuantity = GetNewShippedQuantity(deliveryMethod, orderDetail.ShippedQuantity, shipOderDetail.Quantity);

            orderDetail.UpdateShippedQuantity(newShippedQuantity);
        }

        _orderDetailRepository.UpdateRange(orderDetails);
    }

    private int GetNewShippedQuantity(DeliveryMethod deliveryMethod, int shippedQuantity, int requestQuantity)
    {
        if (deliveryMethod == DeliveryMethod.SHIP_ORDER)
        {
            return shippedQuantity + requestQuantity;
        }
        else if (deliveryMethod == DeliveryMethod.RETURN_PRODUCT)
        {
            if (shippedQuantity < requestQuantity)
            {
                throw new QuantityNotValidException("Số lượng trả hàng lớn hơn số lượng đã giao");
            }

            return shippedQuantity - requestQuantity;
        }
        else
        {
            throw new ShipOrderBadRequestException("Không tìm thấy loại giao hàng phù hợp");
        }
    }

    private async Task UpdateQuantityInProductPhase(
        List<ShipProductDetail> shipProductDetails,
        DeliveryMethod deliveryMethod,
        Status status)
    {
        var productIds = shipProductDetails.Select(x => x.ProductId).ToList();
        if (productIds is null || productIds.Count == 0)
        {
            throw new QuantityNotValidException("Không có sản phẩm được giao");
        }

        var productPhases = await _productPhaseRepository.GetProductPhaseOfMainFactoryDoneByProductIdsAsync(productIds);
        if (productPhases is null || productPhases.Count == 0)
        {
            throw new QuantityNotValidException("Không tìm thấy sản phẩm trong kho");
        }

        if (deliveryMethod == DeliveryMethod.SHIP_ORDER && status == Status.SHIPPED)
        {
            UpdateQuantity(productPhases, shipProductDetails);
        }
        else if (deliveryMethod == DeliveryMethod.SHIP_ORDER && status == Status.CANCEL)
        {
            UpdateAvailableQuantity(productPhases, shipProductDetails);
        }
        else if (deliveryMethod == DeliveryMethod.RETURN_PRODUCT && status == Status.SHIPPED)
        {
            UpdateErrorQuantity(productPhases, shipProductDetails);
        }
        else
        {
            throw new QuantityNotValidException("Không tìm thấy loại giao hàng phù hợp");
        }
    }

    private void UpdateQuantity(List<ProductPhase> productPhases, List<ShipProductDetail> shipProductDetails)
    {
        foreach (var product in productPhases)
        {
            var productDetailRequest = shipProductDetails.SingleOrDefault(s => s.ProductId == product.ProductId)
                ?? throw new ShipOrderDetailNotFoundException();

            var quantity = product.Quantity - productDetailRequest.Quantity;
            if (quantity < 0)
            {
                throw new QuantityNotValidException("Số lượng trong kho không đủ");
            }

            product.UpdateQuantity(quantity);
        }

        _productPhaseRepository.UpdateProductPhaseRange(productPhases);
    }

    private void UpdateAvailableQuantity(List<ProductPhase> productPhases, List<ShipProductDetail> shipProductDetails)
    {
        foreach (var product in productPhases)
        {
            var productDetailRequest = shipProductDetails.SingleOrDefault(s => s.ProductId == product.ProductId)
                ?? throw new ShipOrderDetailNotFoundException();

            var availableQuantity = product.AvailableQuantity + productDetailRequest.Quantity;

            product.UpdateAvailableQuantity(availableQuantity);
        }

        _productPhaseRepository.UpdateProductPhaseRange(productPhases);
    }

    private void UpdateErrorQuantity(List<ProductPhase> productPhases, List<ShipProductDetail> shipProductDetails)
    {
        foreach (var product in productPhases)
        {
            var productDetailRequest = shipProductDetails.SingleOrDefault(s => s.ProductId == product.ProductId)
                ?? throw new ShipOrderDetailNotFoundException();

            product.UpdateErrorQuantity(product.ErrorQuantity + productDetailRequest.Quantity);
            product.UpdateErrorAvailableQuantity(product.ErrorAvailableQuantity + productDetailRequest.Quantity);
        }

        _productPhaseRepository.UpdateProductPhaseRange(productPhases);
    }

}
