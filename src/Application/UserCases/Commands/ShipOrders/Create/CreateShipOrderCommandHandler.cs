using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Shipment.Create;
using Contract.Services.ShipOrder.Create;
using Contract.Services.ShipOrder.Share;
using Domain.Abstractions.Exceptions;
using Domain.Exceptions.SetProducts;
using Domain.Exceptions.Sets;
using FluentValidation;

namespace Application.UserCases.Commands.ShipOrders.Create;

internal class CreateShipOrderCommandHandler(
    IShipOrderRepository _shipOrderRepository, 
    IOrderDetailRepository _orderDetailRepository,
    ISetRepository _setRepository,
    IValidator<CreateShipOrderRequest> _validator)
    : ICommandHandler<CreateShipOrderCommand>
{
    public async Task<Result.Success> Handle(CreateShipOrderCommand request, CancellationToken cancellationToken)
    {
        var createShipOrderRequest = request.CreateShipOrderRequest;
        await ValidateRequest(createShipOrderRequest);

        var shipOrderDetails = createShipOrderRequest.ShipOrderDetailRequests;

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


        throw new NotImplementedException();
    }

    private async Task ValidateRequest(CreateShipOrderRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }
    }
}
