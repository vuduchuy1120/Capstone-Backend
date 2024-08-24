using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.ProductPhases.Updates.ChangeQuantityStatus;
using Contract.Services.ProductPhase.ShareDto;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using FluentValidation;

namespace Application.UserCases.Commands.ProductPhases.Updates;

public sealed class UpdateQuantityStatusCommandHandler
    (IProductPhaseRepository _productPhaseRepository,
    IUnitOfWork _unitOfWork,
    IValidator<UpdateQuantityStatusRequest> _validator
        ) : ICommandHandler<UpdateQuantityStatusCommand>
{
    public async Task<Result.Success> Handle(UpdateQuantityStatusCommand request, CancellationToken cancellationToken)
    {
        var validator = await _validator.ValidateAsync(request.updateQuantityStatusRequest, cancellationToken);
        if (!validator.IsValid)
        {
            throw new MyValidationException(validator.ToDictionary());
        }

        var productPhaseFrom = await _productPhaseRepository.GetByProductIdPhaseIdAndCompanyIdAsync(
           request.updateQuantityStatusRequest.ProductId,
           request.updateQuantityStatusRequest.PhaseIdFrom,
           request.updateQuantityStatusRequest.CompanyIdFrom);

        var productPhaseTo = await _productPhaseRepository.GetByProductIdPhaseIdAndCompanyIdAsync(
            request.updateQuantityStatusRequest.ProductId,
            request.updateQuantityStatusRequest.PhaseIdTo,
            request.updateQuantityStatusRequest.CompanyIdTo);


        var quantity = request.updateQuantityStatusRequest.Quantity;
        var from = request.updateQuantityStatusRequest.From;
        var to = request.updateQuantityStatusRequest.To;

        switch (from)
        {
            case QuantityType.QUANTITY:
                if (productPhaseFrom.AvailableQuantity < quantity)
                {
                    throw new MyValidationException("Số lượng không đủ để cập nhật.");
                }
                productPhaseFrom.UpdateAvailableQuantity(productPhaseFrom.AvailableQuantity - quantity);
                productPhaseFrom.UpdateQuantity(productPhaseFrom.Quantity - quantity);
                break;
            case QuantityType.ERROR_QUANTITY:
                if (productPhaseFrom.ErrorAvailableQuantity < quantity)
                {
                    throw new MyValidationException("Số lượng không đủ để cập nhật.");
                }
                productPhaseFrom.UpdateErrorAvailableQuantity(productPhaseFrom.ErrorAvailableQuantity - quantity);
                productPhaseFrom.UpdateErrorQuantity(productPhaseFrom.ErrorQuantity - quantity);
                break;
            case QuantityType.FAILURE_QUANTITY:
                if (productPhaseFrom.FailureAvailabeQuantity < quantity)
                {
                    throw new MyValidationException("Số lượng không đủ để cập nhật.");
                }
                productPhaseFrom.UpdateFailureAvailableQuantity(productPhaseFrom.FailureAvailabeQuantity - quantity);
                productPhaseFrom.UpdateFailureQuantity(productPhaseFrom.FailureQuantity - quantity);
                break;
            case QuantityType.BROKEN_QUANTITY:
                if (productPhaseFrom.BrokenAvailableQuantity < quantity)
                {
                    throw new MyValidationException("Số lượng không đủ để cập nhật.");
                }
                productPhaseFrom.UpdateBrokenAvailableQuantity(productPhaseFrom.BrokenAvailableQuantity - quantity);
                productPhaseFrom.UpdateBrokenQuantity(productPhaseFrom.BrokenQuantity - quantity);
                break;
        }

        if (productPhaseTo == null)
        {
            productPhaseTo = ProductPhase.Create(
                request.updateQuantityStatusRequest.ProductId,
                request.updateQuantityStatusRequest.PhaseIdTo,
                quantity,
                request.updateQuantityStatusRequest.To,
                request.updateQuantityStatusRequest.CompanyIdTo
            );
            _productPhaseRepository.AddProductPhase(productPhaseTo);
        }
        else
        {
            switch (to)
            {
                case QuantityType.QUANTITY:
                    productPhaseTo.UpdateAvailableQuantity(productPhaseTo.AvailableQuantity + quantity);
                    productPhaseTo.UpdateQuantity(productPhaseTo.Quantity + quantity);
                    break;
                case QuantityType.ERROR_QUANTITY:
                    productPhaseTo.UpdateErrorAvailableQuantity(productPhaseTo.ErrorAvailableQuantity + quantity);
                    productPhaseTo.UpdateErrorQuantity(productPhaseTo.ErrorQuantity + quantity);
                    break;
                case QuantityType.FAILURE_QUANTITY:
                    productPhaseTo.UpdateFailureAvailableQuantity(productPhaseTo.FailureAvailabeQuantity + quantity);
                    productPhaseTo.UpdateFailureQuantity(productPhaseTo.FailureQuantity + quantity);
                    break;
                case QuantityType.BROKEN_QUANTITY:
                    productPhaseTo.UpdateBrokenAvailableQuantity(productPhaseTo.BrokenAvailableQuantity + quantity);
                    productPhaseTo.UpdateBrokenQuantity(productPhaseTo.BrokenQuantity + quantity);
                    break;
            }
            _productPhaseRepository.UpdateProductPhase(productPhaseTo);
        }

        _productPhaseRepository.UpdateProductPhase(productPhaseFrom);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success.Update();
    }
}
