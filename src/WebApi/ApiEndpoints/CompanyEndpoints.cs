using Carter;
using Contract.Services.Company.Create;
using Contract.Services.Company.Queries;
using Contract.Services.Company.Updates;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace WebApi.ApiEndpoints;

public class CompanyEndpoints : CarterModule
{
    public CompanyEndpoints() : base("/api/companies")
    {
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(string.Empty, async (ISender _sender,
            [FromBody] CreateCompanyRequest createCompanyRequest) =>
        {
            var result = await _sender.Send(new CreateCompanyCommand(createCompanyRequest));
            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Company api" } }
        });
        app.MapPut(string.Empty, async (ISender _sender,
            [FromBody] UpdateCompanyRequest updateCompanyRequest) =>
        {
            var result = await _sender.Send(new UpdateCompanyCommand(updateCompanyRequest));
            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Company api" } }
        });

        app.MapGet(string.Empty, async (
                ISender _sender,
                [AsParameters] SearchCompanyQuery searchCompanyQuery) =>
        {
            var result = await _sender.Send(searchCompanyQuery);
            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Company api" } }
        });

        app.MapGet("{id}", async (
                ISender _sender,
                [FromRoute] Guid id) =>
        {
            var result = await _sender.Send(new GetCompanyByIdQuery(id));
            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Company api" } }
        });
    }
}
