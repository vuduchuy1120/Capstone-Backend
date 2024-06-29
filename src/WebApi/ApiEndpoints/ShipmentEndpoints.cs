using Application.Utils;
using Carter;
using Contract.Services.Shipment.Create;
using Contract.Services.Shipment.GetShipmentDetail;
using Contract.Services.Shipment.GetShipments;
using Contract.Services.Shipment.UpdateStatus;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

namespace WebApi.ApiEndpoints;

public class ShipmentEndpoints : CarterModule
{
    public ShipmentEndpoints() : base("api/shipments")
    {
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(string.Empty, async (
            ISender sender, 
            ClaimsPrincipal claim,
            [FromBody] CreateShipmentRequest createShipmentRequest) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
            var createShipmentCommand = new CreateShipmentCommand(createShipmentRequest, userId);

            var result = await sender.Send(createShipmentCommand);

            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Shipment api" } }
        });

        app.MapPut("{id}", async (
            ISender sender, 
            ClaimsPrincipal claim, 
            [FromRoute] Guid id,
            [FromBody] UpdateStatusRequest request) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
            var updateStatusCommand = new UpdateShipmentStatusCommand(id, request, userId); 

            var result = await sender.Send(updateStatusCommand);

            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Shipment api" } }
        });

        app.MapGet("{id}", async (ISender sender, [FromRoute] Guid id) =>
        {
            var result = await sender.Send(new GetShipmentDetailQuery(id));

            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Shipment api" } }
        });

        app.MapGet(string.Empty, async (ISender sender, [AsParameters] GetShipmentsQuery request) =>
        {
            var result = await sender.Send(request);

            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Shipment api" } }
        });

        app.MapPatch("{id}/status", async (
            ISender sender,
            ClaimsPrincipal claim,
            [FromRoute] Guid id,
            [FromBody] UpdateStatusRequest request) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
            var result = await sender.Send(new UpdateShipmentStatusCommand(id, request, userId));

            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Shipment api" } }
        });
    }
}
