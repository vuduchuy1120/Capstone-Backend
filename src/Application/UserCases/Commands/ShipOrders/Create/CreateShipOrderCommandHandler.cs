using Application.Abstractions.Data;
using Application.Utils;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.ShipOrder.Create;
using Contract.Services.ShipOrder.Share;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using Domain.Exceptions.SetProducts;
using Domain.Exceptions.Sets;
using Domain.Exceptions.ShipOrder;
using FluentValidation;

namespace Application.UserCases.Commands.ShipOrders.Create;

internal class CreateShipOrderCommandHandler(
    IShipOrderRepository _shipOrderRepository, 
    IOrderDetailRepository _orderDetailRepository,
    IProductPhaseRepository _productPhaseRepository,
    IShipOrderDetailRepository _shipOrderDetailRepository,
    ISetRepository _setRepository,
    IUnitOfWork _unitOfWork,
    IValidator<CreateShipOrderRequest> _validator)
    : ICommandHandler<CreateShipOrderCommand>
{
    public async Task<Result.Success> Handle(CreateShipOrderCommand request, CancellationToken cancellationToken)
    {
        var createShipOrderRequest = request.CreateShipOrderRequest;
        await ValidateRequest(createShipOrderRequest);

        var shipOrderDetails = createShipOrderRequest.ShipOrderDetailRequests;

        var productDetails = await ShipOrderUtil.GetProductDetailInShipOrder(shipOrderDetails, _setRepository);

        if (createShipOrderRequest.KindOfShipOrder == DeliveryMethod.SHIP_ORDER)
        {
            await ShipOrderUtil.CheckDeliveredQuantity(shipOrderDetails, createShipOrderRequest.OrderId, _orderDetailRepository);
            await CheckAndUpdateAvailableQuantityInStock(productDetails);
        }
        else if(createShipOrderRequest.KindOfShipOrder == DeliveryMethod.RETURN_PRODUCT)
        {
            await ShipOrderUtil.CheckReturnQuantity(shipOrderDetails, createShipOrderRequest.OrderId, _orderDetailRepository);
            //await UpdateAvailableQuantityInStock(productDetails);
        }
        else
        {
            throw new QuantityNotValidException("Không tìm thấy loại giao hàng phù hợp");
        }

        var shipOrder = ShipOrder.Create(request.createdBy, createShipOrderRequest);

        var shipOderDetails = ShipOrderUtil.CreateShipOrderDetails(shipOrderDetails, shipOrder.Id);

        _shipOrderRepository.Add(shipOrder);
        _shipOrderDetailRepository.AddRange(shipOderDetails);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success.Create();
    }

    private async Task ValidateRequest(CreateShipOrderRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }
    }

    private async Task CheckAndUpdateAvailableQuantityInStock(List<ShipProductDetail> shipProductDetails)
    {
        var productIds = shipProductDetails.Select(shipProductDetails => shipProductDetails.ProductId).ToList();
        if(productIds is null || productIds.Count == 0)
        {
            throw new QuantityNotValidException("Không có sản phẩm được giao");
        }

        var productPhases = await _productPhaseRepository.GetProductPhaseOfMainFactoryDoneByProductIdsAsync(productIds);
        if (productPhases is null || productPhases.Count == 0)
        {
            throw new QuantityNotValidException("Không tìm thấy sản phẩm trong kho");
        }

        foreach(var product in productPhases)
        {
            var shipDetail = shipProductDetails.SingleOrDefault(s => s.ProductId == product.ProductId);
            if(shipDetail is null)
            {
                throw new QuantityNotValidException("Không tìm thấy sản phẩm trong kho");
            }

            if(shipDetail.Quantity > product.AvailableQuantity)
            {
                throw new QuantityNotValidException("Số lượng trong kho không đủ hoặc không tìm thấy sản phẩm trong kho");
            }

            product.UpdateAvailableQuantity(product.AvailableQuantity - shipDetail.Quantity);
        }

        _productPhaseRepository.UpdateProductPhaseRange(productPhases);
    }

    private async Task UpdateAvailableQuantityInStock(List<ShipProductDetail> shipProductDetails)
    {
        var productIds = shipProductDetails.Select(shipProductDetails => shipProductDetails.ProductId).ToList();
        if (productIds is null || productIds.Count == 0)
        {
            throw new QuantityNotValidException("Không tìm thấy sản phẩm trong đơn giao");
        }

        var productPhases = await _productPhaseRepository.GetProductPhaseOfMainFactoryDoneByProductIdsAsync(productIds);
        if (productPhases is null || productPhases.Count == 0)
        {
            throw new QuantityNotValidException("Không tìm thấy sản phẩm trong kho");
        }

        foreach (var shipDetail in shipProductDetails)
        {
            var product = productPhases.FirstOrDefault(p => p.ProductId == shipDetail.ProductId);
            if(product is null)
            {
                throw new QuantityNotValidException("Không tìm thấy sản phẩm trong kho");
            }

            product.UpdateAvailableQuantity(product.ErrorAvailableQuantity + shipDetail.Quantity);
        }

        _productPhaseRepository.UpdateProductPhaseRange(productPhases);
    }
}
