using Application.Utils;
using Carter;
using Contract.Services.Product.CreateProduct;
using Contract.Services.Product.UpdateProduct;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

namespace WebApi.ApiEndpoints;

public class ProductEndpoints : CarterModule
{
    public ProductEndpoints() : base("api/products")
    {
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(string.Empty, async (ISender sender, [FromBody] CreateProductRequest request, ClaimsPrincipal claim) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
            var createProductCommand = new CreateProductCommand(request, userId);
            var result = await sender.Send(createProductCommand);

            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Product api" } }
        });

        app.MapPut("{productId}", async (
            ISender sender, 
            [FromBody] UpdateProductRequest request,
            [FromRoute] Guid productId,
            ClaimsPrincipal claim) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
            var updateProductCommand = new UpdateProductCommand(request, userId, productId);
            var result = await sender.Send(updateProductCommand);

            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Product api" } }
        });
    }
}
