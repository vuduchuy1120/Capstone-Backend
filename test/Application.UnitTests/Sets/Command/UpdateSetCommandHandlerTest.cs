using Application.Abstractions.Data;
using Application.UserCases.Commands.Sets.UpdateSet;
using Contract.Services.Set.SharedDto;
using Contract.Services.Set.UpdateSet;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using Domain.Exceptions.SetProducts;
using Domain.Exceptions.Sets;
using FluentValidation;
using Moq;
using Xunit;

namespace Application.UnitTests.Sets.Command
{
    public class UpdateSetCommandHandlerTest
    {
        private readonly Mock<ISetRepository> _setRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ISetProductRepository> _setProductRepositoryMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly IValidator<UpdateSetRequest> _validator;
        private readonly UpdateSetCommandHandler _updateSetCommandHandler;

        public UpdateSetCommandHandlerTest()
        {
            _setRepositoryMock = new();
            _unitOfWorkMock = new();
            _productRepositoryMock = new();
            _setProductRepositoryMock = new();
            _validator = new UpdateSetValidator(_setProductRepositoryMock.Object, _productRepositoryMock.Object);
            _updateSetCommandHandler = new(
                _setRepositoryMock.Object,
                _setProductRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _validator);
        }

        [Fact]
        public async Task Handler_Success_ShouldUpdateSet_WhenRemoveSetProductIsNull()
        {
            // Arrange
            var add = new List<SetProductRequest>
            {
                new SetProductRequest(Guid.NewGuid(), 5),
                new SetProductRequest(Guid.NewGuid(), 5),
            };

            var update = new List<SetProductRequest>
            {
                new SetProductRequest(Guid.NewGuid(), 5),
                new SetProductRequest(Guid.NewGuid(), 5),
            };

            var setId = Guid.NewGuid();
            var updateSetRequest = new UpdateSetRequest(setId, "CD123", "Name",
                "Description", "Image", add, update, null);
            var updateSetCommand = new UpdateSetCommand(updateSetRequest, "UpdatedBy", setId);

            _productRepositoryMock.Setup(repo => repo.IsAllSubProductIdsExist(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);
            _setProductRepositoryMock.Setup(repo => repo.IsAnyIdExistAsync(It.IsAny<List<Guid>>(), It.IsAny<Guid>()))
                .ReturnsAsync(false);
            _setProductRepositoryMock.Setup(repo => repo.DoProductIdsExistAsync(It.IsAny<List<Guid>>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);
            _setProductRepositoryMock.Setup(repo => repo.GetByProductIdsAndSetId(It.IsAny<List<Guid>>(), It.IsAny<Guid>()))
                .ReturnsAsync(new List<SetProduct> { new SetProduct() });

            _setRepositoryMock.Setup(repo => repo.GetByIdWithoutSetProductAsync(setId))
                .ReturnsAsync(new Set());

            // Act
            var result = await _updateSetCommandHandler.Handle(updateSetCommand, default);

            // Assert
            Assert.True(result.isSuccess);
            _setProductRepositoryMock.Verify(sp => sp.DeleteRange(It.IsAny<List<SetProduct>>()), Times.Never);
            _setProductRepositoryMock.Verify(sp => sp.UpdateRange(It.IsAny<List<SetProduct>>()), Times.Once);
            _setProductRepositoryMock.Verify(sp => sp.AddRange(It.IsAny<List<SetProduct>>()), Times.Once);
            _setRepositoryMock.Verify(p => p.Update(It.IsAny<Set>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task Handler_Success_ShouldAddAndRemoveSetProducts()
        {
            // Arrange
            var add = new List<SetProductRequest>
            {
                new SetProductRequest(Guid.NewGuid(), 5),
            };

            var update = new List<SetProductRequest>
            {
                new SetProductRequest(Guid.NewGuid(), 10),
            };

            var remove = new List<Guid> { Guid.NewGuid() };

            var setId = Guid.NewGuid();
            var updateSetRequest = new UpdateSetRequest(setId, "CD123", "Name",
                "Description", "Image", add, update, remove);
            var updateSetCommand = new UpdateSetCommand(updateSetRequest, "UpdatedBy", setId);

            _productRepositoryMock.Setup(repo => repo.IsAllSubProductIdsExist(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);
            _setProductRepositoryMock.Setup(repo => repo.IsAnyIdExistAsync(It.IsAny<List<Guid>>(), It.IsAny<Guid>()))
                .ReturnsAsync(false);
            _setProductRepositoryMock.Setup(repo => repo.DoProductIdsExistAsync(It.IsAny<List<Guid>>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);
            _setProductRepositoryMock.Setup(repo => repo.GetByProductIdsAndSetId(It.IsAny<List<Guid>>(), It.IsAny<Guid>()))
                .ReturnsAsync(new List<SetProduct> { new SetProduct() });

            _setRepositoryMock.Setup(repo => repo.GetByIdWithoutSetProductAsync(setId))
                .ReturnsAsync(new Set());
            _setRepositoryMock.Setup(repo => repo.IsCodeExistAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act
            var result = await _updateSetCommandHandler.Handle(updateSetCommand, default);

            // Assert
            Assert.True(result.isSuccess);
            _setProductRepositoryMock.Verify(sp => sp.DeleteRange(It.IsAny<List<SetProduct>>()), Times.Once);
            _setProductRepositoryMock.Verify(sp => sp.UpdateRange(It.IsAny<List<SetProduct>>()), Times.Once);
            _setProductRepositoryMock.Verify(sp => sp.AddRange(It.IsAny<List<SetProduct>>()), Times.Once);
            _setRepositoryMock.Verify(p => p.Update(It.IsAny<Set>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task Handler_Success_ShouldUpdateSetProducts()
        {
            // Arrange
            var update = new List<SetProductRequest>
            {
                new SetProductRequest(Guid.NewGuid(), 10),
            };

            var setId = Guid.NewGuid();
            var updateSetRequest = new UpdateSetRequest(setId, "CD123", "Name",
                "Description", "Image", null, update, null);
            var updateSetCommand = new UpdateSetCommand(updateSetRequest, "UpdatedBy", setId);

            _productRepositoryMock.Setup(repo => repo.IsAllSubProductIdsExist(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);
            _setProductRepositoryMock.Setup(repo => repo.IsAnyIdExistAsync(It.IsAny<List<Guid>>(), It.IsAny<Guid>()))
                .ReturnsAsync(false);
            _setProductRepositoryMock.Setup(repo => repo.DoProductIdsExistAsync(It.IsAny<List<Guid>>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);
            _setProductRepositoryMock.Setup(repo => repo.GetByProductIdsAndSetId(It.IsAny<List<Guid>>(), It.IsAny<Guid>()))
                .ReturnsAsync(new List<SetProduct> { new SetProduct() });

            _setRepositoryMock.Setup(repo => repo.GetByIdWithoutSetProductAsync(setId))
                .ReturnsAsync(new Set());
            _setRepositoryMock.Setup(repo => repo.IsCodeExistAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act
            var result = await _updateSetCommandHandler.Handle(updateSetCommand, default);

            // Assert
            Assert.True(result.isSuccess);
            _setProductRepositoryMock.Verify(sp => sp.DeleteRange(It.IsAny<List<SetProduct>>()), Times.Never);
            _setProductRepositoryMock.Verify(sp => sp.UpdateRange(It.IsAny<List<SetProduct>>()), Times.Once);
            _setProductRepositoryMock.Verify(sp => sp.AddRange(It.IsAny<List<SetProduct>>()), Times.Never);
            _setRepositoryMock.Verify(p => p.Update(It.IsAny<Set>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task Handler_Success_ShouldRemoveSetProducts()
        {
            // Arrange
            var remove = new List<Guid> { Guid.NewGuid() };

            var setId = Guid.NewGuid();
            var updateSetRequest = new UpdateSetRequest(setId, "CD123", "Name",
                "Description", "Image", null, null, remove);
            var updateSetCommand = new UpdateSetCommand(updateSetRequest, "UpdatedBy", setId);

            _productRepositoryMock.Setup(repo => repo.IsAllSubProductIdsExist(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);
            _setProductRepositoryMock.Setup(repo => repo.IsAnyIdExistAsync(It.IsAny<List<Guid>>(), It.IsAny<Guid>()))
                .ReturnsAsync(false);
            _setProductRepositoryMock.Setup(repo => repo.DoProductIdsExistAsync(It.IsAny<List<Guid>>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);
            _setProductRepositoryMock.Setup(repo => repo.GetByProductIdsAndSetId(It.IsAny<List<Guid>>(), It.IsAny<Guid>()))
                .ReturnsAsync(new List<SetProduct> { new SetProduct() });

            _setRepositoryMock.Setup(repo => repo.GetByIdWithoutSetProductAsync(setId))
                .ReturnsAsync(new Set());
            _setRepositoryMock.Setup(repo => repo.IsCodeExistAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act
            var result = await _updateSetCommandHandler.Handle(updateSetCommand, default);

            // Assert
            Assert.True(result.isSuccess);
            _setProductRepositoryMock.Verify(sp => sp.DeleteRange(It.IsAny<List<SetProduct>>()), Times.Once);
            _setProductRepositoryMock.Verify(sp => sp.UpdateRange(It.IsAny<List<SetProduct>>()), Times.Never);
            _setProductRepositoryMock.Verify(sp => sp.AddRange(It.IsAny<List<SetProduct>>()), Times.Never);
            _setRepositoryMock.Verify(p => p.Update(It.IsAny<Set>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task Handler_Failure_ShouldThrowSetNotFoundException_WhenSetDoesNotExist()
        {
            // Arrange
            var setId = Guid.NewGuid();
            var updateSetRequest = new UpdateSetRequest(setId, "CD123", "Name",
                "Description", "Image", null, null, null);
            var updateSetCommand = new UpdateSetCommand(updateSetRequest, "UpdatedBy", setId);

            _setRepositoryMock.Setup(repo => repo.GetByIdWithoutSetProductAsync(setId))
                .ReturnsAsync((Set)null);
            _setRepositoryMock.Setup(repo => repo.IsCodeExistAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<SetNotFoundException>(() => _updateSetCommandHandler.Handle(updateSetCommand, default));
        }

        [Fact]
        public async Task Handler_Failure_ShouldThrowSetProductNotFoundException_WhenProductsToRemoveDoNotExist()
        {
            // Arrange
            var remove = new List<Guid> { Guid.NewGuid() };
            var setId = Guid.NewGuid();
            var updateSetRequest = new UpdateSetRequest(setId, "CD123", "Name",
                "Description", "Image", null, null, remove);
            var updateSetCommand = new UpdateSetCommand(updateSetRequest, "UpdatedBy", setId);

            _setProductRepositoryMock.Setup(repo => repo.DoProductIdsExistAsync(It.IsAny<List<Guid>>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);
            _setRepositoryMock.Setup(repo => repo.GetByIdWithoutSetProductAsync(setId))
                .ReturnsAsync(new Set());

            _setProductRepositoryMock.Setup(repo => repo.GetByProductIdsAndSetId(remove, setId))
                .ReturnsAsync((List<SetProduct>)null);
            _setRepositoryMock.Setup(repo => repo.IsCodeExistAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<SetProductNotFoundException>(() => _updateSetCommandHandler.Handle(updateSetCommand, default));
        }

        [Fact]
        public async Task Handler_Failure_ShouldThrowSetProductNotFoundException_WhenProductsToUpdateDoNotExist()
        {
            // Arrange
            var update = new List<SetProductRequest>
            {
                new SetProductRequest(Guid.NewGuid(), 10),
            };

            var setId = Guid.NewGuid();
            var updateSetRequest = new UpdateSetRequest(setId, "CD123", "Name",
                "Description", "Image", null, update, null);
            var updateSetCommand = new UpdateSetCommand(updateSetRequest, "UpdatedBy", setId);

            _setProductRepositoryMock.Setup(repo => repo.DoProductIdsExistAsync(It.IsAny<List<Guid>>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);
            _setRepositoryMock.Setup(repo => repo.GetByIdWithoutSetProductAsync(setId))
                .ReturnsAsync(new Set());

            _setProductRepositoryMock.Setup(repo => repo.GetByProductIdsAndSetId(It.IsAny<List<Guid>>(), It.IsAny<Guid>()))
                .ReturnsAsync((List<SetProduct>)null);
            _setRepositoryMock.Setup(repo => repo.IsCodeExistAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<SetProductNotFoundException>(() => _updateSetCommandHandler.Handle(updateSetCommand, default));
        }

        [Fact]
        public async Task Handler_Failure_ShouldThrowSetIdConflictException_WhenSetIdConflicts()
        {
            // Arrange
            var setId = Guid.NewGuid();
            var updateSetRequest = new UpdateSetRequest(setId, "CD123", "Name",
                "Description", "Image", null, null, null);
            var conflictingSetId = Guid.NewGuid();
            var updateSetCommand = new UpdateSetCommand(updateSetRequest, "UpdatedBy", conflictingSetId);

            // Act & Assert
            await Assert.ThrowsAsync<SetIdConflictException>(() => _updateSetCommandHandler.Handle(updateSetCommand, default));
        }

        [Theory]
        [InlineData("CD123", "", "Description", "Image")] // Empty Name
        [InlineData("CD123", "Name", "", "Image")] // Empty Description
        [InlineData("CD123", "Name", "Description", "")] // Empty ImageUrl
        [InlineData("", "Name", "Description", "Image")] // Invalid Code format
        [InlineData("CDD1234", "Name", "Description", "Image")] // Invalid Code format
        [InlineData("CDDF", "Name", "Description", "Image")] // Invalid Code format
        [InlineData("3423", "Name", "Description", "Image")] // Invalid Code format
        [InlineData("", "", "", "")]
        public async Task Handler_Failure_ShouldThrowValidationException_WhenRequestIsInvalid(string code, string name, string description, string imageUrl)
        {
            // Arrange
            var setId = Guid.NewGuid();
            var updateSetRequest = new UpdateSetRequest(setId, code, name, description, imageUrl, null, null, null);
            var updateSetCommand = new UpdateSetCommand(updateSetRequest, "UpdatedBy", setId);

            // Act & Assert
            await Assert.ThrowsAsync<MyValidationException>(() => _updateSetCommandHandler.Handle(updateSetCommand, default));
        }

        [Fact]
        public async Task Handler_Failure_ShouldThrowValidationException_WhenAddProductIdNotFound()
        {
            // Arrange
            var add = new List<SetProductRequest>
            {
                new SetProductRequest(Guid.NewGuid(), 5),
            };

            var setId = Guid.NewGuid();
            var updateSetRequest = new UpdateSetRequest(setId, "CD123", "Name",
                "Description", "Image", add, null, null);
            var updateSetCommand = new UpdateSetCommand(updateSetRequest, "UpdatedBy", setId);

            _productRepositoryMock.Setup(repo => repo.IsAllSubProductIdsExist(It.IsAny<List<Guid>>()))
                .ReturnsAsync(false);
            _setRepositoryMock.Setup(repo => repo.IsCodeExistAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<MyValidationException>(() => _updateSetCommandHandler.Handle(updateSetCommand, default));
        }

        [Fact]
        public async Task Handler_Failure_ShouldThrowValidationException_WhenAddProductIdExistInSetProduct()
        {
            // Arrange
            var add = new List<SetProductRequest>
            {
                new SetProductRequest(Guid.NewGuid(), 5),
            };

            var setId = Guid.NewGuid();
            var updateSetRequest = new UpdateSetRequest(setId, "CD123", "Name",
                "Description", "Image", add, null, null);
            var updateSetCommand = new UpdateSetCommand(updateSetRequest, "UpdatedBy", setId);

            _productRepositoryMock.Setup(repo => repo.IsAllSubProductIdsExist(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);
            _setProductRepositoryMock.Setup(repo => repo.IsAnyIdExistAsync(It.IsAny<List<Guid>>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);
            _setRepositoryMock.Setup(repo => repo.IsCodeExistAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<MyValidationException>(() => _updateSetCommandHandler.Handle(updateSetCommand, default));
        }

        [Fact]
        public async Task Handler_Failure_ShouldThrowValidationException_WhenRemoveProductIdNotExistInSetProduct()
        {
            // Arrange
            var remove = new List<Guid>
            {
                Guid.NewGuid(),
            };

            var setId = Guid.NewGuid();
            var updateSetRequest = new UpdateSetRequest(setId, "CD123", "Name",
                "Description", "Image", null, null, remove);
            var updateSetCommand = new UpdateSetCommand(updateSetRequest, "UpdatedBy", setId);

            _setProductRepositoryMock.Setup(repo => repo.DoProductIdsExistAsync(It.IsAny<List<Guid>>(), It.IsAny<Guid>()))
                .ReturnsAsync(false);
            _setRepositoryMock.Setup(repo => repo.IsCodeExistAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<MyValidationException>(() => _updateSetCommandHandler.Handle(updateSetCommand, default));
        }

        [Fact]
        public async Task Handler_Failure_ShouldThrowValidationException_WhenUpdateProductIdNotExistInSetProduct()
        {
            // Arrange
            var update = new List<SetProductRequest>
            {
                new SetProductRequest(Guid.NewGuid(), 5),
            };

            var setId = Guid.NewGuid();
            var updateSetRequest = new UpdateSetRequest(setId, "CD123", "Name",
                "Description", "Image", null, update, null);
            var updateSetCommand = new UpdateSetCommand(updateSetRequest, "UpdatedBy", setId);

            _setProductRepositoryMock.Setup(repo => repo.DoProductIdsExistAsync(It.IsAny<List<Guid>>(), It.IsAny<Guid>()))
                .ReturnsAsync(false);
            _setRepositoryMock.Setup(repo => repo.IsCodeExistAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<MyValidationException>(() => _updateSetCommandHandler.Handle(updateSetCommand, default));
        }
    }
}
