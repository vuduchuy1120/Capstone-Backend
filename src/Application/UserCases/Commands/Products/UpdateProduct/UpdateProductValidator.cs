using Application.Abstractions.Data;
using Contract.Services.Product.UpdateProduct;
using FluentValidation;

namespace Application.UserCases.Commands.Products.UpdateProduct;

public class UpdateProductValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductValidator(IProductRepository _productRepository)
    {
        RuleFor(req => req.Code)
            .NotEmpty().WithMessage("Product's code cannot be empty")
            .Matches(@"^[a-zA-Z]{2}\d+$").WithMessage("Product's code must start with two characters followed by numbers")
            .MustAsync(async (id, _) =>
            {
                return !await _productRepository.IsProductCodeExist(id);
            }).WithMessage("Product's code already exist");

        RuleFor(req => req.Price)
                .GreaterThan(0).WithMessage("Product's price must be greater than 0");

        RuleFor(req => req.Size)
            .NotEmpty().WithMessage("Product's size cannot be empty");

        RuleFor(req => req.Description)
            .NotEmpty().WithMessage("Product's description cannot be empty");
    }
}
