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
using Infrastructure.Services;
using Application.Abstractions.Services;
using Contract.Services.User.RefreshToken;

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

        app.MapPost("test", async (ISmsService smsService) =>
        {
            var from = "14142613150";
            var to = "84976099351";
            var message = "Hello nguyen dinh son";

            await smsService.SendSmsAsync(from, to, message);

            return Results.Ok("Send sms request ok");
        }).WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Authentication api" } }
        });

        app.MapPost("refresh-token", async (ISender sender, [FromBody] RefreshTokenCommand refreshTokenCommand) =>
        {
            var result = await sender.Send(refreshTokenCommand);

            return Results.Ok(result);
        }).WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Authentication api" } }
        });



    }
}