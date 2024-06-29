using Carter;
using Contract.Services.Role.GetRoles;
using MediatR;
using Microsoft.OpenApi.Models;

namespace WebApi.ApiEndpoints
{
    public class RoleApiEndpoints : CarterModule
    {
        public RoleApiEndpoints() : base("api/roles")
        {
        }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet(string.Empty, async (ISender sender) =>
            {
                var result = await sender.Send(new GetRolesQuery());

                return Results.Ok(result);
            }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
            {
                Tags = new List<OpenApiTag> { new() { Name = "Roles api" } }
            });
        }
    }
}
