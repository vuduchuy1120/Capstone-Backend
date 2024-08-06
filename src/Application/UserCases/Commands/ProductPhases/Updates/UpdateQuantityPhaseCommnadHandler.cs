using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.ProductPhase.Creates;
using Contract.Services.ProductPhase.Updates;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using FluentValidation;

namespace Application.UserCases.Commands.ProductPhases.Updates;

public class UpdateQuantityPhaseCommnadHandler
    (
    IProductPhaseRepository _productPhaseRepository,
    IValidator<UpdateQuantityPhaseRequest> _validator,
    IUnitOfWork _unitOfWork)
     : ICommandHandler<UpdateQuantityPhaseCommand>
{
    public async Task<Result.Success> Handle(UpdateQuantityPhaseCommand request, CancellationToken cancellationToken)
    {
        var validator = await _validator.ValidateAsync(request.updateReq, cancellationToken);
        if (!validator.IsValid)
        {
            throw new MyValidationException(validator.ToDictionary());
        }


        var productPhaseFrom = await _productPhaseRepository.GetByProductIdPhaseIdCompanyID(request.updateReq.ProductId, request.updateReq.PhaseIdFrom, request.updateReq.CompanyId);
        var productPhaseTo = await _productPhaseRepository.GetByProductIdPhaseIdCompanyID(request.updateReq.ProductId, request.updateReq.PhaseIdTo, request.updateReq.CompanyId);


        var quantityAvailable = productPhaseFrom.AvailableQuantity;
        if (quantityAvailable < request.updateReq.quantity)
        {
            throw new MyValidationException("Số lượng hàng hoàn thành lớn hơn số lượng hàng trong kho.");
        }

        var updateQuantityPhase2 = productPhaseFrom.Quantity - request.updateReq.quantity;
        var updateAvailableQuantityPhase2 = productPhaseFrom.AvailableQuantity - request.updateReq.quantity;
        productPhaseFrom.UpdateQuantityPhase(updateQuantityPhase2, updateAvailableQuantityPhase2);
        _productPhaseRepository.UpdateProductPhase(productPhaseFrom);

        if (productPhaseTo == null)
        {
            var productPhase = ProductPhase.Create(new CreateProductPhaseRequest
            (
                ProductId: request.updateReq.ProductId,
                PhaseId: request.updateReq.PhaseIdTo,
                Quantity: request.updateReq.quantity,
                AvailableQuantity: request.updateReq.quantity,
                CompanyId: request.updateReq.CompanyId
            ));
            _productPhaseRepository.AddProductPhase(productPhase);
        }
        else
        {
            var updateQuantityPhaseTo = productPhaseTo.Quantity + request.updateReq.quantity;
            var updateAvailableQuantityPhaseTo = productPhaseTo.AvailableQuantity + request.updateReq.quantity;
            productPhaseTo.UpdateQuantityPhase(updateQuantityPhaseTo, updateAvailableQuantityPhaseTo);
            _productPhaseRepository.UpdateProductPhase(productPhaseTo);
        }

        await _unitOfWork.SaveChangesAsync();

        return Result.Success.Update();

    }
}
