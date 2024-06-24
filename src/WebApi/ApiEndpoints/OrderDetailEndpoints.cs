using Carter;
using Contract.Services.OrderDetail.Creates;
using Contract.Services.OrderDetail.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace WebApi.ApiEndpoints
{
    public class OrderDetailEndpoints : CarterModule
    {
        public OrderDetailEndpoints() : base("api/orderDetails")
        {
        }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost(string.Empty, async (
                            ISender sender,
                            [FromBody] CreateListOrderDetailsRequest orderDetailRequest) =>
            {
                var createOrderDetailCommand = new CreateOrderDetailsCommand(orderDetailRequest);
                var result = await sender.Send(createOrderDetailCommand);

                return Results.Ok(result);
            }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
            {
                Tags = new List<OpenApiTag> { new() { Name = "Order Detail api" } }
            });

            app.MapGet(string.Empty, async (
                            ISender sender,
                            [AsParameters] GetOrderDetailsByOrderIdQuery request) =>
            {
                var result = await sender.Send(request);

                return Results.Ok(result);
            }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
            {
                Tags = new List<OpenApiTag> { new() { Name = "Order Detail api" } }
            });
        }
    }
}
