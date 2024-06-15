using Application.Abstractions.Data;
using Application.UserCases.Queries.Sets.GetSets;
using AutoMapper;
using Contract.Services.Set.GetSets;
using Domain.Entities;
using Domain.Exceptions.Sets;
using Moq;

namespace Application.UnitTests.Sets.Query;

public class GetSetsQueryHandlerTest
{
    private readonly Mock<ISetRepository> _setRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetSetsQueryHandler _getSetsQueryHandler;

    public GetSetsQueryHandlerTest()
    {
        _setRepositoryMock = new Mock<ISetRepository>();
        _mapperMock = new Mock<IMapper>();
        _getSetsQueryHandler = new GetSetsQueryHandler(_setRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSetsResponse_WhenSetsExist()
    {
        // Arrange
        var query = new GetSetsQuery("", 1, 10);
        var sets = new List<Set> { new Set { Id = Guid.NewGuid() } };
        var setsResponse = new List<SetsResponse> { new SetsResponse(Guid.NewGuid(), "Code", "Name", "ImageUrl", "Description") };
        var totalPages = 2;

        _setRepositoryMock.Setup(repo => repo.SearchSetAsync(query))
            .ReturnsAsync((sets, totalPages));
        _mapperMock.Setup(mapper => mapper.Map<List<SetsResponse>>(sets))
            .Returns(setsResponse);

        // Act
        var result = await _getSetsQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.isSuccess);
        Assert.Equal(setsResponse, result.data.Data);
        Assert.Equal(1, result.data.CurrentPage);
        Assert.Equal(totalPages, result.data.TotalPages);

        _setRepositoryMock.Verify(repo => repo.SearchSetAsync(query), Times.Once);
        _mapperMock.Verify(mapper => mapper.Map<List<SetsResponse>>(sets), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowSetNotFoundException_WhenSetsAreNull()
    {
        // Arrange
        var query = new GetSetsQuery("", 1, 10);
        var totalPages = 0;

        _setRepositoryMock.Setup(repo => repo.SearchSetAsync(query))
            .ReturnsAsync((null, totalPages));

        // Act & Assert
        await Assert.ThrowsAsync<SetNotFoundException>(() => _getSetsQueryHandler.Handle(query, CancellationToken.None));

        _setRepositoryMock.Verify(repo => repo.SearchSetAsync(query), Times.Once);
        _mapperMock.Verify(mapper => mapper.Map<List<SetsResponse>>(It.IsAny<List<Set>>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowSetNotFoundException_WhenSetsAreEmpty()
    {
        // Arrange
        var query = new GetSetsQuery("", 1, 10);
        var sets = new List<Set>();
        var totalPages = 0;

        _setRepositoryMock.Setup(repo => repo.SearchSetAsync(query))
            .ReturnsAsync((sets, totalPages));

        // Act & Assert
        await Assert.ThrowsAsync<SetNotFoundException>(() => _getSetsQueryHandler.Handle(query, CancellationToken.None));

        _setRepositoryMock.Verify(repo => repo.SearchSetAsync(query), Times.Once);
        _mapperMock.Verify(mapper => mapper.Map<List<SetsResponse>>(sets), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowSetNotFoundException_WhenTotalPagesIsZero()
    {
        // Arrange
        var query = new GetSetsQuery("", 1, 10);
        var sets = new List<Set> { new Set { Id = Guid.NewGuid() } };
        var totalPages = 0;

        _setRepositoryMock.Setup(repo => repo.SearchSetAsync(query))
            .ReturnsAsync((sets, totalPages));

        // Act & Assert
        await Assert.ThrowsAsync<SetNotFoundException>(() => _getSetsQueryHandler.Handle(query, CancellationToken.None));

        _setRepositoryMock.Verify(repo => repo.SearchSetAsync(query), Times.Once);
        _mapperMock.Verify(mapper => mapper.Map<List<SetsResponse>>(sets), Times.Never);
    }
}
