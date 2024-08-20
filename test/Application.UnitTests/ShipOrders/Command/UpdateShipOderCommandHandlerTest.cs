using Application.Abstractions.Data;
using Application.UseCases.Commands.Shipments.Update;
using Application.UserCases.Commands.ShipOrders.Create;
using Application.UserCases.Commands.ShipOrders.Update;
using Contract.Services.ShipOrder.Create;
using Contract.Services.ShipOrder.Share;
using Contract.Services.ShipOrder.Update;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using Domain.Exceptions.ShipOrder;
using FluentValidation;
using Moq;

namespace Application.UnitTests.ShipOrders.Command;

public class UpdateShipOderCommandHandlerTest
{
    private readonly Mock<IShipOrderRepository> _shipOrderRepositoryMock;
    private readonly Mock<ISetRepository> _setRepositoryMock;
    private readonly Mock<IOrderDetailRepository> _orderDetailRepositoryMock;
    private readonly Mock<IProductPhaseRepository> _productPhaseRepositoryMock;
    private readonly Mock<IValidator<UpdateShipOrderRequest>> _validatorMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdateShipOderCommandHandler _handler;
    private readonly Mock<IUserRepository> _IUserRepositoryMock;
    private readonly Mock<IOrderRepository> _IOrderRepositoryMock;
    private readonly Mock<IOrderDetailRepository> _IOrderDetailRepositoryMock;
    private readonly IValidator<UpdateShipOrderRequest> _Validator;

