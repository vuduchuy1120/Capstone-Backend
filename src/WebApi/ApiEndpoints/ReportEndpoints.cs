using Application.Utils;
using Carter;
using Contract.Services.Report.Creates;
using Contract.Services.Report.Queries;
using Contract.Services.Report.Updates;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

namespace WebApi.ApiEndpoints;

public class ReportEndpoints : CarterModule
{
    public ReportEndpoints() : base("api/reports")
    {
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(string.Empty, async (
            ISender sender,
            ClaimsPrincipal claims,
            [FromBody] CreateReportRequest request) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(claims);
            var result = await sender.Send(new CreateReportCommand(request, userId));
            return Results.Ok(result);
        }).RequireAuthorization("RequireAnyRole")
        .WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Report api" } }
        });

        app.MapPut(string.Empty, async (
            ISender sender,
            ClaimsPrincipal claims,
            [FromBody] UpdateReportRequest request) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(claims);
            var companyId = UserUtil.GetCompanyIdFromClaimsPrincipal(claims);
            Guid.TryParse(companyId, out var companyIdGuid);
            var roleName = UserUtil.GetRoleFromClaimsPrincipal(claims);
            var result = await sender.Send(
                new UpdateReportCommand(
                    new UpdateReportRequestWithClaims(companyIdGuid, roleName, request), userId));
            return Results.Ok(result);
        }).RequireAuthorization("RequireAdminOrBranchAdmin")
        .WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Report api" } }
        });

        app.MapGet("/search", async (
            ISender _sender,
            ClaimsPrincipal _claims,
            [AsParameters] SearchReportsQuery request) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(_claims);
            var CompanyId = UserUtil.GetCompanyIdFromClaimsPrincipal(_claims);
            Guid.TryParse(CompanyId, out var companyId);
            var roleName = UserUtil.GetRoleFromClaimsPrincipal(_claims);
            var searchReportsQuery = new SearchReportsWithClaimsQuery(request, roleName, companyId, userId);
            var result = await _sender.Send(searchReportsQuery);
            return Results.Ok(result);
        }).RequireAuthorization("RequireAnyRole")
        .WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Report api" } }
        });

        app.MapGet("{id}", async (
                ISender _sender,
                ClaimsPrincipal _claims,
                [FromRoute] Guid id) =>
        {
            var userId = UserUtil.GetUserIdFromClaimsPrincipal(_claims);
            var CompanyId = UserUtil.GetCompanyIdFromClaimsPrincipal(_claims);
            Guid.TryParse(CompanyId, out var companyId);
            var roleName = UserUtil.GetRoleFromClaimsPrincipal(_claims);
            var getReportByIdQuery = new GetReportByIdQuery(id, companyId, userId, roleName);
            var result = await _sender.Send(getReportByIdQuery);
            return Results.Ok(result);
        }).RequireAuthorization("RequireAnyRole")
        .WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Report api" } }
        });
    }
}
