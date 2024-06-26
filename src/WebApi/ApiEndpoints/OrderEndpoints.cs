using Application.Utils;
using Carter;
using Contract.Services.Order.Creates;
using Contract.Services.Order.Queries;
using Contract.Services.Order.Updates;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

namespace WebApi.ApiEndpoints;

public class OrderEndpoints : CarterModule
{
    public OrderEndpoints() : base("api/orders")
    { }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(string.Empty, async (
            ISender _sender,
            ClaimsPrincipal _claimsPrincipal,
            [FromBody] CreateOrderRequest request) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(_claimsPrincipal);
            var command = new CreateOrderCommand(request, userId);
            var result = await _sender.Send(command);
            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Order api" } }
        });

        app.MapPut(string.Empty, async (
                       ISender _sender,
                        ClaimsPrincipal _claimsPrincipal,
                        [FromBody] UpdateOrderRequest request) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(_claimsPrincipal);
            var command = new UpdateOrderCommand(request, userId);
            var result = await _sender.Send(command);
            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Order api" } }
        });

        app.MapGet("{id}", async (
                       ISender _sender,
                        Guid id) =>
        {
            var query = new GetOrderByIdQuery(id);
            var result = await _sender.Send(query);
            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Order api" } }
        });

        app.MapGet(string.Empty, async (
                       ISender _sender,
                       [AsParameters] SearchOrderQuery request) =>
        {
            var result = await _sender.Send(request);
            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Order api" } }
        });
    }
}
