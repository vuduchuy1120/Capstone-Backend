using Carter;
using Contract.Services.MonthEmployeeSalary;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace WebApi.ApiEndpoints;

public class MonthlyEmployeeSalaryEndpoints : CarterModule
{
    public MonthlyEmployeeSalaryEndpoints() : base("api/monthly-employee-salaries")
    {

    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {

        app.MapPost(string.Empty, async (
            ISender sender) =>
        {
            var command = new CreateMonthEmployeeSalaryCommand(6, 2024);
            var result = await sender.Send(command);
            return Results.Ok(result);
        }).WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Monthly employee product api" } }
        });

    }
}
