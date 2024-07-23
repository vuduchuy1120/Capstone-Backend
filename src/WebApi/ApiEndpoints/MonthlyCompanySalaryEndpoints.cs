using Carter;
using Contract.Services.MonthlyCompanySalary.Creates;
using MediatR;
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
            Tags = new List<OpenApiTag> { new() { Name = "monthly company salary api test" } }
        });

    }
}
