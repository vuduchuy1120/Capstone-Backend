using Carter;
using Contract.Services.Files.DeleteFile;
using Contract.Services.Files.GetFile;
using Contract.Services.Files.UploadFile;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace WebApi.ApiEndpoints;

public class FileEndpoints : CarterModule
{
    public FileEndpoints() : base("api/files")
    {
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(string.Empty, async (ISender sender, IFormFile file) =>
        {
            var uploadFileCommand = new UploadFileCommand(file);
            var result = await sender.Send(uploadFileCommand);

            return Results.Ok(result);
        }).WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new OpenApiTag { Name = "File API" } }
        }).DisableAntiforgery();

        app.MapGet("{fileName}", async (ISender sender, [FromRoute] string fileName, HttpContext context) =>
        {
            var getFileQuery = new GetFileQuery(fileName);
            var result = await sender.Send(getFileQuery);

            byte[] fileBytes = result.data;

            context.Response.ContentType = "image/png";
            await context.Response.BodyWriter.WriteAsync(fileBytes);
        }).WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new OpenApiTag { Name = "File API" } }
        }).DisableAntiforgery();

        app.MapDelete("{fileName}", async (ISender sender, [FromRoute] string fileName) =>
        {
            var deleteFileCommand = new DeleteFileCommand(fileName);

            var result = await sender.Send(deleteFileCommand);

            return Results.Ok(result);
        }).WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new OpenApiTag { Name = "File API" } }
        });
    }
}
