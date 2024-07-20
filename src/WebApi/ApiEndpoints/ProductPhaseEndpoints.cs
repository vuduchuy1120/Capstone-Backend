using Carter;
using Contract.ProductPhases.Updates.ChangeQuantityStatus;
using Contract.Services.ProductPhase.Creates;
using Contract.Services.ProductPhase.Queries;
using Contract.Services.ProductPhase.Updates;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace WebApi.ApiEndpoints;

public class ProductPhaseEndpoints : CarterModule
{
    public ProductPhaseEndpoints() : base("/api/productphase")
    {
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        //app.MapPost(string.Empty, async (
        //        ISender sender,
        //        CreateProductPhaseRequest request) =>
        //{
        //    var createProductPhaseCommand = new CreateProductPhaseCommand(request);
        //    var result = await sender.Send(createProductPhaseCommand);
        //    return Results.Ok(result);
        //}).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        //{
        //    Tags = new List<OpenApiTag> { new() { Name = "Product Phase api" } }
        //});

        //app.MapPut(string.Empty, async (
        //                   ISender sender,
        //                   UpdateProductPhaseRequest updateProductPhaseRequest) =>
        //{
        //    var updateProductPhaseCommand = new UpdateProductPhaseCommand(updateProductPhaseRequest);
        //    var result = await sender.Send(updateProductPhaseCommand);
        //    return Results.Ok(result);
        //}).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        //{
        //    Tags = new List<OpenApiTag> { new() { Name = "Product Phase api" } }
        //});

        app.MapGet(string.Empty, async (
               ISender sender,
               [AsParameters] SearchProductPhaseQuery request) =>
        {
            var result = await sender.Send(request);
            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Product Phase api" } }
        });

        app.MapPut("changePhase", async (
                ISender sender,
                [FromBody] UpdateQuantityPhaseRequest request) =>
        {
            var updateQuantityPhaseCommand = new UpdateQuantityPhaseCommand(request);
            var result = await sender.Send(updateQuantityPhaseCommand);
            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Product Phase api" } }
        });

        app.MapPut("changeQuantityType", async (
                ISender sender,
                [FromBody] UpdateQuantityStatusRequest request) =>
        {
            var updateQuantityStatusCommand = new UpdateQuantityStatusCommand(request);
            var result = await sender.Send(updateQuantityStatusCommand);
            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Product Phase api" } }
           
        });

    }
}
