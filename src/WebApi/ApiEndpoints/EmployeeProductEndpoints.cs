using Application.Utils;
using Carter;
using Contract.Services.Attendance.Create;
using Contract.Services.EmployeeProduct.Creates;
using Contract.Services.EmployeeProduct.Deletes;
using Contract.Services.EmployeeProduct.Queries;
using Contract.Services.EmployeeProduct.Updates;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

namespace WebApi.ApiEndpoints;

public class EmployeeProductEndpoints : CarterModule
{
    public EmployeeProductEndpoints() : base("/api/EmployeeProduct")
    {
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(string.Empty, async (
       ISender sender,
       ClaimsPrincipal claim,
       [FromBody] CreateEmployeeProductRequest createEmployeeProductRequest) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(claim);
            var CompanyId = UserUtil.GetCompanyIdFromClaimsPrincipal(claim);
            Guid.TryParse(CompanyId, out var companyId);
            var roleName = UserUtil.GetRoleFromClaimsPrincipal(claim);
            var createEmployeeProductCommand = new CreateEmployeeProductComand(createEmployeeProductRequest, userId, roleName, companyId);
            var result = await sender.Send(createEmployeeProductCommand);

            return Results.Ok(result);
        }).RequireAuthorization("RequireAdminOrCounter")
        .WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "EmployeeProduct api" } }
        });

        app.MapGet(string.Empty, async (
                       ISender sender,
                       ClaimsPrincipal claim,
                        [AsParameters] GetEmployeeProductsByEmployeeIdDateAndSlotIdRequest request) =>
        {
            var CompanyId = UserUtil.GetCompanyIdFromClaimsPrincipal(claim);
            Guid.TryParse(CompanyId, out var companyIdGuid);
            var roleName = UserUtil.GetRoleFromClaimsPrincipal(claim);
            var userIdClaim = UserUtil.GetUserIdFromClaimsPrincipal(claim);
            var getEmployeeProductsByEmployeeIdDateAndSlotIdQuery = new GetEmployeeProductsByEmployeeIdDateAndSlotIdQuery(request, roleName,userIdClaim,companyIdGuid);
            var result = await sender.Send(getEmployeeProductsByEmployeeIdDateAndSlotIdQuery);

            return Results.Ok(result);
        })
        .RequireAuthorization("RequireAnyRole")
        .WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "EmployeeProduct api" } }
        });

    }
}
