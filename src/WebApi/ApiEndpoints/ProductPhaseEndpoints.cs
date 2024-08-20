using Carter;
using Contract.ProductPhases.Updates.ChangeQuantityStatus;
using Contract.Services.ProductPhase.Queries;
using Contract.Services.ProductPhase.SearchByThirdPartyCompany;
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

        //app.MapPut("changePhase", async (
        //        ISender sender,
        //        [FromBody] UpdateQuantityPhaseRequest request) =>
        //{
        //    var updateQuantityPhaseCommand = new UpdateQuantityPhaseCommand(request);
        //    var result = await sender.Send(updateQuantityPhaseCommand);
        //    return Results.Ok(result);
        //}).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        //{
        //    Tags = new List<OpenApiTag> { new() { Name = "Product Phase api" } }
        //});

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

        app.MapGet("search", async (
              ISender sender,
              [AsParameters] SearchByThirdPartyCompanyQuery request) =>
        {
            var result = await sender.Send(request);
            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Product Phase api" } }
        });

    }
}
