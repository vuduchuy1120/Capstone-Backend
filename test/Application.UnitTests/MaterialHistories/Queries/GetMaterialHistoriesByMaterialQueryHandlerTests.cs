using Application.Abstractions.Data;
using Application.UserCases.Queries.MaterialHistories;
using AutoMapper;
using Contract.Services.Material.Get;
using Contract.Services.MaterialHistory.Queries;
using Contract.Services.MaterialHistory.ShareDto;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UnitTests.MaterialHistories.Queries;

public class GetMaterialHistoriesByMaterialQueryHandlerTests
{
    private readonly Mock<IMaterialHistoryRepository> _materialHistoryRepositoryMock;
    private readonly Mock<IMaterialRepository> _materialRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;

    public GetMaterialHistoriesByMaterialQueryHandlerTests()
    {
        _materialHistoryRepositoryMock = new();
        _materialRepositoryMock = new();
        _mapperMock = new();
    }
    [Fact]
    public async Task Handler_ShouldReturnSuccess_WhenReceivedMaterialHistoriesIsNotNull()
    {
        var getMaterialHistoriesByMaterialQuery = new GetMaterialHistoriesByMaterialQuery("", "");
        var getMaterialHistoriesByMaterialQueryHandler = new GetMaterialHistoriesByMaterialQueryHandler(_materialHistoryRepositoryMock.Object, _mapperMock.Object);

        _materialHistoryRepositoryMock.Setup(repo => repo.GetMaterialHistoriesByMaterialNameAndDateAsync(getMaterialHistoriesByMaterialQuery)).ReturnsAsync((new List<Domain.Entities.MaterialHistory>() { new Domain.Entities.MaterialHistory() }, 1));
        _mapperMock.Setup(mapper => mapper.Map<MaterialHistoryResponse>(It.IsAny<Domain.Entities.MaterialHistory>())).Returns(It.IsAny<MaterialHistoryResponse>);
        var result = await getMaterialHistoriesByMaterialQueryHandler.Handle(getMaterialHistoriesByMaterialQuery, default);
        Assert.NotNull(result);
    }
}
