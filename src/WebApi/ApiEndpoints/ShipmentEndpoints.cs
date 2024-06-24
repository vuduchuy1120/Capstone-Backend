using Application.Utils;
using Carter;
using Contract.Services.Shipment.Create;
using Contract.Services.Shipment.UpdateReturnQuantity;
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
            [FromBody] UpdateReturnQuantityRequest updateReturnQuantityRequest) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
            var updateReturnQuantityCommand = new UpdateShipmentReturnQuantityCommand(
                userId, 
                id,
                updateReturnQuantityRequest);

            var result = await sender.Send(updateReturnQuantityCommand);

            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Shipment api" } }
        });
    }
}
