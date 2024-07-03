using Application.Abstractions.Data;
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

        var productDetails = await GetProductDetailInShipOrder(shipOrderDetails);

        if (createShipOrderRequest.KindOfShipOrder == DeliveryMethod.SHIP_ORDER)
        {
            await CheckDeliveredQuantity(shipOrderDetails, createShipOrderRequest.OrderId);
            await CheckAndUpdateAvailableQuantityInStock(productDetails);
        }
        else if(createShipOrderRequest.KindOfShipOrder == DeliveryMethod.RETURN_PRODUCT)
        {
            await CheckReturnQuantity(shipOrderDetails, createShipOrderRequest.OrderId);
            await UpdateAvailableQuantityInStock(productDetails);
        }
        else
        {
            throw new QuantityNotValidException("Không tìm thấy loại giao hàng phù hợp");
        }

        var shipOrder = ShipOrder.Create(request.createdBy, createShipOrderRequest);

        var shipOderDetails = CreateShipOrderDetails(shipOrderDetails, shipOrder.Id);

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

    private async Task CheckDeliveredQuantity(List<ShipOrderDetailRequest> shipOrderDetails, Guid orderId)
    {
        var orderDetails = await _orderDetailRepository.GetOrderDetailsByOrderIdAsync(orderId);

        foreach(var shipOrderDetailRequest in shipOrderDetails)
        {
            if(shipOrderDetailRequest.ItemKind == ItemKind.PRODUCT)
            {
                var isQuantityValid = orderDetails
                    .Any(order => order.ProductId == shipOrderDetailRequest.ItemId 
                        && order.Quantity - order.ShippedQuantity >= shipOrderDetailRequest.Quantity);
                if (!isQuantityValid) 
                    throw new QuantityNotValidException("Số lượng giao bị thừa hoặc không tìm thấy sản phẩm trong đơn hàng");
            }
            else if(shipOrderDetailRequest.ItemKind == ItemKind.SET)
            {
                var isQuantityValid = orderDetails
                    .Any(order => order.SetId == shipOrderDetailRequest.ItemId
                        && order.Quantity - order.ShippedQuantity >= shipOrderDetailRequest.Quantity);
                if (!isQuantityValid)
                    throw new QuantityNotValidException("Số lượng giao bị thừa hoặc không tìm thấy bộ trong đơn hàng");
            }
            else
            {
                throw new QuantityNotValidException("Không tìm thấy loại sản phẩm đặt hàng phù hợp");
            }
        }
    }

    private async Task CheckReturnQuantity(List<ShipOrderDetailRequest> shipOrderDetails, Guid orderId)
    {
        var orderDetails = await _orderDetailRepository.GetOrderDetailsByOrderIdAsync(orderId);

        foreach (var shipOrderDetailRequest in shipOrderDetails)
        {
            if (shipOrderDetailRequest.ItemKind == ItemKind.PRODUCT)
            {
                var isQuantityValid = orderDetails
                    .Any(order => order.ProductId == shipOrderDetailRequest.ItemId
                        && order.ShippedQuantity >= shipOrderDetailRequest.Quantity);
                if (!isQuantityValid)
                    throw new QuantityNotValidException("Số lượng sản phẩm trả quá số lượng đã giao hoặc không tìm thấy sản phầm trong đơn hàng");
            }
            else if (shipOrderDetailRequest.ItemKind == ItemKind.SET)
            {
                var isQuantityValid = orderDetails
                    .Any(order => order.SetId == shipOrderDetailRequest.ItemId
                        && order.ShippedQuantity >= shipOrderDetailRequest.Quantity);
                if (!isQuantityValid)
                    throw new QuantityNotValidException("Số lượng bộ trả quá số lượng đã giao hoặc không tìm thấy bộ trong đơn hàng");
            }
            else
            {
                throw new QuantityNotValidException("Không tìm thấy loại sản phẩm đặt hàng phù hợp");
            }
        }
    }

    public async Task<List<ShipProductDetail>> GetProductDetailInShipOrder(List<ShipOrderDetailRequest> shipOrderDetails)
    {
        var products = shipOrderDetails
            .Where(s => s.ItemKind == ItemKind.PRODUCT)
            .Select(s => new ShipProductDetail
            {
                ProductId = s.ItemId,
                Quantity = s.Quantity,
            })
        .ToList();

        var productsInSetTasks = shipOrderDetails
            .Where(s => s.ItemKind == ItemKind.SET)
            .Select(async s =>
            {
                var set = await _setRepository.GetByIdAsync(s.ItemId) ?? throw new SetNotFoundException();

                if (set.SetProducts == null || set.SetProducts.Count == 0)
                {
                    throw new SetProductNotFoundException();
                }

                return set.SetProducts.Select(sp => new ShipProductDetail
                {
                    ProductId = sp.ProductId,
                    Quantity = s.Quantity * sp.Quantity,
                });
            })
            .ToList();

        var productsInSet = (await Task.WhenAll(productsInSetTasks)).SelectMany(x => x).ToList();

        var mergedProducts = products
            .Concat(productsInSet)
            .GroupBy(p => p.ProductId)
            .Select(g => new ShipProductDetail
            {
                ProductId = g.Key,
                Quantity = g.Sum(p => p.Quantity)
            })
            .ToList();

        return mergedProducts;
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

    private List<ShipOrderDetail> CreateShipOrderDetails(List<ShipOrderDetailRequest> shipOrderDetails, Guid shipOrderId)
    {
        return shipOrderDetails.Select(shipOrderDetail =>
        {
            if(shipOrderDetail.ItemKind == ItemKind.PRODUCT)
            {
                return ShipOrderDetail.CreateShipProductOrder(shipOrderDetail.ItemId, shipOrderId, shipOrderDetail.Quantity);
            }
            else if(shipOrderDetail.ItemKind == ItemKind.SET)
            {
                return ShipOrderDetail.CreateShipSetOrder(shipOrderDetail.ItemId, shipOrderId, shipOrderDetail.Quantity);
            }
            throw new QuantityNotValidException("Không tìm thấy loại sản phẩm đặt hàng phù hợp");
        }).ToList();
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

            product.UpdateAvailableQuantity(product.AvailableQuantity + shipDetail.Quantity);
        }

        _productPhaseRepository.UpdateProductPhaseRange(productPhases);
    }
}
