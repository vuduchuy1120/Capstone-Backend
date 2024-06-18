using Application.UserCases.Commands.EmployeeProducts.Creates;
using Contract.Services.EmployeeProduct.Creates;
using FluentValidation;
using Moq;
using Application.Abstractions.Data;
using Domain.Entities;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Contract.Abstractions.Shared.Results;
using Domain.Abstractions.Exceptions;
using System.Linq;
using Application.Utils;
using System;

namespace Application.UnitTests.EmployeeProducts.Commands
{
    public class CreateEmployeeProductCommandHandlerTest
    {
        private readonly Mock<IEmployeeProductRepository> _employeeProductRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly IValidator<CreateEmployeeProductRequest> _validator;

        public CreateEmployeeProductCommandHandlerTest()
        {
            _employeeProductRepositoryMock = new Mock<IEmployeeProductRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _validator = new CreateEmployeeProductRequestValidator(
                new Mock<IProductRepository>().Object,
                new Mock<IPhaseRepository>().Object,
                new Mock<ISlotRepository>().Object,
                new Mock<IUserRepository>().Object);
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

             _validator.ValidateAsync(createEmployeeProductRequest);

            _employeeProductRepositoryMock.Setup(repo => repo.GetEmployeeProductsByDateAndSlotId(createEmployeeProductRequest.SlotId, DateUtil.ConvertStringToDateTimeOnly(createEmployeeProductRequest.Date)))
                .ReturnsAsync(new List<EmployeeProduct>());

            // Act
            var result = await createEmployeeProductCommandHandler.Handle(command, default);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.isSuccess);        }

        [Fact]
        public async Task Handler_Should_Throw_MyValidationException_WhenDateInvalid()
        {
            // Arrange
            var createEmployeeProductRequest = new CreateEmployeeProductRequest(
                Date: "invalid-date",
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

            // Act & Assert
            await Assert.ThrowsAsync<MyValidationException>(() => createEmployeeProductCommandHandler.Handle(command, default));
        }

        [Theory]
        [InlineData("01/01/2024", 0, "CreateQuantityProductRequest is required")] // SlotId is invalid
        [InlineData("invalid-date", 1, "Date must be a valid date in the format dd/MM/yyyy")] // Date is invalid
        public async Task Handler_Should_Throw_MyValidationException_WhenInputNotValid(string date, int slotId, string expectedErrorMessage)
        {
            // Arrange
            var createEmployeeProductRequest = new CreateEmployeeProductRequest(
                Date: date,
                SlotId: slotId,
                CreateQuantityProducts: new List<CreateQuantityProductRequest>
                {
                    new CreateQuantityProductRequest(Guid.NewGuid(), Guid.NewGuid(), 10, "user1", true)
                });
            var command = new CreateEmployeeProductComand(createEmployeeProductRequest, "admin");

            var createEmployeeProductCommandHandler = new CreateEmployeeProductCommandHandler(
                _employeeProductRepositoryMock.Object,
                _validator,
                _unitOfWorkMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<MyValidationException>(() => createEmployeeProductCommandHandler.Handle(command, default));
            Assert.Contains(expectedErrorMessage, exception.Message);
        }
    }
}
