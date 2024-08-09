using Application.Abstractions.Data;
using Contract.Services.ShipOrder.Create;
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
        //_Validator = new Create
    }
}
