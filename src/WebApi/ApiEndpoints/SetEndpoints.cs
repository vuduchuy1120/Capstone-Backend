using Application.Utils;
using Carter;
using Contract.Services.Set.CreateSet;
using Contract.Services.Set.GetSet;
using Contract.Services.Set.GetSets;
using Contract.Services.Set.UpdateSet;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

namespace WebApi.ApiEndpoints;

public class SetEndpoints : CarterModule
{
    public SetEndpoints() : base("api/sets")
    {
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(string.Empty, async (
            ISender sender, 
            [FromBody] CreateSetRequest createSetRequest,
            ClaimsPrincipal claim) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
            var createSetCommand = new CreateSetCommand(createSetRequest, userId);

            var result = await sender.Send(createSetCommand);

            return Results.Ok(result);
        }).RequireAuthorization().WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Set api" } }
        });

        app.MapPut("{id}", async(
            ISender sender, 
            [FromRoute] Guid id, 
            [FromBody] UpdateSetRequest updateSetRequest,
            ClaimsPrincipal claim) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
            var updateSetCommand = new UpdateSetCommand(updateSetRequest, userId, id);

            var result = await sender.Send(updateSetCommand);

            return Results.Ok(result);
        }).RequireAuthorization().WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Set api" } }
        });

        app.MapGet("{id}", async (ISender sender, [FromRoute] Guid id) =>
        {
            var getSetQuery = new GetSetQuery(id);

            var result = await sender.Send(getSetQuery);

            return Results.Ok(result);
        }).RequireAuthorization().WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Set api" } }
        });

        app.MapGet(string.Empty, async (ISender sender, [AsParameters] GetSetsQuery request) =>
        {
            var result = await sender.Send(request);

            return Results.Ok(result);
        }).RequireAuthorization().WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Set api" } }
        });
    }
}
