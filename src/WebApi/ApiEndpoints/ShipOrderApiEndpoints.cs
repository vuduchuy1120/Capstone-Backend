﻿using Application.Utils;
using Carter;
using Contract.Services.Shipment.GetShipmentDetail;
using Contract.Services.ShipOrder.Create;
using Contract.Services.ShipOrder.GetShipOrderByOrderId;
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
    }
}