using Application.Abstractions.Data;
using Application.UserCases.Queries.Phases;
using AutoMapper;
using Contract.Services.Phase.Queries;
using Contract.Services.Phase.ShareDto;
using Moq;

namespace Application.UnitTests.Phases.Queries;

public class GetPhasesQueryHandlerTests
{
    private readonly Mock<IPhaseRepository> _phaseRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    public GetPhasesQueryHandlerTests()
    {
        _phaseRepositoryMock = new();
        _mapperMock = new();
    }
    // handler should return success
    [Fact]
    public async Task Handler_ShouldReturnSuccess_WhenReceivedPhasesIsNotNull()
    {
        var getPhasesQuery = new GetPhasesQuery();
        var getPhasesQueryHandler = new GetPhasesQueryHandler(_phaseRepositoryMock.Object, _mapperMock.Object);

        _phaseRepositoryMock.Setup(repo => repo.GetPhases()).ReturnsAsync(new List<Domain.Entities.Phase> { new Domain.Entities.Phase() });
        _mapperMock.Setup(mapper => mapper.Map<PhaseResponse>(It.IsAny<Domain.Entities.Phase>())).Returns(It.IsAny<PhaseResponse>);
        var result = await getPhasesQueryHandler.Handle(getPhasesQuery, default);

        Assert.NotNull(result);
    }
}
