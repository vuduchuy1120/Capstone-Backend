using Application.Abstractions.Data;
using Application.UserCases.Queries.Materials;
using AutoMapper;
using Contract.Services.Material.Query;
using Contract.Services.Material.ShareDto;
using Domain.Exceptions.Materials;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UnitTests.Materials.Queries;

public class GetMaterialByIdQueryHandlerTests
{
    private readonly Mock<IMaterialRepository> _materialRepositoryMock;
    private readonly Mock<IMapper> _mock;

    public GetMaterialByIdQueryHandlerTests()
    {
        _materialRepositoryMock = new();
        _mock = new();
    }

    [Fact]
    public async Task Handler_ShouldReturnSuccess_WhenReceivedMaterialIsNotNull()
    {
        // Arrange
        var getMaterialByIdQuery = new GetMaterialByIdQuery(1);
        var getMaterialByIdQueryHandler = new GetMaterialByIdQueryHandler(_materialRepositoryMock.Object, _mock.Object);

        _materialRepositoryMock.Setup(repo => repo.GetMaterialByIdAsync(getMaterialByIdQuery.Id)).ReturnsAsync(new Domain.Entities.Material());
        _mock.Setup(mapper => mapper.Map<MaterialResponse>(It.IsAny<Domain.Entities.Material>())).Returns(It.IsAny<MaterialResponse>);

        // Act
        var result = await getMaterialByIdQueryHandler.Handle(getMaterialByIdQuery, default);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handler_ShouldThrow_MaterialNotFoundException_WhenReceivedMaterialIsNull()
    {
        // Arrange
        var getMaterialByIdQuery = new GetMaterialByIdQuery(1);
        var getMaterialByIdQueryHandler = new GetMaterialByIdQueryHandler(_materialRepositoryMock.Object, _mock.Object);

        _materialRepositoryMock.Setup(repo => repo.GetMaterialByIdAsync(getMaterialByIdQuery.Id)).ReturnsAsync((Domain.Entities.Material)null);

        // Act & Assert
        await Assert.ThrowsAsync<MaterialNotFoundException>(async () =>
        {
            await getMaterialByIdQueryHandler.Handle(getMaterialByIdQuery, default);
        });
    }
}
