using Application.Abstractions.Data;
using Contract.Services.ShipOrder.Share;
using Domain.Entities;
using Domain.Exceptions.OrderDetails;
using Domain.Exceptions.SetProducts;
using Domain.Exceptions.Sets;
using Domain.Exceptions.ShipOrder;

namespace Application.Utils;

public class ShipOrderUtil
{
    public static List<ShipOrderDetail> CreateShipOrderDetails(List<ShipOrderDetailRequest> shipOrderDetails, Guid shipOrderId)
    {
        return shipOrderDetails.Select(shipOrderDetail =>
        {
            if (shipOrderDetail.ItemKind == ItemKind.PRODUCT)
            {
                return ShipOrderDetail.CreateShipProductOrder(shipOrderDetail.ItemId, shipOrderId, shipOrderDetail.Quantity);
            }
            else if (shipOrderDetail.ItemKind == ItemKind.SET)
            {
                return ShipOrderDetail.CreateShipSetOrder(shipOrderDetail.ItemId, shipOrderId, shipOrderDetail.Quantity);
            }
            throw new QuantityNotValidException("Không tìm thấy loại sản phẩm đặt hàng phù hợp");
        }).ToList();
    }

    public async static Task<List<ShipProductDetail>> GetProductDetailInShipOrder(
        List<ShipOrderDetailRequest> shipOrderDetails, ISetRepository setRepository)
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
                var set = await setRepository.GetByIdAsync(s.ItemId) ?? throw new SetNotFoundException();

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

    public async static Task CheckReturnQuantity(
        List<ShipOrderDetailRequest> shipOrderDetails, Guid orderId, IOrderDetailRepository orderDetailRepository)
    {
        var orderDetails = await orderDetailRepository.GetOrderDetailsByOrderIdAsync(orderId);

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

    public async static Task CheckDeliveredQuantity(
        List<ShipOrderDetailRequest> shipOrderDetails, Guid orderId, IOrderDetailRepository orderDetailRepository)
    {
        var orderDetails = await orderDetailRepository.GetOrderDetailsByOrderIdAsync(orderId);
        if (orderDetails == null || orderDetails.Count == 0)
        {
            throw new OrderDetailNotFoundException();
        }

        foreach (var shipOrderDetailRequest in shipOrderDetails)
        {
            if (shipOrderDetailRequest.ItemKind == ItemKind.PRODUCT)
            {
                var isQuantityValid = orderDetails
                    .Any(order => order.ProductId == shipOrderDetailRequest.ItemId
                        && order.Quantity - order.ShippedQuantity >= shipOrderDetailRequest.Quantity);
                if (!isQuantityValid)
                    throw new QuantityNotValidException("Số lượng giao bị thừa hoặc không tìm thấy sản phẩm trong đơn hàng");
            }
            else if (shipOrderDetailRequest.ItemKind == ItemKind.SET)
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

    public static List<ShipOrderDetailRequest> GetShipOrderRequestFromShipOrder(ShipOrder shipOrder)
    {
        var shipOrderDetails = shipOrder.ShipOrderDetails
            ?? throw new ShipOrderDetailNotFoundException();

        return shipOrderDetails.Select(s =>
        {
            if (s.ProductId is not null)
            {
                return new ShipOrderDetailRequest((Guid)s.ProductId, s.Quantity, ItemKind.PRODUCT);
            }
            else if (s.SetId is not null)
            {
                return new ShipOrderDetailRequest((Guid)s.SetId, s.Quantity, ItemKind.SET);
            }
            else
            {
                throw new ShipOrderDetailDoNotHaveItemIdException();
            }
        }).ToList();
    }



}
