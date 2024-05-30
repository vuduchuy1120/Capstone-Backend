using Carter;
using Contract.Services.Slot.GetSlots;
using MediatR;
using Microsoft.OpenApi.Models;

namespace WebApi.ApiEndpoints;

public class SlotEndpoints : CarterModule
{
    public SlotEndpoints() : base("/api/slots")
    {
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(string.Empty, async (ISender sender) =>
        {
            var result = await sender.Send(new GetAllSlotsQuery());

            return Results.Ok(result);
        }).RequireAuthorization().WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Slot api" } }
        });
    }
}
