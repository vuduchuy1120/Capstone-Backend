using Application.Abstractions.Data;
using Application.UserCases.Commands.EmployeeProducts.Creates;
using Application.Utils;
using Contract.Services.EmployeeProduct.Creates;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using FluentValidation;
using Moq;

namespace Application.UnitTests.EmployeeProducts.Commands
{
    public class CreateEmployeeProductCommandHandlerTest
    {
        private readonly Mock<IEmployeeProductRepository> _employeeProductRepositoryMock;
        private readonly Mock<ISlotRepository> _slotRepositoryMock;
        private readonly Mock<IPhaseRepository> _phaseRepositoryMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly IValidator<CreateEmployeeProductRequest> _validator;
        private readonly CreateEmployeeProductCommandHandler _handler;

        public CreateEmployeeProductCommandHandlerTest()
        {
            _employeeProductRepositoryMock = new Mock<IEmployeeProductRepository>();
            _slotRepositoryMock = new Mock<ISlotRepository>();
            _phaseRepositoryMock = new Mock<IPhaseRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _validator = new CreateEmployeeProductRequestValidator(
                _productRepositoryMock.Object,
                _phaseRepositoryMock.Object,
                _slotRepositoryMock.Object,
                _userRepositoryMock.Object
                );
            _handler = new CreateEmployeeProductCommandHandler(
                _employeeProductRepositoryMock.Object,
                _validator,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handler_Should_Return_SuccessResult()
        {
            // Arrange
            var createEmployeeProductRequest = new CreateEmployeeProductRequest(
                Date: "01/01/2024",
                SlotId: 1,
                CreateQuantityProducts: new List<CreateQuantityProductRequest>
                {
                    new CreateQuantityProductRequest(Guid.NewGuid(), Guid.NewGuid(), 10, "user1", true)
                });
            var command = new CreateEmployeeProductComand(createEmployeeProductRequest, "admin");

            var createEmployeeProductCommandHandler = new CreateEmployeeProductCommandHandler(
                _employeeProductRepositoryMock.Object,
                _validator,
                _unitOfWorkMock.Object);

            _slotRepositoryMock.Setup(repo => repo.IsSlotExisted(createEmployeeProductRequest.SlotId))
                .ReturnsAsync(true);
            _userRepositoryMock.Setup(repo => repo.IsAllUserActiveAsync(It.IsAny<List<string>>()))
                .ReturnsAsync(true);
            _productRepositoryMock.Setup(repo => repo.IsAllProductIdsExistAsync(It.IsAny<List<Guid>>())).ReturnsAsync(true);
            _phaseRepositoryMock.Setup(repo => repo.IsAllPhaseExistByIdAsync(It.IsAny<List<Guid>>())).ReturnsAsync(true);

            _employeeProductRepositoryMock.Setup(repo => repo.GetEmployeeProductsByDateAndSlotId(createEmployeeProductRequest.SlotId, DateUtil.ConvertStringToDateTimeOnly(createEmployeeProductRequest.Date)))
                .ReturnsAsync(new List<EmployeeProduct>());

            // Act
            var result = await createEmployeeProductCommandHandler.Handle(command, default);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.isSuccess);
        }

        [Fact]
        public async Task Handler_Should_Throw_MyValidationException_WhenDateInvalid()
        {
            // Arrange
            var createEmployeeProductRequest = new CreateEmployeeProductRequest(
                Date: "01-01-2002",
                SlotId: 1,
                CreateQuantityProducts: new List<CreateQuantityProductRequest>
                {
                    new CreateQuantityProductRequest(Guid.NewGuid(), Guid.NewGuid(), 10, "user1", true)
                });
            var command = new CreateEmployeeProductComand(createEmployeeProductRequest, "admin");

            _slotRepositoryMock.Setup(repo => repo.IsSlotExisted(createEmployeeProductRequest.SlotId))
              .ReturnsAsync(true);
            _userRepositoryMock.Setup(repo => repo.IsAllUserActiveAsync(It.IsAny<List<string>>()))
                .ReturnsAsync(true);
            _productRepositoryMock.Setup(repo => repo.IsAllProductIdsExistAsync(It.IsAny<List<Guid>>())).ReturnsAsync(true);

            _phaseRepositoryMock.Setup(repo => repo.IsAllPhaseExistByIdAsync(It.IsAny<List<Guid>>())).ReturnsAsync(true);

            _employeeProductRepositoryMock.Setup(repo => repo.GetEmployeeProductsByDateAndSlotId(It.IsAny<int>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(new List<EmployeeProduct>());
            // Act & Assert
            await Assert.ThrowsAsync<MyValidationException>(async () => await _handler.Handle(command, default));
        }

        [Theory]
        [InlineData("01/01/2024", 10, 0)] // SlotId is invalid
        [InlineData("invalid-quantity", 10, 1)] // Invalid Quantity
        [InlineData("01/01/2024", -10, 2)] // UserIds are invalid

        public async Task Handler_Should_Throw_MyValidationException_WhenInputNotValid(
            string date,
            int quantity,
            int slotId)
        {
            // Arrange
            var createEmployeeProductRequest = new CreateEmployeeProductRequest(
                Date: date,
                SlotId: slotId,
                CreateQuantityProducts: new List<CreateQuantityProductRequest>
                {
                    new CreateQuantityProductRequest(Guid.NewGuid(), Guid.NewGuid(), quantity, "user1", true)
                });
            var command = new CreateEmployeeProductComand(createEmployeeProductRequest, "admin");


            _slotRepositoryMock.Setup(repo => repo.IsSlotExisted(createEmployeeProductRequest.SlotId))
              .ReturnsAsync(true);
            _userRepositoryMock.Setup(repo => repo.IsAllUserActiveAsync(It.IsAny<List<string>>()))
                .ReturnsAsync(true);
            _productRepositoryMock.Setup(repo => repo.IsAllProductIdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            _phaseRepositoryMock.Setup(repo => repo.IsAllPhaseExistByIdAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            _employeeProductRepositoryMock.Setup(repo => repo.GetEmployeeProductsByDateAndSlotId(
                It.IsAny<int>(),
                It.IsAny<DateOnly>()))
                .ReturnsAsync(new List<EmployeeProduct>());
            // Act & Assert
            await Assert.ThrowsAsync<MyValidationException>(() => _handler.Handle(command, default));
        }

        [Fact]
        public async Task Handler_Should_Throw_MyValidationException_WhenSlotIdNotFound()
        {
            // Arrange
            var createEmployeeProductRequest = new CreateEmployeeProductRequest(
                    Date: "01/01/2024",
                    SlotId: 1,
                    CreateQuantityProducts: new List<CreateQuantityProductRequest>
                    {
                        new CreateQuantityProductRequest(Guid.NewGuid(), Guid.NewGuid(), 10, "user1", true)
                    });
            var command = new CreateEmployeeProductComand(createEmployeeProductRequest, "admin");

            _slotRepositoryMock.Setup(repo => repo.IsSlotExisted(createEmployeeProductRequest.SlotId))
              .ReturnsAsync(false);
            _userRepositoryMock.Setup(repo => repo.IsAllUserActiveAsync(It.IsAny<List<string>>()))
                .ReturnsAsync(true);
            _productRepositoryMock.Setup(repo => repo.IsAllProductIdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            _phaseRepositoryMock.Setup(repo => repo.IsAllPhaseExistByIdAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            _employeeProductRepositoryMock.Setup(repo => repo.GetEmployeeProductsByDateAndSlotId(
                It.IsAny<int>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(new List<EmployeeProduct>());
            // Act & Assert
            await Assert.ThrowsAsync<MyValidationException>(() => _handler.Handle(command, default));
        }

        [Fact]
        public async Task Handler_Should_Throw_MyValidationException_WhenUserNotFound()
        {
            // Arrange
            var createEmployeeProductRequest = new CreateEmployeeProductRequest(
                    Date: "01/01/2024",
                    SlotId: 1,
                    CreateQuantityProducts: new List<CreateQuantityProductRequest>
                    {
                        new CreateQuantityProductRequest(Guid.NewGuid(), Guid.NewGuid(), 10, "user1", true)
                    });
            var command = new CreateEmployeeProductComand(createEmployeeProductRequest, "admin");

            _slotRepositoryMock.Setup(repo => repo.IsSlotExisted(createEmployeeProductRequest.SlotId))
              .ReturnsAsync(true);
            _userRepositoryMock.Setup(repo => repo.IsAllUserActiveAsync(It.IsAny<List<string>>()))
                .ReturnsAsync(false);
            _productRepositoryMock.Setup(repo => repo.IsAllProductIdsExistAsync(
                It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            _phaseRepositoryMock.Setup(repo => repo.IsAllPhaseExistByIdAsync(
                It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            _employeeProductRepositoryMock.Setup(repo => repo.GetEmployeeProductsByDateAndSlotId(
                It.IsAny<int>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(new List<EmployeeProduct>());
            // Act & Assert
            await Assert.ThrowsAsync<MyValidationException>(() => _handler.Handle(command, default));
        }

        [Fact]
        public async Task Handler_Should_Throw_MyValidationException_WhenPhaseIdNotFound()
        {
            // Arrange
            var createEmployeeProductRequest = new CreateEmployeeProductRequest(
                            Date: "01/01/2024",
                            SlotId: 1,
                            CreateQuantityProducts: new List<CreateQuantityProductRequest>
                            {
                                new CreateQuantityProductRequest(Guid.NewGuid(), Guid.NewGuid(), 10, "user1", true)
                            });
            var command = new CreateEmployeeProductComand(createEmployeeProductRequest, "admin");

            _slotRepositoryMock.Setup(repo => repo.IsSlotExisted(createEmployeeProductRequest.SlotId))
              .ReturnsAsync(true);
            _userRepositoryMock.Setup(repo => repo.IsAllUserActiveAsync(It.IsAny<List<string>>()))
                .ReturnsAsync(true);
            _productRepositoryMock.Setup(repo => repo.IsAllProductIdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            _phaseRepositoryMock.Setup(repo => repo.IsAllPhaseExistByIdAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(false);

            _employeeProductRepositoryMock.Setup(repo => repo.GetEmployeeProductsByDateAndSlotId(
                It.IsAny<int>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(new List<EmployeeProduct>());
            // Act & Assert
            await Assert.ThrowsAsync<MyValidationException>(() => _handler.Handle(command, default));
        }

        [Fact]
        public async Task Handler_Should_Throw_MyValidationException_WhenProductIdNotFound()
        {
            // Arrange
            var createEmployeeProductRequest = new CreateEmployeeProductRequest(
                Date: "01/01/2024",
                SlotId: 1,
                CreateQuantityProducts: new List<CreateQuantityProductRequest>
                {
                    new CreateQuantityProductRequest(Guid.NewGuid(), Guid.NewGuid(), 10, "user1", true)
                });
            var command = new CreateEmployeeProductComand(createEmployeeProductRequest, "admin");

            _slotRepositoryMock.Setup(repo => repo.IsSlotExisted(createEmployeeProductRequest.SlotId))
              .ReturnsAsync(true);
            _userRepositoryMock.Setup(repo => repo.IsAllUserActiveAsync(It.IsAny<List<string>>()))
                .ReturnsAsync(true);
            _productRepositoryMock.Setup(repo => repo.IsAllProductIdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(false);

            _phaseRepositoryMock.Setup(repo => repo.IsAllPhaseExistByIdAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            _employeeProductRepositoryMock.Setup(repo => repo.GetEmployeeProductsByDateAndSlotId(
                It.IsAny<int>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(new List<EmployeeProduct>());
            // Act & Assert
            await Assert.ThrowsAsync<MyValidationException>(() => _handler.Handle(command, default));
        }
    }
}
