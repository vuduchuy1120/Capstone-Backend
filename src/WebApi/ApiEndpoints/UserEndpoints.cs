using Application.Users.Create;
using Application.Users.GetById;
using Application.Users.Login;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.ApiEndpoints;

public class UserEndpoints : CarterModule
{
    public UserEndpoints() : base("/api/users")
    {
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id}", async (ISender sender,  string id) =>
        {
            var result = await sender.Send(new GetByIdQuery() { Id = id });

            return Results.Ok(result);
        }).RequireAuthorization();

        app.MapPost("", async (ISender sender, [FromBody] CreateUserRequest userRequest) =>
        {
            var createUserCommand = new CreateUserCommand()
            {
                Id = userRequest.Id,
                Fullname = userRequest.Fullname,
                Phone = userRequest.Phone,
                Password = userRequest.Password,   
                Address = userRequest.Address,
                RoleId = userRequest.RoleId
            };
            var result = await sender.Send(createUserCommand);

            return Results.Ok(result);  
        });

        app.MapPost("/login", async (ISender sender, [FromBody] LoginRequest loginRequest) =>
        {
            var loginCommand = new LoginCommand()
            {
                Id = loginRequest.Id,
                Password = loginRequest.Password,
            };
            var result = await sender.Send(loginCommand);

            return Results.Ok(result);
        });
    }
}
