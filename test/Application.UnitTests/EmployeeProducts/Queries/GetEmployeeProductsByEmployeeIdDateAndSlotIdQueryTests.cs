using Application.Abstractions.Data;
using Application.UserCases.Queries.EmployeeProducts;
using AutoMapper;
using Contract.Services.EmployeeProduct.Queries;
using Contract.Services.EmployeeProduct.ShareDto;
using Domain.Abstractions.Exceptions;
using Moq;

namespace Application.UnitTests.EmployeeProducts.Queries;

public class GetEmployeeProductsByEmployeeIdDateAndSlotIdQueryTests
{
    private readonly Mock<IEmployeeProductRepository> _employeeProductRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    public GetEmployeeProductsByEmployeeIdDateAndSlotIdQueryTests()
    {
        _employeeProductRepositoryMock = new();
        _mapperMock = new();
    }
    [Fact]
    public async Task Handler_ShouldReturnSuccess_WhenReceivedEmployeeProductsIsNotNull()
    {
        var getEmployeeProductsByEmployeeIdDateAndSlotIdQuery = 
            new GetEmployeeProductsByEmployeeIdDateAndSlotIdQuery(
                new GetEmployeeProductsByEmployeeIdDateAndSlotIdRequest(
                    1, 
                    "user1", 
                    "01/02/2002", 
                    Guid.Parse("b9fb1c8d-b84d-42db-8f5f-cb8583de4286")),
                "MAIN_ADMIN", 
                "user1", 
                Guid.Parse("b9fb1c8d-b84d-42db-8f5f-cb8583de4286")
                    );

        var getEmployeeProductsByEmployeeIdDateAndSlotIdQueryHandler = 
            new GetEmployeeProductsByEmployeeIdDateAndSlotIdQueryHandler(
                _employeeProductRepositoryMock.Object, _mapperMock.Object);

        _employeeProductRepositoryMock.Setup(repo => repo.GetEmployeeProductsByEmployeeIdDateAndSlotId(
            It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateOnly>(),It.IsAny<Guid>()))
            .ReturnsAsync(new List<Domain.Entities.EmployeeProduct> { new Domain.Entities.EmployeeProduct() });
        _mapperMock.Setup(mapper => mapper
                                    .Map<EmployeeProductResponse>(It.IsAny<Domain.Entities.EmployeeProduct>()))
                                    .Returns(It.IsAny<EmployeeProductResponse>);
        var result = await getEmployeeProductsByEmployeeIdDateAndSlotIdQueryHandler
            .Handle(getEmployeeProductsByEmployeeIdDateAndSlotIdQuery, default);

        Assert.NotNull(result);
    }


    public async Task Handler_ShouldThrow_MyValidationException_WhenInvalidDate()
    {
        var getEmployeeProductsByEmployeeIdDateAndSlotIdQuery =
           new GetEmployeeProductsByEmployeeIdDateAndSlotIdQuery(
                new GetEmployeeProductsByEmployeeIdDateAndSlotIdRequest(
                    1,
                    "user1",
                    "01/02/2002",
                    Guid.Parse("b9fb1c8d-b84d-42db-8f5f-cb8583de4286")),
                "MAIN_ADMIN",
                "user1",
                Guid.Parse("b9fb1c8d-b84d-42db-8f5f-cb8583de4286")
                    );
        var getEmployeeProductsByEmployeeIdDateAndSlotIdQueryHandler =
            new GetEmployeeProductsByEmployeeIdDateAndSlotIdQueryHandler(
                _employeeProductRepositoryMock.Object, _mapperMock.Object);

        _employeeProductRepositoryMock
            .Setup(repo => repo.GetEmployeeProductsByEmployeeIdDateAndSlotId(
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateOnly>(), It.IsAny<Guid>()))
            .ReturnsAsync((List<Domain.Entities.EmployeeProduct>)null);

        await Assert.ThrowsAsync<MyValidationException>(async () =>
        {
            await getEmployeeProductsByEmployeeIdDateAndSlotIdQueryHandler
            .Handle(getEmployeeProductsByEmployeeIdDateAndSlotIdQuery, default);
        });
    }
}
