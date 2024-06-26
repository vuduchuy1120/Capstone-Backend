using Application.Utils;
using Carter;
using Contract.Services.Attendance.Create;
using Contract.Services.Attendance.Queries;
using Contract.Services.Attendance.Query;
using Contract.Services.Attendance.Update;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

namespace WebApi.ApiEndpoints;

public class AttendanceEndpoints : CarterModule
{
    public AttendanceEndpoints() : base("/api/attendances")
    {
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {

        app.MapPost("batch", async (
            ISender sender,
            ClaimsPrincipal claim,
            [FromBody] CreateAttendanceDefaultRequest attendanceDefaultRequest) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
            var companyID = UserUtil.GetCompanyIdFromClaimsPrincipal(claim);
            Guid.TryParse(companyID, out var companyId);
            var roleName = UserUtil.GetRoleFromClaimsPrincipal(claim);
            var createAttendanceDefaultCommand = new CreateAttendanceDefaultCommand(attendanceDefaultRequest, userId, roleName, companyId);
            var result = await sender.Send(createAttendanceDefaultCommand);
            return Results.Ok(result);
        }).RequireAuthorization("RequireAdminOrBranchAdmin")
        .WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Attendance api" } }
        });

        //update
        app.MapPut(string.Empty, async (
        ISender sender,
        ClaimsPrincipal claim,
        [FromBody] UpdateAttendancesRequest updateAttendanceRequest) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
            var companyID = UserUtil.GetCompanyIdFromClaimsPrincipal(claim);
            Guid.TryParse(companyID, out var companyId);
            var roleName = UserUtil.GetRoleFromClaimsPrincipal(claim);
            var updateAttendanceCommandHandler = new UpdateAttendancesCommand(updateAttendanceRequest, userId, companyId, roleName);

            var result = await sender.Send(updateAttendanceCommandHandler);

            return Results.Ok(result);
        })
        .RequireAuthorization("RequireAdminOrBranchAdmin")
        .WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Attendance api" } }
        });

        // get Attendances by Date
        app.MapGet(string.Empty, async (
            ISender sender,
            ClaimsPrincipal claim,
            [AsParameters] GetAttendanceRequest request) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
            var companyID = UserUtil.GetCompanyIdFromClaimsPrincipal(claim);
            Guid.TryParse(companyID, out var companyId);
            var roleName = UserUtil.GetRoleFromClaimsPrincipal(claim);
            var query = new GetAttendancesQuery(request, companyId, roleName);
            var result = await sender.Send(query);
            return Results.Ok(result);
        })
        .RequireAuthorization("RequireAdminOrCounter")
        .WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Attendance api" } }
        });

        app.MapGet("/users", async (
            ISender sender,
            ClaimsPrincipal claim,
            int month,
            int year) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
            var request = new GetAttendancesByMonthAndUserIdQuery(month, year, userId);
            var result = await sender.Send(request);
            return Results.Ok(result);
        }).RequireAuthorization("RequireAnyRole")
        .WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Attendance api" } }
        });

        app.MapGet("/users/detail", async (
            ISender sender,
            ClaimsPrincipal claim,
            [AsParameters] GetAttendancesByDateRequest getRequest) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);

            var request = new GetAttendanceByUserIdAndDateQuery(getRequest, userId);
            var result = await sender.Send(request);
            return Results.Ok(result);
        })
        .RequireAuthorization("RequireAnyRole")
        .WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Attendance api" } }
        });


    }
}
