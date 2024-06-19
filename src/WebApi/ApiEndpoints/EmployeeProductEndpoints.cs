using Application.Utils;
using Carter;
using Contract.Services.Attendance.Create;
using Contract.Services.EmployeeProduct.Creates;
using Contract.Services.EmployeeProduct.Deletes;
using Contract.Services.EmployeeProduct.Queries;
using Contract.Services.EmployeeProduct.Updates;
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
        app.MapPut(string.Empty, async (
               ISender sender,
               ClaimsPrincipal claim,
               [FromBody] UpdateEmployeeProductRequest updateEmployeeProductRequest) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
            var updateEmployeeProductCommand = new UpdateEmployeeProductCommand(updateEmployeeProductRequest, userId);
            var result = await sender.Send(updateEmployeeProductCommand);

            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "EmployeeProduct api" } }
        });
        app.MapDelete(string.Empty, async (
                ISender sender,
                [FromBody] DeleteEmployeeProductRequest deleteEmployeeProductRequest) =>
        {
            var deleteEmployeeProductCommand = new DeleteEmployeeProductCommand(deleteEmployeeProductRequest);
            var result = await sender.Send(deleteEmployeeProductCommand);

            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "EmployeeProduct api" } }
        });

        app.MapGet(string.Empty, async (
                       ISender sender,
                        [FromQuery] int slotId,
                        [FromQuery] string userId,
                        [FromQuery] string date) =>
        {
            var getEmployeeProductsByEmployeeIdDateAndSlotIdQuery = new GetEmployeeProductsByEmployeeIdDateAndSlotIdQuery(slotId, userId, date);
            var result = await sender.Send(getEmployeeProductsByEmployeeIdDateAndSlotIdQuery);

            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "EmployeeProduct api" } }
        });

    }
}
