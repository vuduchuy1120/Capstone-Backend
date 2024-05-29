using Carter;
using Contract.Services.User.Login;
using Contract.Services.User.Logout;
using MediatR;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Application.Utils;
using Contract.Services.User.ForgetPassword;
using Contract.Services.User.ConfirmVerifyCode;

namespace WebApi.ApiEndpoints;

public class AuthEndpoints : CarterModule
{
    public AuthEndpoints() : base("/api/auth")
    {
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/login", async (ISender sender, [FromBody] LoginCommand loginCommand) =>
        {
            var result = await sender.Send(loginCommand);

            return Results.Ok(result);
        }).WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Authentication api" } }
        });

        app.MapPost("/logout/{id}", async (ISender sender, ClaimsPrincipal claim, [FromRoute] string id) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
            var logoutCommand = new LogoutCommand(userId, id);

            var result = await sender.Send(logoutCommand);

            return Results.Ok(result);
        }).RequireAuthorization().WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Authentication api" } }
        });

        app.MapPost("/forget-password/{id}", async (ISender sender, [FromRoute] string id) =>
        {
            //var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
            var forgetPasswordCommand = new ForgetPasswordCommand(id);

            var result = await sender.Send(forgetPasswordCommand);

            return Results.Ok(result);
        }).WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Authentication api" } }
        });

        app.MapPost("/confirm/forgetpassword", async (ISender sender, [FromBody] ConfirmVerifyCodeCommand confirmVerifyCodeCommand) =>
        {
            var result = await sender.Send(confirmVerifyCodeCommand);

            return Results.Ok(result);
        }).WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Authentication api" } }
        });
    }
}
