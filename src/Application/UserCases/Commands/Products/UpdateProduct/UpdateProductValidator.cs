using Application.Abstractions.Data;
using Contract.Services.Product.UpdateProduct;
using FluentValidation;

namespace Application.UserCases.Commands.Products.UpdateProduct;

public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator(IProductImageRepository productImageRepository)
    {
        RuleFor(req => req.UpdateProductRequest)
            .NotNull().WithMessage("Update product request can not null");

        RuleFor(req => req.UpdateProductRequest.Id)
            .NotEmpty().WithMessage("Product's id cannot be empty");

        RuleFor(req => req.UpdateProductRequest.Code)
            .NotEmpty().WithMessage("Product's code cannot be empty")
            .Matches(@"^[a-zA-Z]{2}\d+$").WithMessage("Product's code must start with two characters followed by numbers");

        RuleFor(req => req.UpdateProductRequest.Price)
                .GreaterThan(0).WithMessage("Product's price must be greater than 0");

        RuleFor(req => req.UpdateProductRequest.Size)
            .NotEmpty().WithMessage("Product's size cannot be empty");

        RuleFor(req => req.UpdateProductRequest.Description)
            .NotEmpty().WithMessage("Product's description cannot be empty");

        RuleFor(req => req.UpdateProductRequest.Name)
           .NotEmpty().WithMessage("Name's description cannot be empty");

        RuleFor(req => req.UpdateProductRequest.RemoveImageIds)
            .MustAsync(async (req, imageIds, _) =>
            {
                if(imageIds is null)
                {
                    return true;
                }

                return await productImageRepository.IsAllImageIdExist(imageIds, req.ProductId);
            }).WithMessage("There is any image not exist");
    }
}
