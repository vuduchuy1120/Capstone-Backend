using Application.Abstractions.Data;
using Contract.Services.Set.CreateSet;
using FluentValidation;

namespace Application.UserCases.Commands.Sets.CreateSet;

public class CreateSetValidator : AbstractValidator<CreateSetRequest>
{
    public CreateSetValidator(ISetRepository setRepository, IProductRepository productRepository)
    {
        RuleFor(req => req.Code)
                .NotEmpty().WithMessage("Set's code cannot be empty")
                .Matches(@"^[a-zA-Z]{2}\d+$").WithMessage("Set's code must start with two characters followed by numbers")
                .MustAsync(async (code, _) => !await setRepository.IsCodeExistAsync(code))
                .WithMessage("Set's code already exists");

        RuleFor(req => req.Description)
                .NotEmpty().WithMessage("Set's description cannot be empty");

        RuleFor(req => req.Name)
            .NotEmpty().WithMessage("Set's description cannot be empty");

        RuleFor(req => req.ImageUrl)
            .NotEmpty().WithMessage("Set's image cannot be empty");

        RuleFor(req => req.SetProductsRequest)
            .MustAsync(async (req, setProductsRequest, _) =>
            {
                if(setProductsRequest is null)
                {
                    return true;
                }

                var productIds = setProductsRequest.Select(p => p.ProductId).ToList();
                if (productIds.Count != productIds.Distinct().Count())
                {
                    return false;
                }

                return await productRepository.IsAllSubProductIdsExist(productIds);
            }).WithMessage("Duplicate product IDs found or some product IDs do not exist.");
    }
}
