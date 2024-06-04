using Contract.Abstractions.Messages;

namespace Contract.Services.Product.CreateProduct;

public record CreateProductCommand(CreateProductRequest CreateProductRequest, string CreatedBy) : ICommand;
