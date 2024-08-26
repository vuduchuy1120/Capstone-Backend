using Application.Abstractions.Data;
using Application.UserCases.Commands.ShipOrders.Create;
using Contract.Services.Order.Creates;
using Contract.Services.Order.ShareDtos;
using Contract.Services.OrderDetail.Creates;
using Contract.Services.ProductPhase.Creates;
using Contract.Services.Set.CreateSet;
using Contract.Services.Set.SharedDto;
using Contract.Services.ShipOrder.Create;
using Contract.Services.ShipOrder.Share;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using Domain.Exceptions.OrderDetails;
using Domain.Exceptions.Sets;
using Domain.Exceptions.ShipOrder;
using FluentValidation;
using Moq;

namespace Application.UnitTests.ShipOrders.Command;

public class CreateShipOrderCommandHandlerTest
{
    private readonly Mock<IShipOrderRepository> _IShipOrderRepositoryMock;
    private readonly Mock<IOrderDetailRepository> _IOrderDetailRepositoryMock;
    private readonly Mock<IProductPhaseRepository> _IProductPhaseRepositoryMock;
    private readonly Mock<IShipOrderDetailRepository> _IShipOrderDetailRepositoryMock;
    private readonly Mock<IOrderRepository> _IOrderRepositoryMock;
    private readonly Mock<IUserRepository> _IUserRepositoryMock;
    private readonly Mock<ISetRepository> _ISetRepositoryMock;
    private readonly Mock<IUnitOfWork> _UnitOfWorkMock;
    private readonly IValidator<CreateShipOrderRequest> _Validator;
    private readonly CreateShipOrderCommandHandler _handler;
    public CreateShipOrderCommandHandlerTest()
    {
        _IShipOrderRepositoryMock = new();
        _IOrderDetailRepositoryMock = new();
        _IProductPhaseRepositoryMock = new();
        _IShipOrderDetailRepositoryMock = new();
        _IOrderRepositoryMock = new();
        _IUserRepositoryMock = new();
        _ISetRepositoryMock = new();
        _UnitOfWorkMock = new();
        _Validator = new CreateShipOrderValidator(
            _IUserRepositoryMock.Object,
            _IOrderRepositoryMock.Object,
            _IOrderDetailRepositoryMock.Object);
        _handler = new CreateShipOrderCommandHandler(
            _IShipOrderRepositoryMock.Object,
            _IOrderDetailRepositoryMock.Object,
            _IProductPhaseRepositoryMock.Object,
            _IShipOrderDetailRepositoryMock.Object,
            _ISetRepositoryMock.Object,
            _UnitOfWorkMock.Object,
            _Validator);
    }

