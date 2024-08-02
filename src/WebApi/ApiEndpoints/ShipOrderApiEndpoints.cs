using Application.Utils;
using Carter;
using Contract.Services.Shipment.GetShipmentDetail;
using Contract.Services.ShipOrder.ChangeStatus;
using Contract.Services.ShipOrder.Create;
using Contract.Services.ShipOrder.GetShipOrderByOrderId;
using Contract.Services.ShipOrder.GetShipOrderDetail;
using Contract.Services.ShipOrder.GetShipOrdersOfShipper;
using Contract.Services.ShipOrder.Update;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

namespace WebApi.ApiEndpoints;

public class ShipOrderApiEndpoints : CarterModule
{
    public ShipOrderApiEndpoints() : base("api/ship-orders")
    {
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(string.Empty, async (ISender sender, ClaimsPrincipal claim, [FromBody] CreateShipOrderRequest createShipOrderRequest) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
            var createShipOrderCommand = new CreateShipOrderCommand(userId, createShipOrderRequest);
            var result = await sender.Send(createShipOrderCommand);

            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Ship order api" } }
        });

        app.MapGet("{id}", async (ISender sender, [FromRoute] Guid id) =>
        {
            var result = await sender.Send(new GetShipOrderByOrderIdQuery(id));

            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Ship order api" } }
        });

        app.MapGet("by-shipper", async (ISender sender, [AsParameters] SearchShipOrderOption searchOption, ClaimsPrincipal claim) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
            var result = await sender.Send(new GetShipOrdersByShipperIdQuery(userId, searchOption));

            return Results.Ok(result);
        }).RequireAuthorization("Require-Driver").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Ship order api" } }
        });

        app.MapGet("detail/{shipOrderId}", async (ISender sender, [FromRoute] Guid shipOrderId) =>
        {
            var result = await sender.Send(new GetShipOrderDetailQuery(shipOrderId));

            return Results.Ok(result);
        }).RequireAuthorization("Require-Driver-MainAdmin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Ship order api" } }
        });

        app.MapPut("{id}", async (ISender sender, ClaimsPrincipal claim, [FromBody] UpdateShipOrderRequest request, [FromRoute] Guid id) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
            var updateShipOrderCommand = new UpdateShipOderCommand(userId, id, request);

            var result = await sender.Send(updateShipOrderCommand);

            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Ship order api" } }
        });

        app.MapPatch("{id}", async (
            ISender sender, 
            ClaimsPrincipal claim, 
            [FromRoute] Guid id, 
            [FromBody] ChangeShipOrderStatusRequest request) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
            var changeShipOrderStatusCommand = new ChangeShipOrderStatusCommand(userId, id, request);

            var result = await sender.Send(changeShipOrderStatusCommand);

            return Results.Ok(result);
        }).RequireAuthorization("Require-Driver-MainAdmin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Ship order api" } }
        });
    }
}
