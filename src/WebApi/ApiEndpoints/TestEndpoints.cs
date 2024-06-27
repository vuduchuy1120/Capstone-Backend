using Carter;
using Microsoft.OpenApi.Models;

namespace WebApi.ApiEndpoints;

public class TestEndpoints : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("test", () =>
        {
            return Results.Ok("Test docker dihson103");
        }).WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Test api" } }
        });

    }
}
