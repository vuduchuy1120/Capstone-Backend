using Application.Abstractions.Data;
using Carter;
using Contract.Services.Product.CreateProduct;
using Domain.Entities;
using Microsoft.OpenApi.Models;

namespace WebApi.ApiEndpoints;

public class TestEndpoints : CarterModule
{
    public TestEndpoints() : base("api/tests")
    {
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(string.Empty, async (IProductRepository productRepository, IUnitOfWork _unitOfWork) =>
        {
            var createProductRequest = new CreateProductRequest("Code", 3434, "Size", "Description", false, "Name", null, null);
            var product = Product.Create(createProductRequest, "001201011091");
            productRepository.Add(product);
            await _unitOfWork.SaveChangesAsync();

            return Results.Ok("oki");
        }).WithOpenApi(x => new OpenApiOperation(x)
        {
            Tags = new List<OpenApiTag> { new() { Name = "Test api" } }
        });
    }
}
