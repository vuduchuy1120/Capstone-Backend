using Application.Abstractions.Data;
using Contract.Services.Product.CreateProduct;
using FluentValidation;

namespace Application.UserCases.Commands.Products.CreateProduct
{
    public class CreateProductValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductValidator(IProductRepository _productRepository)
        {
            RuleFor(req => req.Code)
                .NotEmpty().WithMessage("Product's code cannot be empty")
                .Matches(@"^[a-zA-Z]{2}\d+$").WithMessage("Product's code must start with two characters followed by numbers")
                .MustAsync(async (code, _) => !await _productRepository.IsProductCodeExist(code))
                .WithMessage("Product's code already exists");

            RuleFor(req => req.Price)
                .GreaterThan(0).WithMessage("Product's price must be greater than 0");

            RuleFor(req => req.Size)
                .NotEmpty().WithMessage("Product's size cannot be empty");

            RuleFor(req => req.Description)
                .NotEmpty().WithMessage("Product's description cannot be empty");

            RuleFor(req => req.ImageRequests)
                .Must(imageRequests =>
                {
                    return imageRequests.Count(image => image.IsMainImage == true) == 1;
                }).WithMessage("Must have one main image");

            RuleFor(req => req.ProductUnitRequests)
                .MustAsync(async (req, productUnitRequests, _) =>
                {
                    if (!req.IsGroup)
                    {
                        return productUnitRequests == null || productUnitRequests.Count == 0;
                    }

                    if (productUnitRequests == null || productUnitRequests.Count == 0)
                    {
                        return true;
                    }

                    var subProductIds = productUnitRequests.Select(r => r.SubProductId).ToList();

                    return !await _productRepository.IsHaveGroupInSubProductIds(subProductIds);
                })
                .WithMessage(req => req.IsGroup
                    ? "If product is a group, sub-products cannot be groups."
                    : "If product is not a group, it should not have any sub-products.");
        }
    }
}
