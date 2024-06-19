using Carter;
using Contract.Services.Material.Get;
using Contract.Services.MaterialHistory.Create;
using Contract.Services.MaterialHistory.Queries;
using Contract.Services.MaterialHistory.Update;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

namespace WebApi.ApiEndpoints;

public class MaterialHistoryEndpoint : CarterModule
{
    public MaterialHistoryEndpoint() : base("/api/material-history")
    {
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        //add material history endpoint
        app.MapPost(string.Empty, async (
                       ISender sender,
                       [FromBody] CreateMaterialHistoryRequest materialHistoryRequest) =>
        {
            var createMaterialHistoryCommand = new CreateMaterialHistoryCommand(materialHistoryRequest);
            var result = await sender.Send(createMaterialHistoryCommand);

            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Material History api" } }
        });

        // update
        app.MapPut(string.Empty, async (
                       ISender sender,
                       [FromBody] UpdateMaterialHistoryRequest updateMaterialHistoryRequest) =>
        {
            var updateMaterialHistoryCommand = new UpdateMaterialHistoryCommand(updateMaterialHistoryRequest);
            var result = await sender.Send(updateMaterialHistoryCommand);

            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Material History api" } }
        });


        app.MapGet(string.Empty, async (
            ISender sender,
            [AsParameters] GetMaterialHistoriesByMaterialQuery getMaterialHistoriesQuery) =>
        {
            var result = await sender.Send(getMaterialHistoriesQuery);
            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Material History api" } }
        });

        app.MapGet("{id:guid}", async (
                       ISender sender,
                       Guid id) =>
        {
            var getMaterialHistoryByIdQuery = new GetMaterialHistoryByIdQuery(id);
            var result = await sender.Send(getMaterialHistoryByIdQuery);
            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Material History api" } }
        });


    }
}
