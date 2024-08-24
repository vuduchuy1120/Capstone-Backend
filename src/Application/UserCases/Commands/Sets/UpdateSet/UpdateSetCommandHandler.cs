using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Set.SharedDto;
using Contract.Services.Set.UpdateSet;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using Domain.Exceptions.SetProducts;
using Domain.Exceptions.Sets;
using FluentValidation;

namespace Application.UserCases.Commands.Sets.UpdateSet;

internal sealed class UpdateSetCommandHandler(
    ISetRepository _setRepository,
    ISetProductRepository _setProductRepository,
    IUnitOfWork _unitOfWork,
    IValidator<UpdateSetRequest> _validator) : ICommandHandler<UpdateSetCommand>
{
    public async Task<Result.Success> Handle(UpdateSetCommand request, CancellationToken cancellationToken)
    {
        var updateSetRequest = request.UpdateSetRequest;
        var setId = request.setId;

        var updateSet = await GetAndValidateRequest(updateSetRequest, setId);

        await RemoveSetProduct(updateSetRequest.RemoveProductIds, setId);
        await UpdateSetProduct(updateSetRequest.Update, setId);
        AddSetProducts(updateSetRequest.Add, setId);
        UpdateSet(updateSet, updateSetRequest, request.UpdatedBy);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success.Update();
    }

    private async Task<Set> GetAndValidateRequest(UpdateSetRequest updateSetRequest, Guid setId)
    {
        if (setId != updateSetRequest.SetId)
        {
            throw new SetIdConflictException();
        }

        var validationResult = await _validator.ValidateAsync(updateSetRequest);

        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }

        var updateSet = await _setRepository.GetByIdWithoutSetProductAsync(setId) ??
            throw new SetNotFoundException(setId);

        if(updateSetRequest.Code != updateSet.Code)
        {
            var isCodeExist = await _setRepository.IsCodeExistAsync(updateSetRequest.Code);
            if (isCodeExist)
            {
                throw new SetBadRequestException($"Bộ sản phẩm có mã: {updateSetRequest.Code} đã tồn tại");
            }
        }

        return updateSet;
    }

    private async Task RemoveSetProduct(List<Guid> productIds, Guid setId)
    {
        if(productIds is null ||  productIds.Count == 0)
        {
            return;
        }

        var removeSetProducts = await _setProductRepository.GetByProductIdsAndSetId(productIds, setId)
            ?? throw new SetProductNotFoundException();
        
        _setProductRepository.DeleteRange(removeSetProducts);
    }

    private async Task UpdateSetProduct(List<SetProductRequest> updateSetProductRequest, Guid setId)
    {
        if (updateSetProductRequest is null || updateSetProductRequest.Count == 0)
        {
            return;
        }

        var productIds = updateSetProductRequest.ConvertAll(rq => rq.ProductId).ToList();
        var updateSetProducts = await _setProductRepository.GetByProductIdsAndSetId(productIds, setId)
            ?? throw new SetProductNotFoundException();

        var updatedSetProducts = updateSetProducts.ConvertAll(sp =>
        {
            var updateSetProduct = updateSetProductRequest.SingleOrDefault(u => u.ProductId == sp.ProductId);
            if (updateSetProduct is not null)
            {
                sp.Update(updateSetProduct.Quantity);
            }
            return sp;
        }).ToList();

        _setProductRepository.UpdateRange(updatedSetProducts);
    }

    private void AddSetProducts(List<SetProductRequest> addSetProductsRequest, Guid setId)
    {
        if(addSetProductsRequest is null  || addSetProductsRequest.Count == 0)
        {
            return;
        }

        var addSetProducts = addSetProductsRequest
            .ConvertAll(rq => SetProduct.Create(setId, rq.ProductId, rq.Quantity))
            .ToList();

        _setProductRepository.AddRange(addSetProducts);
    }

    private void UpdateSet(Set updateSet, UpdateSetRequest request, string updatedBy)
    {
        updateSet.Update(request, updatedBy);
        _setRepository.Update(updateSet);
    }
}
