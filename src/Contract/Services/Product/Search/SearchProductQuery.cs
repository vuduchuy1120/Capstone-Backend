using Contract.Abstractions.Messages;
using Contract.Services.Product.SharedDto;

namespace Contract.Services.Product.Search;

public record SearchProductQuery(string Search) : IQuery<List<ProductWithOneImageWithSalary>>;
