using Contract.Abstractions.Messages;
using Contract.Services.Product.SharedDto;

namespace Contract.Services.Product.GetProduct;

public record GetProductQuery(Guid productId) : IQuery<ProductWithSalaryResponse>;