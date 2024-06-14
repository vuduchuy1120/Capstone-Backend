using Application.Utils;
using Carter;
using Contract.Services.Attendance.Create;
using Contract.Services.EmployeeProduct.Creates;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

namespace WebApi.ApiEndpoints;

public class EmployeeProductEndpoints : CarterModule
{
    public EmployeeProductEndpoints() : base("/api/EmployeeProduct")
    {
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(string.Empty, async (
       ISender sender,
       ClaimsPrincipal claim,
       [FromBody] CreateEmployeeProductRequest createEmployeeProductRequest) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
            var createEmployeeProductCommand = new CreateEmployeeProductComand(createEmployeeProductRequest, userId);
            var result = await sender.Send(createEmployeeProductCommand);

            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "EmployeeProduct api" } }
        });

    }
}
