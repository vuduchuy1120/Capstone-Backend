using Application.Utils;
using Carter;
using Contract.Services.SalaryHistory.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

namespace WebApi.ApiEndpoints;

public class SalaryHistoryEndpoints : CarterModule
{
    public SalaryHistoryEndpoints() : base("api/salary-histories")
    {
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/salaryByDay/{userId}", async (
            ISender sender,
            ClaimsPrincipal claim,
            [FromRoute] string userId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10
            ) =>
        {
            var userIdClaim = UserUtil.GetUserIdFromClaimsPrincipal(claim);
            var roleName = UserUtil.GetRoleFromClaimsPrincipal(claim);
            var result = await sender.Send(new GetSalaryByDayByUserIdQuery(userId, roleName, userIdClaim, pageIndex, pageSize));
            return Results.Ok(result);

        }).RequireAuthorization("RequireAnyRole")
        .WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Salary History api" } }
        });

        app.MapGet("/salaryOverTime/{userId}", async (
            ISender sender,
            ClaimsPrincipal claim,
            [FromRoute] string userId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10) =>
        {
            var userIdClaim = UserUtil.GetUserIdFromClaimsPrincipal(claim);
            var roleName = UserUtil.GetRoleFromClaimsPrincipal(claim);
            var result = await sender.Send(new GetSalaryOverTimeByUserIdQuery(userId, roleName, userIdClaim, pageIndex, pageSize));
            return Results.Ok(result);
        }).RequireAuthorization("RequireAnyRole")
        .WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Salary History api" } }
        });
    }
}
