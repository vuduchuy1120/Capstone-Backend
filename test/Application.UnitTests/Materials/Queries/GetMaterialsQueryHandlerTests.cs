using Application.Abstractions.Data;
using Application.UserCases.Queries.Materials;
using AutoMapper;
using Contract.Services.Material.Get;
using Contract.Services.Material.ShareDto;
using Domain.Exceptions.Materials;
using Moq;

namespace Application.UnitTests.Materials.Queries;

public class GetMaterialsQueryHandlerTests
{
    private readonly Mock<IMaterialRepository> _materialRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    public GetMaterialsQueryHandlerTests()
    {
        _materialRepositoryMock = new();
        _mapperMock = new();
    }
    // handler should return success
    [Fact]
    public async Task Handler_ShouldReturnSuccess_WhenReceivedMaterialsIsNotNull()
    {
        var getMaterialsQuery = new GetMaterialsQuery("");
        var getMaterialsQueryHandler = new GetMaterialsQueryHandler(_materialRepositoryMock.Object, _mapperMock.Object);

        _materialRepositoryMock.Setup(repo => repo.SearchMaterialsAsync(getMaterialsQuery)).ReturnsAsync((new List<Domain.Entities.Material>() { new Domain.Entities.Material() }, 1));
        _mapperMock.Setup(mapper => mapper.Map<MaterialResponse>(It.IsAny<Domain.Entities.Material>())).Returns(It.IsAny<MaterialResponse>);
        var result = await getMaterialsQueryHandler.Handle(getMaterialsQuery, default);

        Assert.NotNull(result);
    }

    // handler should throw MaterialNotFoundException when received materials is null
    [Fact]
    public async Task Handler_ShouldThrow_MaterialNotFoundException_WhenReceivedMaterialsIsNull()
    {
        var getMaterialsQuery = new GetMaterialsQuery("");
        var getMaterialsQueryHandler = new GetMaterialsQueryHandler(_materialRepositoryMock.Object, _mapperMock.Object);

        _materialRepositoryMock.Setup(repo => repo.SearchMaterialsAsync(getMaterialsQuery)).ReturnsAsync(((List<Domain.Entities.Material>)null, 0));

        await Assert.ThrowsAsync<MaterialNotFoundException>(async () =>
        {
            await getMaterialsQueryHandler.Handle(getMaterialsQuery, default);
        });
    }
    [Fact]
    public async Task Handler_ShouldThrow_MaterialNotFoundException_WhenReceivedMaterialsCountEqual0()
    {
        var getMaterialsQuery = new GetMaterialsQuery("");
        var getMaterialsQueryHandler = new GetMaterialsQueryHandler(_materialRepositoryMock.Object, _mapperMock.Object);

        _materialRepositoryMock.Setup(repo => repo.SearchMaterialsAsync(getMaterialsQuery)).ReturnsAsync((new List<Domain.Entities.Material>(), 0));

        await Assert.ThrowsAsync<MaterialNotFoundException>(async () =>
        {
            await getMaterialsQueryHandler.Handle(getMaterialsQuery, default);
        });
    }

}
