using Application.Abstractions.Data;
using Application.UserCases.Queries.Sets.GetSet;
using AutoMapper;
using Contract.Services.Set.GetSet;
using Domain.Entities;
using Domain.Exceptions.Sets;
using Moq;

namespace Application.UnitTests.Sets.Query;

public class GetSetQueryHandlerTest
{
    private readonly Mock<ISetRepository> _setRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetSetQueryHandler _getSetQueryHandler;

    public GetSetQueryHandlerTest()
    {
        _setRepositoryMock = new Mock<ISetRepository>();
        _mapperMock = new Mock<IMapper>();
        _getSetQueryHandler = new GetSetQueryHandler(_setRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSetResponse_WhenSetExists()
    {
        // Arrange
        var setId = Guid.NewGuid();
        var query = new GetSetQuery(setId);

        var set = new Set { Id = setId };

        _setRepositoryMock.Setup(repo => repo.GetByIdAsync(setId))
            .ReturnsAsync(set);
        _mapperMock.Setup(mapper => mapper.Map<SetResponse>(set))
            .Returns(It.IsAny<SetResponse>());

        // Act
        var result = await _getSetQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.isSuccess);

        _setRepositoryMock.Verify(repo => repo.GetByIdAsync(setId), Times.Once);
        _mapperMock.Verify(mapper => mapper.Map<SetResponse>(set), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowSetNotFoundException_WhenSetDoesNotExist()
    {
        // Arrange
        var setId = Guid.NewGuid();
        var query = new GetSetQuery(setId);

        _setRepositoryMock.Setup(repo => repo.GetByIdAsync(setId))
            .ReturnsAsync((Set)null);

        // Act & Assert
        await Assert.ThrowsAsync<SetNotFoundException>(() => _getSetQueryHandler.Handle(query, CancellationToken.None));

        _setRepositoryMock.Verify(repo => repo.GetByIdAsync(setId), Times.Once);
        _mapperMock.Verify(mapper => mapper.Map<SetResponse>(It.IsAny<Set>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnResultSuccessWithNullValue_WhenSetIsNull()
    {
        // Arrange
        var setId = Guid.NewGuid();
        var query = new GetSetQuery(setId);

        _setRepositoryMock.Setup(repo => repo.GetByIdAsync(setId))
            .ReturnsAsync((Set)null);

        // Act
        var exception = await Assert.ThrowsAsync<SetNotFoundException>(() => _getSetQueryHandler.Handle(query, CancellationToken.None));

        // Assert
        Assert.NotNull(exception);

        _setRepositoryMock.Verify(repo => repo.GetByIdAsync(setId), Times.Once);
        _mapperMock.Verify(mapper => mapper.Map<SetResponse>(It.IsAny<Set>()), Times.Never);
    }
}