    [Fact]
    public async Task Handler_ShouldThrow_MyValidationException_WhenShipperNotExist()
    {
        var createShipOrderRequest = new CreateShipOrderRequest(
            "001201011091",
            DeliveryMethod.SHIP_ORDER,
            Guid.NewGuid(),
            DateTime.Now.AddDays(1),
            new List<ShipOrderDetailRequest>
            {
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.PRODUCT),
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.PRODUCT)
            });
        var createShipOrderCommand = new CreateShipOrderCommand("001201011091", createShipOrderRequest);

        _IUserRepositoryMock.Setup(repo => repo.IsShipperExistAsync(It.IsAny<string>())).ReturnsAsync(false);

        await Assert.ThrowsAsync<MyValidationException>(async () =>
        {
            await _handler.Handle(createShipOrderCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_MyValidationException_WhenOrderNotExist()
    {
        var createShipOrderRequest = new CreateShipOrderRequest(
            "001201011091",
            DeliveryMethod.SHIP_ORDER,
            Guid.NewGuid(),
            DateTime.Now.AddDays(1),
            new List<ShipOrderDetailRequest>
            {
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.PRODUCT),
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.PRODUCT)
            });
        var createShipOrderCommand = new CreateShipOrderCommand("001201011091", createShipOrderRequest);

        _IUserRepositoryMock.Setup(repo => repo.IsShipperExistAsync(It.IsAny<string>())).ReturnsAsync(true);
        _IOrderRepositoryMock.Setup(repo => repo.IsOrderIdValidToShipAsync(It.IsAny<Guid>())).ReturnsAsync(false);

        await Assert.ThrowsAsync<MyValidationException>(async () =>
        {
            await _handler.Handle(createShipOrderCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_MyValidationException_WhenSomeProductNotExist()
    {
        var createShipOrderRequest = new CreateShipOrderRequest(
            "001201011091",
            DeliveryMethod.SHIP_ORDER,
            Guid.NewGuid(),
            DateTime.Now.AddDays(1),
            new List<ShipOrderDetailRequest>
            {
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.PRODUCT),
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.PRODUCT)
            });
        var createShipOrderCommand = new CreateShipOrderCommand("001201011091", createShipOrderRequest);

        _IUserRepositoryMock.Setup(repo => repo.IsShipperExistAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        _IOrderRepositoryMock.Setup(repo => repo.IsOrderIdValidToShipAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.IsAllOrderDetailProductIdsExistedAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<MyValidationException>(async () =>
        {
            await _handler.Handle(createShipOrderCommand, default);
        });
    }

    [Theory]
    [InlineData("")]
    [InlineData("0999")]
    [InlineData("9999999999999999999")]
    [InlineData("ddddddddddddddddddddddd")]
    [InlineData("ddddddddd")]
    public async Task Handler_ShouldThrow_MyValidationException_ShipperIdWrongFormat(string shipperId)
    {
        var createShipOrderRequest = new CreateShipOrderRequest(
            shipperId,
            DeliveryMethod.SHIP_ORDER,
            Guid.NewGuid(),
            DateTime.Now.AddDays(1),
            new List<ShipOrderDetailRequest>
            {
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.PRODUCT),
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.PRODUCT)
            });
        var createShipOrderCommand = new CreateShipOrderCommand("001201011091", createShipOrderRequest);

        _IUserRepositoryMock.Setup(repo => repo.IsShipperExistAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        _IOrderRepositoryMock.Setup(repo => repo.IsOrderIdValidToShipAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.IsAllOrderDetailProductIdsExistedAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<MyValidationException>(async () =>
        {
            await _handler.Handle(createShipOrderCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_MyValidationException_ShipDateLessThanCurrentDate()
    {
        var createShipOrderRequest = new CreateShipOrderRequest(
            "001201011091",
            DeliveryMethod.SHIP_ORDER,
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(-1),
            new List<ShipOrderDetailRequest>
            {
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.PRODUCT),
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.PRODUCT)
            });
        var createShipOrderCommand = new CreateShipOrderCommand("001201011091", createShipOrderRequest);

        _IUserRepositoryMock.Setup(repo => repo.IsShipperExistAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        _IOrderRepositoryMock.Setup(repo => repo.IsOrderIdValidToShipAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.IsAllOrderDetailProductIdsExistedAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<MyValidationException>(async () =>
        {
            await _handler.Handle(createShipOrderCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_MyValidationException_KindOfShipNotFound()
    {
        var createShipOrderRequest = new CreateShipOrderRequest(
            "001201011091",
            (DeliveryMethod)8,
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(-1),
            new List<ShipOrderDetailRequest>
            {
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.PRODUCT),
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.PRODUCT)
            });
        var createShipOrderCommand = new CreateShipOrderCommand("001201011091", createShipOrderRequest);

        _IUserRepositoryMock.Setup(repo => repo.IsShipperExistAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        _IOrderRepositoryMock.Setup(repo => repo.IsOrderIdValidToShipAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.IsAllOrderDetailProductIdsExistedAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<MyValidationException>(async () =>
        {
            await _handler.Handle(createShipOrderCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_MyValidationException_ItemKindNotFound()
    {
        var createShipOrderRequest = new CreateShipOrderRequest(
            "001201011091",
            DeliveryMethod.SHIP_ORDER,
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(-1),
            new List<ShipOrderDetailRequest>
            {
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, (ItemKind)9),
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.PRODUCT)
            });
        var createShipOrderCommand = new CreateShipOrderCommand("001201011091", createShipOrderRequest);

        _IUserRepositoryMock.Setup(repo => repo.IsShipperExistAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        _IOrderRepositoryMock.Setup(repo => repo.IsOrderIdValidToShipAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.IsAllOrderDetailProductIdsExistedAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<MyValidationException>(async () =>
        {
            await _handler.Handle(createShipOrderCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_MyValidationException_QuantityLessThan0()
    {
        var createShipOrderRequest = new CreateShipOrderRequest(
            "001201011091",
            DeliveryMethod.SHIP_ORDER,
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(-1),
            new List<ShipOrderDetailRequest>
            {
                new ShipOrderDetailRequest(Guid.NewGuid(), -3, ItemKind.PRODUCT),
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.PRODUCT)
            });
        var createShipOrderCommand = new CreateShipOrderCommand("001201011091", createShipOrderRequest);

        _IUserRepositoryMock.Setup(repo => repo.IsShipperExistAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        _IOrderRepositoryMock.Setup(repo => repo.IsOrderIdValidToShipAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.IsAllOrderDetailProductIdsExistedAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<MyValidationException>(async () =>
        {
            await _handler.Handle(createShipOrderCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_ArgumentNullException_ShipOrderDetailRequestsNull()
    {
        var createShipOrderRequest = new CreateShipOrderRequest(
            "001201011091",
            DeliveryMethod.SHIP_ORDER,
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1),
            null);
        var createShipOrderCommand = new CreateShipOrderCommand("001201011091", createShipOrderRequest);

        _IUserRepositoryMock.Setup(repo => repo.IsShipperExistAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        _IOrderRepositoryMock.Setup(repo => repo.IsOrderIdValidToShipAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.IsAllOrderDetailProductIdsExistedAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _handler.Handle(createShipOrderCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_MyValidationException_ShipOrderDetailRequestsEmpty()
    {
        var createShipOrderRequest = new CreateShipOrderRequest(
            "001201011091",
            DeliveryMethod.SHIP_ORDER,
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1),
            new List<ShipOrderDetailRequest> { });
        var createShipOrderCommand = new CreateShipOrderCommand("001201011091", createShipOrderRequest);

        _IUserRepositoryMock.Setup(repo => repo.IsShipperExistAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        _IOrderRepositoryMock.Setup(repo => repo.IsOrderIdValidToShipAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.IsAllOrderDetailProductIdsExistedAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<MyValidationException>(async () =>
        {
            await _handler.Handle(createShipOrderCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_OrderDetailNotFoundException_WhenOrderDetailNotFound()
    {
        var createShipOrderRequest = new CreateShipOrderRequest(
            "001201011091",
            DeliveryMethod.SHIP_ORDER,
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1),
            new List<ShipOrderDetailRequest> 
            {
                new ShipOrderDetailRequest(Guid.NewGuid(), 3, ItemKind.PRODUCT),
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.PRODUCT)
            });
        var createShipOrderCommand = new CreateShipOrderCommand("001201011091", createShipOrderRequest);

        var year = DateTime.Now.Year;
        var request = new CreateOrderRequest(
                CompanyId: Guid.NewGuid(),
                Status: StatusOrder.INPROGRESS,
                StartOrder: $"01/02/{year -1}",
                EndOrder: $"01/01/{year + 1}", // End date before start date
                VAT: 10,
                OrderDetailRequests: new List<OrderDetailRequest>
                {
                    new OrderDetailRequest(
                        ProductIdOrSetId: Guid.NewGuid(),
                        Quantity: 100,
                        UnitPrice: 10,
                        Note: "Note",
                        isProductId: true
                    )
                });
        var order = Order.Create(request, "dihson103");

        _IUserRepositoryMock.Setup(repo => repo.IsShipperExistAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        _IOrderRepositoryMock.Setup(repo => repo.IsOrderIdValidToShipAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.IsAllOrderDetailProductIdsExistedAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.GetOrderDetailsByOrderIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new List<OrderDetail> { });
        _IOrderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(order);

        await Assert.ThrowsAsync<OrderDetailNotFoundException>(async () =>
        {
            await _handler.Handle(createShipOrderCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_QuantityNotValidException_SendProductNumberNotValid()
    {
        var itemId = Guid.NewGuid();
        var orderDetail = OrderDetail.Create(Guid.NewGuid(), 0, new OrderDetailRequest(itemId, 3, 333, "note", true));
        var createShipOrderRequest = new CreateShipOrderRequest(
            "001201011091",
            DeliveryMethod.SHIP_ORDER,
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1),
            new List<ShipOrderDetailRequest>
            {
                new ShipOrderDetailRequest(itemId, 5, ItemKind.PRODUCT)
            });
        var createShipOrderCommand = new CreateShipOrderCommand("001201011091", createShipOrderRequest);

        var year = DateTime.Now.Year;
        var request = new CreateOrderRequest(
                CompanyId: Guid.NewGuid(),
                Status: StatusOrder.INPROGRESS,
                StartOrder: $"01/02/{year - 1}",
                EndOrder: $"01/01/{year + 1}", // End date before start date
                VAT: 10,
                OrderDetailRequests: new List<OrderDetailRequest>
                {
                    new OrderDetailRequest(
                        ProductIdOrSetId: Guid.NewGuid(),
                        Quantity: 100,
                        UnitPrice: 10,
                        Note: "Note",
                        isProductId: true
                    )
                });
        var order = Order.Create(request, "dihson103");

        _IUserRepositoryMock.Setup(repo => repo.IsShipperExistAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        _IOrderRepositoryMock.Setup(repo => repo.IsOrderIdValidToShipAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.IsAllOrderDetailProductIdsExistedAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.GetOrderDetailsByOrderIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new List<OrderDetail> { orderDetail });
        _IOrderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(order);

        await Assert.ThrowsAsync<QuantityNotValidException>(async () =>
        {
            await _handler.Handle(createShipOrderCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_MyValidationException_SetNotFoundOrderDetail()
    {
        var itemId = Guid.NewGuid();
        var createShipOrderRequest = new CreateShipOrderRequest(
            "001201011091",
            DeliveryMethod.SHIP_ORDER,
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1),
            new List<ShipOrderDetailRequest>
            {
                new ShipOrderDetailRequest(itemId, 5, ItemKind.SET)
            });
        var createShipOrderCommand = new CreateShipOrderCommand("001201011091", createShipOrderRequest);

        _IUserRepositoryMock.Setup(repo => repo.IsShipperExistAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        _IOrderRepositoryMock.Setup(repo => repo.IsOrderIdValidToShipAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.IsAllOrderDetailProductIdsExistedAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.IsAllOrderDetailSetIdsExistedAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()))
            .ReturnsAsync(false);

        var ex = await Assert.ThrowsAsync<MyValidationException>(async () =>
        {
            await _handler.Handle(createShipOrderCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_SetNotFoundException_SetNotFoundInDatabase()
    {
        var itemId = Guid.NewGuid();
        var createShipOrderRequest = new CreateShipOrderRequest(
            "001201011091",
            DeliveryMethod.SHIP_ORDER,
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1),
            new List<ShipOrderDetailRequest>
            {
                new ShipOrderDetailRequest(itemId, 5, ItemKind.SET)
            });
        var createShipOrderCommand = new CreateShipOrderCommand("001201011091", createShipOrderRequest);

        var year = DateTime.Now.Year;
        var request = new CreateOrderRequest(
                CompanyId: Guid.NewGuid(),
                Status: StatusOrder.INPROGRESS,
                StartOrder: $"01/02/{year - 1}",
                EndOrder: $"01/01/{year + 1}", // End date before start date
                VAT: 10,
                OrderDetailRequests: new List<OrderDetailRequest>
                {
                    new OrderDetailRequest(
                        ProductIdOrSetId: Guid.NewGuid(),
                        Quantity: 100,
                        UnitPrice: 10,
                        Note: "Note",
                        isProductId: true
                    )
                });
        var order = Order.Create(request, "dihson103");

        _IUserRepositoryMock.Setup(repo => repo.IsShipperExistAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        _IOrderRepositoryMock.Setup(repo => repo.IsOrderIdValidToShipAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.IsAllOrderDetailProductIdsExistedAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.IsAllOrderDetailSetIdsExistedAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()))
            .ReturnsAsync(true);
        _ISetRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Set)null);
        _IOrderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(order);

        var ex = await Assert.ThrowsAsync<SetNotFoundException>(async () =>
        {
            await _handler.Handle(createShipOrderCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_QuantityNotValidException_SendSetNumberNotValid()
    {
        var itemId = Guid.NewGuid();
        var orderDetail = OrderDetail.Create(Guid.NewGuid(), 0, new OrderDetailRequest(itemId, 3, 333, "note", false));
        var createShipOrderRequest = new CreateShipOrderRequest(
            "001201011091",
            DeliveryMethod.SHIP_ORDER,
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1),
            new List<ShipOrderDetailRequest>
            {
                new ShipOrderDetailRequest(itemId, 5, ItemKind.SET)
            });
        var createShipOrderCommand = new CreateShipOrderCommand("001201011091", createShipOrderRequest);
        var set = Set.Create(new CreateSetCommand(new CreateSetRequest("SE123", "Bộ", "Miêu tả", "Image", new List<SetProductRequest> { new SetProductRequest(itemId, 5) }), "0000000001"));

        var year = DateTime.Now.Year;
        var request = new CreateOrderRequest(
                CompanyId: Guid.NewGuid(),
                Status: StatusOrder.INPROGRESS,
                StartOrder: $"01/02/{year - 1}",
                EndOrder: $"01/01/{year + 1}", // End date before start date
                VAT: 10,
                OrderDetailRequests: new List<OrderDetailRequest>
                {
                    new OrderDetailRequest(
                        ProductIdOrSetId: Guid.NewGuid(),
                        Quantity: 100,
                        UnitPrice: 10,
                        Note: "Note",
                        isProductId: true
                    )
                });
        var order = Order.Create(request, "dihson103");

        _IUserRepositoryMock.Setup(repo => repo.IsShipperExistAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        _IOrderRepositoryMock.Setup(repo => repo.IsOrderIdValidToShipAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.IsAllOrderDetailProductIdsExistedAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.IsAllOrderDetailSetIdsExistedAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()))
            .ReturnsAsync(true);
        _ISetRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Set)set);
        _IOrderDetailRepositoryMock.Setup(repo => repo.GetOrderDetailsByOrderIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new List<OrderDetail> { orderDetail });
        _IOrderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(order);

        await Assert.ThrowsAsync<QuantityNotValidException>(async () =>
        {
            await _handler.Handle(createShipOrderCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_QuantityNotValidException_WhenNotFoundProductInProductPhase()
    {
        var itemId = Guid.NewGuid();
        var orderDetail = OrderDetail.Create(Guid.NewGuid(), 0, new OrderDetailRequest(itemId, 3, 333, "note", true));
        var createShipOrderRequest = new CreateShipOrderRequest(
            "001201011091",
            DeliveryMethod.SHIP_ORDER,
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1),
            new List<ShipOrderDetailRequest> 
            {
                new ShipOrderDetailRequest(itemId, 1, ItemKind.PRODUCT)
            });
        var createShipOrderCommand = new CreateShipOrderCommand("001201011091", createShipOrderRequest);

        var year = DateTime.Now.Year;
        var request = new CreateOrderRequest(
                CompanyId: Guid.NewGuid(),
                Status: StatusOrder.INPROGRESS,
                StartOrder: $"01/02/{year - 1}",
                EndOrder: $"01/01/{year + 1}", // End date before start date
                VAT: 10,
                OrderDetailRequests: new List<OrderDetailRequest>
                {
                    new OrderDetailRequest(
                        ProductIdOrSetId: Guid.NewGuid(),
                        Quantity: 100,
                        UnitPrice: 10,
                        Note: "Note",
                        isProductId: true
                    )
                });
        var order = Order.Create(request, "dihson103");

        _IUserRepositoryMock.Setup(repo => repo.IsShipperExistAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        _IOrderRepositoryMock.Setup(repo => repo.IsOrderIdValidToShipAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.IsAllOrderDetailProductIdsExistedAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.GetOrderDetailsByOrderIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new List<OrderDetail> { orderDetail });
        _IProductPhaseRepositoryMock.Setup(repo => repo.GetProductPhaseOfMainFactoryDoneByProductIdsAsync(It.IsAny<List<Guid>>()))
            .ReturnsAsync(new List<ProductPhase>{});
        _IOrderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(order);

        await Assert.ThrowsAsync<QuantityNotValidException>(async () =>
        {
            await _handler.Handle(createShipOrderCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_QuantityNotValidException_WhenQuantityNotEnough()
    {
        var itemId = Guid.NewGuid();
        var orderDetail = OrderDetail.Create(Guid.NewGuid(), 0, new OrderDetailRequest(itemId, 13, 333, "note", true));
        var productPhase = ProductPhase.Create(new CreateProductPhaseRequest(itemId, Guid.NewGuid(), 3, 3, Guid.NewGuid()));

        var createShipOrderRequest = new CreateShipOrderRequest(
            "001201011091",
            DeliveryMethod.SHIP_ORDER,
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1),
            new List<ShipOrderDetailRequest>
            {
                new ShipOrderDetailRequest(itemId, 5, ItemKind.PRODUCT)
            });
        var createShipOrderCommand = new CreateShipOrderCommand("001201011091", createShipOrderRequest);

        var year = DateTime.Now.Year;
        var request = new CreateOrderRequest(
                CompanyId: Guid.NewGuid(),
                Status: StatusOrder.INPROGRESS,
                StartOrder: $"01/02/{year - 1}",
                EndOrder: $"01/01/{year + 1}", // End date before start date
                VAT: 10,
                OrderDetailRequests: new List<OrderDetailRequest>
                {
                    new OrderDetailRequest(
                        ProductIdOrSetId: Guid.NewGuid(),
                        Quantity: 100,
                        UnitPrice: 10,
                        Note: "Note",
                        isProductId: true
                    )
                });
        var order = Order.Create(request, "dihson103");

        _IUserRepositoryMock.Setup(repo => repo.IsShipperExistAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        _IOrderRepositoryMock.Setup(repo => repo.IsOrderIdValidToShipAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.IsAllOrderDetailProductIdsExistedAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.GetOrderDetailsByOrderIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new List<OrderDetail> { orderDetail });
        _IProductPhaseRepositoryMock.Setup(repo => repo.GetProductPhaseOfMainFactoryDoneByProductIdsAsync(It.IsAny<List<Guid>>()))
            .ReturnsAsync(new List<ProductPhase> { productPhase });
        _IOrderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(order);

        await Assert.ThrowsAsync<QuantityNotValidException>(async () =>
        {
            await _handler.Handle(createShipOrderCommand, default);
        });
    }

    [Fact]
    public async Task Handler_Should_AddShipOrderHaveProductOnly_Success()
    {
        var itemId = Guid.NewGuid();
        var orderDetail = OrderDetail.Create(Guid.NewGuid(), 0, new OrderDetailRequest(itemId, 13, 333, "note", true));
        var productPhase = ProductPhase.Create(new CreateProductPhaseRequest(itemId, Guid.NewGuid(), 31, 31, Guid.NewGuid()));

        var createShipOrderRequest = new CreateShipOrderRequest(
            "001201011091",
            DeliveryMethod.SHIP_ORDER,
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1),
            new List<ShipOrderDetailRequest>
            {
                new ShipOrderDetailRequest(itemId, 5, ItemKind.PRODUCT)
            });
        var createShipOrderCommand = new CreateShipOrderCommand("001201011091", createShipOrderRequest);

        var year = DateTime.Now.Year;
        var request = new CreateOrderRequest(
                CompanyId: Guid.NewGuid(),
                Status: StatusOrder.INPROGRESS,
                StartOrder: $"01/02/{year - 1}",
                EndOrder: $"01/01/{year + 1}", // End date before start date
                VAT: 10,
                OrderDetailRequests: new List<OrderDetailRequest>
                {
                    new OrderDetailRequest(
                        ProductIdOrSetId: Guid.NewGuid(),
                        Quantity: 100,
                        UnitPrice: 10,
                        Note: "Note",
                        isProductId: true
                    )
                });
        var order = Order.Create(request, "dihson103");

        _IUserRepositoryMock.Setup(repo => repo.IsShipperExistAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        _IOrderRepositoryMock.Setup(repo => repo.IsOrderIdValidToShipAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.IsAllOrderDetailProductIdsExistedAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.GetOrderDetailsByOrderIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new List<OrderDetail> { orderDetail });
        _IProductPhaseRepositoryMock.Setup(repo => repo.GetProductPhaseOfMainFactoryDoneByProductIdsAsync(It.IsAny<List<Guid>>()))
            .ReturnsAsync(new List<ProductPhase> { productPhase });
        _IOrderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(order);

        await _handler.Handle(createShipOrderCommand, default);

        _IShipOrderRepositoryMock.Verify(repo => repo.Add(It.IsAny<ShipOrder>()), Times.Once);
        _IShipOrderDetailRepositoryMock.Verify(repo => repo.AddRange(It.IsAny<List<ShipOrderDetail>>()), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_AddShipOrderHaveSetOnly_Success()
    {
        var itemId = Guid.NewGuid();
        var orderDetail = OrderDetail.Create(Guid.NewGuid(), 0, new OrderDetailRequest(itemId, 13, 333, "note", false));
        var productPhase = ProductPhase.Create(new CreateProductPhaseRequest(itemId, Guid.NewGuid(), 31, 31, Guid.NewGuid()));

        var createShipOrderRequest = new CreateShipOrderRequest(
            "001201011091",
            DeliveryMethod.SHIP_ORDER,
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1),
            new List<ShipOrderDetailRequest>
            {
                new ShipOrderDetailRequest(itemId, 5, ItemKind.SET)
            });
        var createShipOrderCommand = new CreateShipOrderCommand("001201011091", createShipOrderRequest);
        var set = Set.Create(new CreateSetCommand(new CreateSetRequest("SE123", "Bộ", "Miêu tả", "Image", new List<SetProductRequest> { new SetProductRequest(itemId, 5) }), "0000000001"));

        var year = DateTime.Now.Year;
        var request = new CreateOrderRequest(
                CompanyId: Guid.NewGuid(),
                Status: StatusOrder.INPROGRESS,
                StartOrder: $"01/02/{year - 1}",
                EndOrder: $"01/01/{year + 1}", // End date before start date
                VAT: 10,
                OrderDetailRequests: new List<OrderDetailRequest>
                {
                    new OrderDetailRequest(
                        ProductIdOrSetId: Guid.NewGuid(),
                        Quantity: 100,
                        UnitPrice: 10,
                        Note: "Note",
                        isProductId: true
                    )
                });
        var order = Order.Create(request, "dihson103");

        _IUserRepositoryMock.Setup(repo => repo.IsShipperExistAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        _IOrderRepositoryMock.Setup(repo => repo.IsOrderIdValidToShipAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.IsAllOrderDetailProductIdsExistedAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.IsAllOrderDetailSetIdsExistedAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()))
            .ReturnsAsync(true);
        _ISetRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Set)set);
        _IOrderDetailRepositoryMock.Setup(repo => repo.GetOrderDetailsByOrderIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new List<OrderDetail> { orderDetail });
        _IProductPhaseRepositoryMock.Setup(repo => repo.GetProductPhaseOfMainFactoryDoneByProductIdsAsync(It.IsAny<List<Guid>>()))
            .ReturnsAsync(new List<ProductPhase> { productPhase });
        _IOrderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(order);

        await _handler.Handle(createShipOrderCommand, default);

        _IShipOrderRepositoryMock.Verify(repo => repo.Add(It.IsAny<ShipOrder>()), Times.Once);
        _IShipOrderDetailRepositoryMock.Verify(repo => repo.AddRange(It.IsAny<List<ShipOrderDetail>>()), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_AddShipOrderHaveBothProductAndSetOnly_Success()
    {
        var productId = Guid.NewGuid();
        var setId = Guid.NewGuid();
        var productOrderDetail = OrderDetail.Create(Guid.NewGuid(), 0, new OrderDetailRequest(productId, 13, 333, "note", true));
        var setOrderDetail = OrderDetail.Create(Guid.NewGuid(), 0, new OrderDetailRequest(setId, 13, 333, "note", false));
        var productPhase = ProductPhase.Create(new CreateProductPhaseRequest(productId, Guid.NewGuid(), 31, 31, Guid.NewGuid()));

        var createShipOrderRequest = new CreateShipOrderRequest(
            "001201011091",
            DeliveryMethod.SHIP_ORDER,
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1),
            new List<ShipOrderDetailRequest>
            {
                new ShipOrderDetailRequest(setId, 5, ItemKind.SET),
                new ShipOrderDetailRequest(productId, 5, ItemKind.PRODUCT)
            });
        var createShipOrderCommand = new CreateShipOrderCommand("001201011091", createShipOrderRequest);
        var set = Set.Create(new CreateSetCommand(new CreateSetRequest("SE123", "Bộ", "Miêu tả", "Image", new List<SetProductRequest> { new SetProductRequest(productId, 5) }), "0000000001"));

        var year = DateTime.Now.Year;
        var request = new CreateOrderRequest(
                CompanyId: Guid.NewGuid(),
                Status: StatusOrder.INPROGRESS,
                StartOrder: $"01/02/{year - 1}",
                EndOrder: $"01/01/{year + 1}", // End date before start date
                VAT: 10,
                OrderDetailRequests: new List<OrderDetailRequest>
                {
                    new OrderDetailRequest(
                        ProductIdOrSetId: Guid.NewGuid(),
                        Quantity: 100,
                        UnitPrice: 10,
                        Note: "Note",
                        isProductId: true
                    )
                });
        var order = Order.Create(request, "dihson103");

        _IUserRepositoryMock.Setup(repo => repo.IsShipperExistAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        _IOrderRepositoryMock.Setup(repo => repo.IsOrderIdValidToShipAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.IsAllOrderDetailProductIdsExistedAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.IsAllOrderDetailSetIdsExistedAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()))
            .ReturnsAsync(true);
        _ISetRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Set)set);
        _IOrderDetailRepositoryMock.Setup(repo => repo.GetOrderDetailsByOrderIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new List<OrderDetail> { productOrderDetail, setOrderDetail });
        _IProductPhaseRepositoryMock.Setup(repo => repo.GetProductPhaseOfMainFactoryDoneByProductIdsAsync(It.IsAny<List<Guid>>()))
            .ReturnsAsync(new List<ProductPhase> { productPhase });
        _IOrderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(order);

        await _handler.Handle(createShipOrderCommand, default);

        _IShipOrderRepositoryMock.Verify(repo => repo.Add(It.IsAny<ShipOrder>()), Times.Once);
        _IShipOrderDetailRepositoryMock.Verify(repo => repo.AddRange(It.IsAny<List<ShipOrderDetail>>()), Times.Once);
    }
}