    public UpdateShipOderCommandHandlerTest()
    {
        _shipOrderRepositoryMock = new Mock<IShipOrderRepository>();
        _setRepositoryMock = new Mock<ISetRepository>();
        _orderDetailRepositoryMock = new Mock<IOrderDetailRepository>();
        _productPhaseRepositoryMock = new Mock<IProductPhaseRepository>();
        //_validatorMock = new Mock<IValidator<UpdateShipOrderRequest>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _IUserRepositoryMock = new();
        _IOrderDetailRepositoryMock = new Mock<IOrderDetailRepository>();
        _IOrderRepositoryMock = new Mock<IOrderRepository>();

        _Validator = new UpdateShipOrderValidator(
            _shipOrderRepositoryMock.Object,
           _IUserRepositoryMock.Object,
           _IOrderRepositoryMock.Object,
           _IOrderDetailRepositoryMock.Object);

        _handler = new UpdateShipOderCommandHandler(
            _shipOrderRepositoryMock.Object,
            _setRepositoryMock.Object,
            _orderDetailRepositoryMock.Object,
            _productPhaseRepositoryMock.Object,
            _Validator,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handler_ShouldThrow_ShipOrderIdConflictException_WhenShipOrderIdNotSame ()
    {
        var shipOrderId = Guid.NewGuid();
        var updateShipOrderRequest = new UpdateShipOrderRequest(
            shipOrderId,
            "001201011091",
            DeliveryMethod.SHIP_ORDER,
            Guid.NewGuid(),
            DateTime.Now.AddDays(1),
            new List<ShipOrderDetailRequest>
            {
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.PRODUCT),
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.PRODUCT)
            });
        var updateShipOrderCommand = new UpdateShipOderCommand("Dihson103", Guid.NewGuid(), updateShipOrderRequest);

        await Assert.ThrowsAsync<ShipOrderIdConflictException>(async () =>
        {
            await _handler.Handle(updateShipOrderCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_MyValidationException_WhenShipperNotExist()
    {
        var shipOrderId = Guid.NewGuid();
        var updateShipOrderRequest = new UpdateShipOrderRequest(
            shipOrderId,
            "001201011091",
            DeliveryMethod.SHIP_ORDER,
            Guid.NewGuid(),
            DateTime.Now.AddDays(1),
            new List<ShipOrderDetailRequest>
            {
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.PRODUCT),
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.PRODUCT)
            });
        var updateShipOrderCommand = new UpdateShipOderCommand("Dihson103", shipOrderId, updateShipOrderRequest);

        _IUserRepositoryMock.Setup(repo => repo.IsShipperExistAsync(It.IsAny<string>())).ReturnsAsync(false);

        await Assert.ThrowsAsync<MyValidationException>(async () =>
        {
            await _handler.Handle(updateShipOrderCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_MyValidationException_WhenSomeProductNotExist()
    {
        var shipOrderId = Guid.NewGuid();
        var updateShipOrderRequest = new UpdateShipOrderRequest(
            shipOrderId,
            "001201011091",
            DeliveryMethod.SHIP_ORDER,
            Guid.NewGuid(),
            DateTime.Now.AddDays(1),
            new List<ShipOrderDetailRequest>
            {
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.PRODUCT),
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.PRODUCT)
            });
        var updateShipOrderCommand = new UpdateShipOderCommand("Dihson103", shipOrderId, updateShipOrderRequest);

        _IUserRepositoryMock.Setup(repo => repo.IsShipperExistAsync(It.IsAny<string>())).ReturnsAsync(false);
        _IOrderRepositoryMock.Setup(repo => repo.IsOrderIdValidToShipAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.IsAllOrderDetailProductIdsExistedAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<MyValidationException>(async () =>
        {
            await _handler.Handle(updateShipOrderCommand, default);
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
        var shipOrderId = Guid.NewGuid();
        var updateShipOrderRequest = new UpdateShipOrderRequest(
            shipOrderId,
            shipperId,
            DeliveryMethod.SHIP_ORDER,
            Guid.NewGuid(),
            DateTime.Now.AddDays(1),
            new List<ShipOrderDetailRequest>
            {
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.PRODUCT),
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.PRODUCT)
            });
        var updateShipOrderCommand = new UpdateShipOderCommand("Dihson103", shipOrderId, updateShipOrderRequest);

        _IUserRepositoryMock.Setup(repo => repo.IsShipperExistAsync(It.IsAny<string>())).ReturnsAsync(false);
        _IOrderRepositoryMock.Setup(repo => repo.IsOrderIdValidToShipAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.IsAllOrderDetailProductIdsExistedAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<MyValidationException>(async () =>
        {
            await _handler.Handle(updateShipOrderCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_MyValidationException_ShipDateLessThanCurrentDate()
    {
        var shipOrderId = Guid.NewGuid();
        var updateShipOrderRequest = new UpdateShipOrderRequest(
            shipOrderId,
            "001201011091",
            DeliveryMethod.SHIP_ORDER,
            Guid.NewGuid(),
            DateTime.Now.AddDays(-1),
            new List<ShipOrderDetailRequest>
            {
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.PRODUCT),
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.PRODUCT)
            });
        var updateShipOrderCommand = new UpdateShipOderCommand("Dihson103", shipOrderId, updateShipOrderRequest);

        _IUserRepositoryMock.Setup(repo => repo.IsShipperExistAsync(It.IsAny<string>())).ReturnsAsync(false);
        _IOrderRepositoryMock.Setup(repo => repo.IsOrderIdValidToShipAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.IsAllOrderDetailProductIdsExistedAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<MyValidationException>(async () =>
        {
            await _handler.Handle(updateShipOrderCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_MyValidationException_KindOfShipNotFound()
    {
        var shipOrderId = Guid.NewGuid();
        var updateShipOrderRequest = new UpdateShipOrderRequest(
            shipOrderId,
            "001201011091",
            (DeliveryMethod)8,
            Guid.NewGuid(),
            DateTime.Now.AddDays(1),
            new List<ShipOrderDetailRequest>
            {
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.PRODUCT),
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.PRODUCT)
            });
        var updateShipOrderCommand = new UpdateShipOderCommand("Dihson103", shipOrderId, updateShipOrderRequest);

        _IUserRepositoryMock.Setup(repo => repo.IsShipperExistAsync(It.IsAny<string>())).ReturnsAsync(false);
        _IOrderRepositoryMock.Setup(repo => repo.IsOrderIdValidToShipAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.IsAllOrderDetailProductIdsExistedAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<MyValidationException>(async () =>
        {
            await _handler.Handle(updateShipOrderCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_MyValidationException_ItemKindNotFound()
    {
        var shipOrderId = Guid.NewGuid();
        var updateShipOrderRequest = new UpdateShipOrderRequest(
            shipOrderId,
            "001201011091",
            DeliveryMethod.SHIP_ORDER,
            Guid.NewGuid(),
            DateTime.Now.AddDays(1),
            new List<ShipOrderDetailRequest>
            {
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, (ItemKind)8),
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.PRODUCT)
            });
        var updateShipOrderCommand = new UpdateShipOderCommand("Dihson103", shipOrderId, updateShipOrderRequest);

        _IUserRepositoryMock.Setup(repo => repo.IsShipperExistAsync(It.IsAny<string>())).ReturnsAsync(false);
        _IOrderRepositoryMock.Setup(repo => repo.IsOrderIdValidToShipAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.IsAllOrderDetailProductIdsExistedAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<MyValidationException>(async () =>
        {
            await _handler.Handle(updateShipOrderCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_OrderDetailNotFoundException_WhenOrderDetailNotFound()
    {
        var shipOrderId = Guid.NewGuid();
        var updateShipOrderRequest = new UpdateShipOrderRequest(
            shipOrderId,
            "001201011091",
            DeliveryMethod.SHIP_ORDER,
            Guid.NewGuid(),
            DateTime.Now.AddDays(1),
            new List<ShipOrderDetailRequest>
            {
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.PRODUCT),
                new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.PRODUCT)
            });
        var updateShipOrderCommand = new UpdateShipOderCommand("Dihson103", shipOrderId, updateShipOrderRequest);

        _IUserRepositoryMock.Setup(repo => repo.IsShipperExistAsync(It.IsAny<string>())).ReturnsAsync(false);
        _IOrderRepositoryMock.Setup(repo => repo.IsOrderIdValidToShipAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.IsAllOrderDetailProductIdsExistedAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()))
            .ReturnsAsync(true);
        _IOrderDetailRepositoryMock.Setup(repo => repo.GetOrderDetailsByOrderIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new List<OrderDetail> { });

        await Assert.ThrowsAsync<MyValidationException>(async () =>
        {
            await _handler.Handle(updateShipOrderCommand, default);
        });
    }
}
