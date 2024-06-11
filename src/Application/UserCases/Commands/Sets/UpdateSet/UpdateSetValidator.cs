using Application.Abstractions.Data;
using Contract.Services.Set.UpdateSet;
using FluentValidation;

namespace Application.UserCases.Commands.Sets.UpdateSet;

public class UpdateSetValidator : AbstractValidator<UpdateSetRequest>
{
    public UpdateSetValidator(ISetProductRepository setProductRepository, IProductRepository productRepository)
    {
        RuleFor(req => req.SetId)
            .NotEmpty().WithMessage("Set's id cannot be empty");

        RuleFor(req => req.Code)
                .NotEmpty().WithMessage("Set's code cannot be empty")
                .Matches(@"^[a-zA-Z]{2}\d+$").WithMessage("Set's code must start with two characters followed by numbers");

        RuleFor(req => req.Description)
                .NotEmpty().WithMessage("Set's description cannot be empty");

        RuleFor(req => req.Name)
            .NotEmpty().WithMessage("Set's description cannot be empty");

        RuleFor(req => req.ImageUrl)
            .NotEmpty().WithMessage("Set's image cannot be empty");

        RuleFor(req => req.Add)
            .MustAsync(async (req, addSetProductsRequest, _) =>
            {
                if (addSetProductsRequest is null || !addSetProductsRequest.Any())
                {
                    return true;
                }

                var productIds = addSetProductsRequest.Select(p => p.ProductId).ToList();

                var areAllProductIdsExist = await productRepository.IsAllSubProductIdsExist(productIds);
                if (!areAllProductIdsExist)
                {
                    return false;
                }

                return !await setProductRepository.IsAnyIdExistAsync(productIds, req.SetId);
            }).WithMessage("Some productId already exist in set product or not found");

        RuleFor(req => req.RemoveProductIds)
            .MustAsync(async (req, removeProductIds, _) =>
            {
                if (removeProductIds is null || !removeProductIds.Any())
                {
                    return true;
                }

                if(req.Update is not null)
                {
                    var updateProductIds = req.Update.Select(p => p.ProductId).ToList();
                    var intersection = removeProductIds.Intersect(updateProductIds);

                    if (intersection.Any())
                    {
                        return false;
                    }
                }

                return await setProductRepository.DoProductIdsExistAsync(removeProductIds, req.SetId);
            }).WithMessage("Some productIds in RemoveProductIds are either in Update list or do not exist in db");

        RuleFor(req => req.Update)
            .MustAsync(async (req, updateSetProductsRequest, _) =>
            {
                if (updateSetProductsRequest is null || !updateSetProductsRequest.Any())
                {
                    return true;
                }

                var productIds = updateSetProductsRequest.Select(p => p.ProductId).ToList();
                return await setProductRepository.DoProductIdsExistAsync(productIds, req.SetId);
            }).WithMessage("Some productIds in Update do not exist in db");
    }
}
