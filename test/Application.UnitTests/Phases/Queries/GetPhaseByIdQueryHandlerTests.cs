using Application.Abstractions.Data;
using Application.UserCases.Queries.Phases;
using AutoMapper;
using Contract.Services.Phase.Queries;
using Contract.Services.Phase.ShareDto;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UnitTests.Phases.Queries;

public class GetPhaseByIdQueryHandlerTests
{
    private readonly Mock<IPhaseRepository> _phaseRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    public GetPhaseByIdQueryHandlerTests()
    {
        _phaseRepositoryMock = new();
        _mapperMock = new();
    }
    // handler should return success
    [Fact]
    public async Task Handler_ShouldReturnSuccess_WhenReceivedPhaseIsNotNull()
    {
        var getPhaseByIdQuery = new GetPhaseByIdQuery(new Guid());
        var getPhaseByIdQueryHandler = new GetPhaseByIdQueryHandler(_phaseRepositoryMock.Object, _mapperMock.Object);

        _phaseRepositoryMock.Setup(repo => repo.GetPhaseById(It.IsAny<Guid>())).ReturnsAsync(new Domain.Entities.Phase());
        _mapperMock.Setup(mapper => mapper.Map<PhaseResponse>(It.IsAny<Domain.Entities.Phase>())).Returns(It.IsAny<PhaseResponse>);
        var result = await getPhaseByIdQueryHandler.Handle(getPhaseByIdQuery, default);

        Assert.NotNull(result);
    }
}
