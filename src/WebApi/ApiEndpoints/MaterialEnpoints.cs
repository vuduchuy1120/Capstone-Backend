using Carter;
using Contract.Services.Material.Create;
using Contract.Services.Material.Get;
using Contract.Services.Material.Query;
using Contract.Services.Material.Update;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

namespace WebApi.ApiEndpoints;

public class MaterialEndpoints : CarterModule
{
    public MaterialEndpoints() : base("/api/material")
    {
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(string.Empty, async (
            ISender sender,
            ClaimsPrincipal claim,
            [FromBody] CreateMaterialRequest materialRequest) =>
        {
            var createMaterialCommand = new CreateMaterialCommand(materialRequest);
            var result = await sender.Send(createMaterialCommand);

            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Material api" } }
        });

        app.MapPut(string.Empty, async (
            ISender sender,
            ClaimsPrincipal claim,
            [FromBody] UpdateMaterialRequest updateMaterialRequest) =>
        {
            var updateMaterialCommand = new UpdateMaterialCommand(updateMaterialRequest);
            var result = await sender.Send(updateMaterialCommand);

            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Material api" } }
        });

        app.MapGet("{id}", async (
            ISender sender,
            ClaimsPrincipal claim,
            [FromRoute] Guid id) =>
        {
            var getMaterialByIdQuery = new GetMaterialByIdQuery(id);
            var result = await sender.Send(getMaterialByIdQuery);

            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Material api" } }
        });

        //app.MapGet("units", async (
        //    ISender sender,
        //    ClaimsPrincipal claim) =>
        //{
        //    var getMaterialUnitsQuery = new GetMaterialUnitsQuery();
        //    var result = await sender.Send(getMaterialUnitsQuery);

        //    return Results.Ok(result);
        //}).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        //{
        //    Tags = new List<OpenApiTag> { new() { Name = "Material api" } }
        //});

        app.MapGet(string.Empty, async (
            ISender sender,
            ClaimsPrincipal claim,
            [AsParameters] GetMaterialsQuery getMaterialsQuery) =>
        {
            var result = await sender.Send(getMaterialsQuery);
            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Material api" } }
        });
    }
}
