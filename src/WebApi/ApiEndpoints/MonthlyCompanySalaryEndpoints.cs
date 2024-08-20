using Carter;
using Contract.Services.MonthlyCompanySalary.Creates;
using Contract.Services.MonthlyCompanySalary.Queries;
using Contract.Services.MonthlyCompanySalary.Updates;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace WebApi.ApiEndpoints;

public class MonthlyCompanySalaryEndpoints : CarterModule
{
    public MonthlyCompanySalaryEndpoints() : base("api/monthly-company-salaries")
    {
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(string.Empty, async (
            ISender sender,
            int month,
            int year
            ) =>
            {
                var request = new CreateMonthlyCompanySalaryCommand(month, year);
                var result = await sender.Send(request);
                return Results.Ok(result);
            }).WithOpenApi(x => new OpenApiOperation(x)
            {
                Tags = new List<OpenApiTag> { new() { Name = "Monthly company salary api test" } }
            });

        app.MapPut(string.Empty, async (
            ISender _sender,
            [FromBody] UpdateMonthlyCompanySalaryRequest request
            ) =>
            {
                var result = await _sender.Send(new UpdateStatusMonthlyCompanySalaryCommand(request));
                return Results.Ok(result);
            }).RequireAuthorization("Require-Admin")
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Tags = new List<OpenApiTag> { new() { Name = "Monthly company salary api" } }
            });
        app.MapGet(string.Empty, async (
            ISender sender,
            [AsParameters] GetMonthlyCompanySalaryQuery request) =>
        {
            var result = await sender.Send(request);
            return Results.Ok(result);
        })
        .RequireAuthorization("Require-Admin")
        .WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Monthly company salary api" } }
        });

        app.MapGet("detail", async (
            ISender sender,
            [AsParameters] GetMonthlyCompanySalaryByIdQuery request) =>
        {
            var result = await sender.Send(request);
            return Results.Ok(result);
        })
        .RequireAuthorization("Require-Admin")
        .WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Monthly company salary api" } }
        });
    }
}
