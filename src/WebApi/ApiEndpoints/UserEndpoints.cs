using Application.Utils;
using Carter;
using Contract.Services.User.BanUser;
using Contract.Services.User.ChangePassword;
using Contract.Services.User.Command;
using Contract.Services.User.CreateUser;
using Contract.Services.User.GetUserById;
using Contract.Services.User.GetUsers;
using Contract.Services.User.UpdateUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

namespace WebApi.ApiEndpoints;

public class UserEndpoints : CarterModule
{
    public UserEndpoints() : base("/api/users")
    {
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id}", async (ISender sender, string id) =>
        {
            var result = await sender.Send(new GetUserByIdQuery(id));

            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "User api" } }
        });

        app.MapGet("/me", async (ISender sender, ClaimsPrincipal claim) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
            var result = await sender.Send(new GetUserByIdQuery(userId));

            return Results.Ok(result);
        }).RequireAuthorization().WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "User api" } }
        });

        app.MapGet(string.Empty, async (ISender sender, [AsParameters] GetUsersQuery getUsersQuery) =>
        {
            var result = await sender.Send(getUsersQuery);

            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "User api" } }
        });

        app.MapPost(string.Empty, async (
            ISender sender,
            ClaimsPrincipal claim,
            [FromBody] CreateUserRequest userRequest) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
            var createUserCommand = new CreateUserCommand(userRequest, userId);
            var result = await sender.Send(createUserCommand);

            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "User api" } }
        });

        app.MapPut(string.Empty, async (
            ISender sender,
            ClaimsPrincipal claim,
            [FromBody] UpdateUserRequest updateUserRequest) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
            var updateUserCommandHandler = new UpdateUserCommand(updateUserRequest, userId);

            var result = await sender.Send(updateUserCommandHandler);

            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "User api" } }
        });

        app.MapPut("/{id}/status/{isActive}", async (
            ISender sender,
            ClaimsPrincipal claim,
            [FromRoute] string id,
            [FromRoute] bool isActive) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
            var changeUserStatusCommand = new ChangeUserStatusCommand(userId, id, isActive);

            var result = await sender.Send(changeUserStatusCommand);

            return Results.Ok(result);
        }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "User api" } }
        });

        app.MapPut("/change-password", async (
            ISender sender,
            ClaimsPrincipal claim,
            [FromBody] ChangePasswordRequest request) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
            var changePasswordCommand = new ChangePasswordCommand(request, userId);

            var result = await sender.Send(changePasswordCommand);

            return Results.Ok(result);
        }).RequireAuthorization().WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "User api" } }
        });

        app.MapGet("/Company", async (
            ISender sender,
            ClaimsPrincipal claim,
            [AsParameters] GetUsersByCompanyIdRequest request) =>
        {
            var CompanyId = UserUtil.GetCompanyIdFromClaimsPrincipal(claim);
            Guid.TryParse(CompanyId, out var companyId);
            var roleName = UserUtil.GetRoleFromClaimsPrincipal(claim);
            var result = await sender.Send(new GetUsersByCompanyIdQuery(request, companyId, roleName));

            return Results.Ok(result);
        }).RequireAuthorization("RequireAdminOrCounter").WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "User api" } }
        });
    }
}
