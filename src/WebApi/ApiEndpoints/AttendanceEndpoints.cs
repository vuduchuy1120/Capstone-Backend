using Application.Utils;
using Carter;
using Contract.Services.Attendance.Create;
using Contract.Services.Attendance.Update;
using Contract.Services.User.CreateUser;
using Contract.Services.User.UpdateUser;
using MediatR;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

namespace WebApi.ApiEndpoints
{
    public class AttendanceEndpoints : CarterModule
    {
        public AttendanceEndpoints() : base("/api/attendance")
        {
        }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost(string.Empty, async (
            ISender sender,
            ClaimsPrincipal claim,
            [FromBody] CreateAttendanceRequest attendanceRequest) =>
            {
                var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
                var createAttendanceCommand = new CreateAttendanceCommand(attendanceRequest, userId);
                var result = await sender.Send(createAttendanceCommand);

                return Results.Ok(result);
            }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
            {
                Tags = new List<OpenApiTag> { new() { Name = "Attendance api" } }
            });

            app.MapPost("batch", async(
                ISender sender,
                ClaimsPrincipal claim,
                [FromBody] CreateAttendanceDefaultRequest attendanceDefaultRequest) =>
            {
                var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
                var createAttendanceDefaultCommand = new CreateAttendanceDefaultCommand(attendanceDefaultRequest, userId);
                var result = await sender.Send(createAttendanceDefaultCommand);
                return Results.Ok(result);
            }).RequireAuthorization("Require-Admin").WithOpenApi(x => new OpenApiOperation(x)
            {
                Tags= new List<OpenApiTag> { new() { Name = "Attendance api" } }
            });

            //update
            app.MapPut(string.Empty, async (
            ISender sender,
            ClaimsPrincipal claim,
            [FromBody] UpdateAttendanceRequest updateAttendanceRequest) =>
            {
                var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
                var updateAttendanceCommandHandler = new UpdateAttendanceCommand(updateAttendanceRequest, userId);

                var result = await sender.Send(updateAttendanceCommandHandler);

                return Results.Ok(result);
            })
               .RequireAuthorization("Require-Admin")
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Tags = new List<OpenApiTag> { new() { Name = "Attendance api" } }
            });

        }
    }
}
