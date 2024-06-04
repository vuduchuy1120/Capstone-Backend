using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Product.UpdateProduct;

namespace Application.UserCases.Commands.Products.UpdateProduct;

internal sealed class UpdateProductCommandHandler(
    IProductRepository _productRepository, 
    IUnitOfWork _unitOfWork) : ICommandHandler<UpdateProductCommand>
{
    public Task<Result.Success> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
