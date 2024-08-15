using Application.Abstractions.Data;
using Application.UserCases.Commands.Orders.Creates;
using Contract.Services.Order.Creates;
using Contract.Services.Order.ShareDtos;
using Contract.Services.OrderDetail.Creates;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using FluentValidation;
using FluentValidation.TestHelper;
using Moq;
using Xunit;

namespace Application.UnitTests.Orders.Command
{
    public class CreateOrderCommandHandlerTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IOrderDetailRepository> _orderDetailRepositoryMock;
        private readonly IValidator<CreateOrderRequest> _validator;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateOrderCommandHandler _handler;

        public CreateOrderCommandHandlerTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _orderDetailRepositoryMock = new Mock<IOrderDetailRepository>();
            _validator = new CreateOrderRequestValidator(new Mock<ICompanyRepository>().Object, new Mock<IProductRepository>().Object, new Mock<ISetRepository>().Object);
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new CreateOrderCommandHandler(
                _orderRepositoryMock.Object,
                _orderDetailRepositoryMock.Object,
                _validator,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_SuccessResult()
        {
            // Arrange
            var request = new CreateOrderRequest(
                CompanyId: Guid.NewGuid(),
                Status: StatusOrder.SIGNED,
                StartOrder: "01/01/2024",
                EndOrder: "01/02/2024",
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

            var command = new CreateOrderCommand(request, "UserId");

            _orderRepositoryMock.Setup(x => x.AddOrder(It.IsAny<Order>()));
            _orderDetailRepositoryMock.Setup(x => x.AddRange(It.IsAny<List<OrderDetail>>()));
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.isSuccess);
        }

        [Fact]
        public async Task Handle_Should_Throw_MyValidationException_If_Validation_Fails()
        {
            // Arrange
            var request = new CreateOrderRequest(
                CompanyId: Guid.Empty, // Invalid CompanyId
                Status: StatusOrder.SIGNED,
                StartOrder: "01/01/2024",
                EndOrder: "01/02/2024",
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

            var command = new CreateOrderCommand(request, "UserId");

            // Act & Assert
            await Assert.ThrowsAsync<MyValidationException>(async () =>
                await _handler.Handle(command, default));
        }

        [Fact]
        public async Task Handle_Should_Throw_MyValidationException_If_EndDate_Is_Before_StartDate()
        {
            // Arrange
            var request = new CreateOrderRequest(
                CompanyId: Guid.NewGuid(),
                Status: StatusOrder.INPROGRESS,
                StartOrder: "01/02/2024",
                EndOrder: "01/01/2024", // End date before start date
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

            var command = new CreateOrderCommand(request, "UserId");

            // Act & Assert
            await Assert.ThrowsAsync<MyValidationException>(async () =>
                await _handler.Handle(command, default));
        }

        [Theory]
        [InlineData("", StatusOrder.SIGNED, "01/01/2023", 10, true)] // CompanyId empty
        [InlineData(null, StatusOrder.SIGNED, "01/01/2023", 10, true)] // CompanyId null
        [InlineData("01/01/2100", StatusOrder.COMPLETED, "01/01/2100", 10, true)] // StartOrder greater than current date for COMPLETED status
        [InlineData("01/01/2100", StatusOrder.CANCELLED, "01/01/2100", 10, true)] // StartOrder greater than current date for CANCELLED status
        [InlineData("01/01/2023", StatusOrder.SIGNED, "01/01/2023", 4, true)] // Invalid VAT (not in 0, 5, 8, 10)
        [InlineData("01/01/2023", StatusOrder.SIGNED, "01/01/2023", 15, true)] // Invalid VAT (not in 0, 5, 8, 10)
        [InlineData("01/01/2023", StatusOrder.SIGNED, "01/01/2023", 10, false)] // Valid scenario
        [InlineData("01/01/2023", StatusOrder.SIGNED, "01/01/2023", 0, false)] // Valid scenario with VAT 0
        [InlineData("01/01/2023", StatusOrder.SIGNED, "01/01/2023", 5, false)] // Valid scenario with VAT 5
        [InlineData("01/01/2023", StatusOrder.SIGNED, "01/01/2023", 8, false)] // Valid scenario with VAT 8
        public void ValidateOrderRequest(
            string startOrder,
            StatusOrder status,
            string expectedCompletionDate,
            int vat,
            bool shouldHaveErrors)
        {
            // Arrange
            var orderDetailRequests = new List<OrderDetailRequest>
            {
                new OrderDetailRequest
                (
                    ProductIdOrSetId : Guid.NewGuid(),
                    Quantity : 10,
                    UnitPrice : 100,
                    isProductId : true,
                    Note : "Note"
                )
            };

            var model = new CreateOrderRequest(
                CompanyId: Guid.NewGuid(),
                Status: status,
                StartOrder: startOrder,
                EndOrder: expectedCompletionDate,
                VAT: vat,
                OrderDetailRequests: orderDetailRequests);

            // Act
            var validationResult = _validator.TestValidate(model);

            // Assert
            if (shouldHaveErrors)
            {
                validationResult.ShouldHaveAnyValidationError();
            }
            else
            {
                validationResult.ShouldNotHaveAnyValidationErrors();
            }
        }
    }

}