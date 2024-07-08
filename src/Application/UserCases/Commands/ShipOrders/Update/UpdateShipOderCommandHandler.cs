using Application.Abstractions.Data;
using Application.Utils;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.ShipOrder.Create;
using Contract.Services.ShipOrder.Share;
using Contract.Services.ShipOrder.Update;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using Domain.Exceptions.ShipOrder;
using FluentValidation;

namespace Application.UserCases.Commands.ShipOrders.Update;

internal sealed class UpdateShipOderCommandHandler(
    IShipOrderRepository _shipOrderRepository,
    ISetRepository _setRepository,
    IOrderDetailRepository _orderDetailRepository,
    IProductPhaseRepository _productPhaseRepository,
    IValidator<UpdateShipOrderRequest> _validator,
    IUnitOfWork _unitOfWork) : ICommandHandler<UpdateShipOderCommand>
{
    public async Task<Result.Success> Handle(UpdateShipOderCommand request, CancellationToken cancellationToken)
    {
        var updateShipOrderRequest = request.UpdateShipOrderRequest;
        var shipOrder = await GetAndValidateShipOrderRequest(request.ShipOrderId, updateShipOrderRequest)
            ?? throw new ShipOrderNotFoundException();
        var shipOrderDetails = updateShipOrderRequest.ShipOrderDetailRequests;

        if (updateShipOrderRequest.KindOfShipOrder == DeliveryMethod.SHIP_ORDER)
        {
            await ShipOrderUtil.CheckDeliveredQuantity(shipOrderDetails, updateShipOrderRequest.OrderId, _orderDetailRepository);

            var oldShipProductDetails = await GetOldProductDetails(shipOrder);
            var newShipProductDetails = await ShipOrderUtil.GetProductDetailInShipOrder(shipOrderDetails, _setRepository);

            await CheckAndUpdateAvailableQuantity(oldShipProductDetails, newShipProductDetails);
        }
        else if (updateShipOrderRequest.KindOfShipOrder == DeliveryMethod.RETURN_PRODUCT)
        {
            await ShipOrderUtil.CheckReturnQuantity(shipOrderDetails, updateShipOrderRequest.OrderId, _orderDetailRepository);
        }
        else
        {
            throw new QuantityNotValidException("Không tìm thấy loại giao hàng phù hợp");
        }

        var newShipOrderDetails = ShipOrderUtil.CreateShipOrderDetails(shipOrderDetails, shipOrder.Id)
            ?? throw new ShipOrderDetailNotFoundException();
        shipOrder.Update(request.UpdatedBy, newShipOrderDetails, updateShipOrderRequest);

        _shipOrderRepository.Update(shipOrder);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success.Update();
    }

    private async Task<ShipOrder> GetAndValidateShipOrderRequest(Guid shipOrderId, UpdateShipOrderRequest request)
    {
        if(shipOrderId != request.Id)
        {
            throw new ShipOrderIdConflictException();
        }

        var validationResult = await _validator.ValidateAsync(request);
        if(!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }

        return await _shipOrderRepository.GetByShipOrderIdAsync(shipOrderId);
    }

    

    private async Task<List<ShipProductDetail>> GetOldProductDetails (ShipOrder shipOrder)
    {
        var shipOrderDetailRequests = ShipOrderUtil.GetShipOrderRequestFromShipOrder(shipOrder);
        return await ShipOrderUtil.GetProductDetailInShipOrder(shipOrderDetailRequests, _setRepository);
    }

    private async Task CheckAndUpdateAvailableQuantity(
        List<ShipProductDetail> oldProductDetails, 
        List<ShipProductDetail> newProductDetails)
    {
        var productIds = GetAllProductId(oldProductDetails, newProductDetails);

        if (productIds is null || productIds.Count == 0)
        {
            throw new QuantityNotValidException("Không có sản phẩm được giao");
        }

        var productPhases = await _productPhaseRepository.GetProductPhaseOfMainFactoryDoneByProductIdsAsync(productIds);
        if (productPhases is null || productPhases.Count == 0)
        {
            throw new QuantityNotValidException("Không tìm thấy sản phẩm trong kho");
        }

        foreach (var product in productPhases)
        {
            var availableQuantity = product.AvailableQuantity;

            var oldProductDetail = oldProductDetails.SingleOrDefault(p => p.ProductId == product.ProductId);
            if(oldProductDetail is not null)
            {
                availableQuantity += oldProductDetail.Quantity;
            }

            var newProductDetail = newProductDetails.SingleOrDefault(p => p.ProductId == product.ProductId);
            if (newProductDetail is not null)
            {
                availableQuantity -= newProductDetail.Quantity;
            }

            if(availableQuantity < 0)
            {
                throw new QuantityNotValidException("Số lượng trong kho không đủ");
            }

            product.UpdateAvailableQuantity(availableQuantity);
        }

        _productPhaseRepository.UpdateProductPhaseRange(productPhases);
    }

    private List<Guid> GetAllProductId(List<ShipProductDetail> oldProductDetails,
        List<ShipProductDetail> newProductDetails)
    {
        var result = new HashSet<Guid>();
        foreach (var product in oldProductDetails)
        {
            var productId = product.ProductId;
            result.Add(productId);
        }
        foreach (var product in newProductDetails)
        {
            var productId = product.ProductId;
            result.Add(productId);
        }
        return result.ToList();
    }
}
