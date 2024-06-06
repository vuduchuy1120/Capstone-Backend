using Application.Abstractions.Data;
using Application.UserCases.Queries.Materials;
using Contract.Services.Material.Query;
using Domain.Exceptions.Materials;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UnitTests.Materials.Queries;

public class GetMaterialUnitsQueryHandlerTests
{
    private readonly Mock<IMaterialRepository> _materialRepositoryMock;
    public GetMaterialUnitsQueryHandlerTests()
    {
        _materialRepositoryMock = new();
    }
    [Fact]
    public async Task Handler_ShouldReturnSuccess_WhenReceivedMaterialUnitsIsNotNull()
    {
        var getMaterialUnitsQuery = new GetMaterialUnitsQuery();
        var getMaterialUnitsQueryHandler = new GetMaterialUnitsQueryHandler(_materialRepositoryMock.Object);

        _materialRepositoryMock.Setup(repo => repo.GetMaterialUnitsAsync()).ReturnsAsync(new List<string> { "Unit 1", "Unit 2" });

        var result = await getMaterialUnitsQueryHandler.Handle(getMaterialUnitsQuery, default);

        Assert.NotNull(result);
    }
}
