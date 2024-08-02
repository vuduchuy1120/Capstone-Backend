using Carter;
using Contract.Services.Phase.Creates;
using Contract.Services.Phase.Queries;
using Contract.Services.Phase.Updates;
using MediatR;
using Microsoft.OpenApi.Models;

namespace WebApi.ApiEndpoints
{
    public class PhaseEndpoints : CarterModule
    {
        public PhaseEndpoints() : base("/api/phase")
        {
        }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            //app.MapPost(string.Empty, async (
            //    ISender sender,
            //    CreatePhaseRequest createPhaseRequest) =>
            //{
            //    var createPhaseCommand = new CreatePhaseCommand(createPhaseRequest);
            //    var result = await sender.Send(createPhaseCommand);
            //    return Results.Ok(result);
            //}).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
            //{
            //    Tags = new List<OpenApiTag> { new() { Name = "Phase api" } }
            //});

            //app.MapPut(string.Empty, async (
            //                   ISender sender,
            //                   UpdatePhaseRequest updatePhaseRequest) =>
            //{
            //    var updatePhaseCommand = new UpdatePhaseCommand(updatePhaseRequest);
            //    var result = await sender.Send(updatePhaseCommand);
            //    return Results.Ok(result);
            //}).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
            //{
            //    Tags = new List<OpenApiTag> { new() { Name = "Phase api" } }
            //});

            app.MapGet(string.Empty, async (ISender sender) =>
            {
                var result = await sender.Send(new GetPhasesQuery());
                return Results.Ok(result);
            }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
            {
                Tags = new List<OpenApiTag> { new() { Name = "Phase api" } }
            });
            // getphasebyid
            //app.MapGet("{id}", async (ISender sender, Guid id) =>
            //{
            //    var result = await sender.Send(new GetPhaseByIdQuery(id));
            //    return Results.Ok(result);
            //}).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
            //{
            //    Tags = new List<OpenApiTag> { new() { Name = "Phase api" } }
            //});

        }
    }
}
