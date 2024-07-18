using Application.Utils;
using Carter;
using Contract.Services.PaidSalary.Creates;
using Contract.Services.PaidSalary.Queries;
using Contract.Services.PaidSalary.Updates;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

namespace WebApi.ApiEndpoints
{
    public class PaidSalaryEndpoints : CarterModule
    {
        public PaidSalaryEndpoints() : base("api/paid-salaries")
        {
        }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost(string.Empty, async (
                ISender sender,
                ClaimsPrincipal claim,
                [FromBody] CreatePaidSalaryRequest request
                ) =>
            {
                var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
                var command = new CreatePaidSalaryCommand(request, userId);
                var result = await sender.Send(command);
                return Results.Ok(result);
            }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
            {
                Tags = new List<OpenApiTag> { new() { Name = "Paid Salary api" } }
            });

            app.MapPut(string.Empty, async (
                ISender sender,
                ClaimsPrincipal claim,
                [FromBody] UpdatePaidSalaryRequest request
                ) =>
            {
                var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
                var command = new UpdatePaidSalaryCommand(request, userId);
                var result = await sender.Send(command);
                return Results.Ok(result);
            }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
            {
                Tags = new List<OpenApiTag> { new() { Name = "Paid Salary api" } }
            });

            app.MapGet("/users/{UserId}", async (
                ISender sender,
                ClaimsPrincipal claim,
                [FromRoute] string UserId,
                [FromQuery] int PageIndex = 1,
                [FromQuery] int PageSize = 10
                ) =>
            {
                var UserIdClaim = UserUtil.GetUserIdFromClaimsPrincipal(claim);
                var roleName = UserUtil.GetRoleFromClaimsPrincipal(claim);
                var query = new GetPaidSalaryByUserIdQuery(UserId, UserIdClaim, roleName, PageIndex, PageSize);
                var result = await sender.Send(query);
                return Results.Ok(result);
            }).RequireAuthorization("RequireAnyRole")
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Tags = new List<OpenApiTag> { new() { Name = "Paid Salary api" } }
            });
        }
    }
}
