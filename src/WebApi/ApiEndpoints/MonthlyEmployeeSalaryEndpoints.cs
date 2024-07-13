using Application.Utils;
using Carter;
using Contract.Services.MonthEmployeeSalary.Creates;
using Contract.Services.MonthEmployeeSalary.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

namespace WebApi.ApiEndpoints;

public class MonthlyEmployeeSalaryEndpoints : CarterModule
{
    public MonthlyEmployeeSalaryEndpoints() : base("api/monthly-employee-salaries")
    {

    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {

        app.MapPost(string.Empty, async (
            [FromBody] CreateSalaryRequest request,
            ISender sender) =>
        {
            var command = new CreateMonthEmployeeSalaryCommand(request.month, request.year);
            var result = await sender.Send(command);
            return Results.Ok(result);
        }).WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Test Create Salary employee" } }
        });

        app.MapGet("/{userId}/{month}/{year}", async (
            ISender _sender,
            ClaimsPrincipal _claimsPrincipal,
            [FromRoute] string userId,
            [FromRoute] int month,
            [FromRoute] int year) =>
            {
                var userIdClaim = UserUtil.GetUserIdFromClaimsPrincipal(_claimsPrincipal);
                var roleClaim = UserUtil.GetRoleFromClaimsPrincipal(_claimsPrincipal);
                var result = await _sender.Send(new GetMonthlyEmployeeSalaryByUserIdQuery(userId, month, year, userIdClaim, roleClaim));
                return Results.Ok(result);
            })
            .RequireAuthorization("RequireAnyRole")
            .WithOpenApi(x=> new OpenApiOperation(x)
            {
                Tags = new List<OpenApiTag> { new() { Name = "Monthly employee product api" } }
            })
            ;
    }
}

public record CreateSalaryRequest
(
    int month,
    int year
    );