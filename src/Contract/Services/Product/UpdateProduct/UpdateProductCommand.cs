using Contract.Abstractions.Messages;

namespace Contract.Services.Product.UpdateProduct;

public record UpdateProductCommand(UpdateProductRequest UpdateProductRequest, string UpdatedBy) : ICommand;
