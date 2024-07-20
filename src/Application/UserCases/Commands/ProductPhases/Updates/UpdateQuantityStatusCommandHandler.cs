using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.ProductPhases.Updates.ChangeQuantityStatus;
using Contract.Services.ProductPhase.ShareDto;
using Domain.Abstractions.Exceptions;
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
        var productPhase = await _productPhaseRepository.GetByProductIdPhaseIdAndCompanyIdAsync(request.updateQuantityStatusRequest.ProductId, request.updateQuantityStatusRequest.PhaseId, request.updateQuantityStatusRequest.CompanyId);

        var quantity = request.updateQuantityStatusRequest.Quantity;
        var from = request.updateQuantityStatusRequest.From;
        var to = request.updateQuantityStatusRequest.To;

        switch (from)
        {
            case QuantityType.QUANTITY:
                if (productPhase.AvailableQuantity < quantity)
                {
                    throw new MyValidationException("Số lượng không đủ để cập nhật.");
                }
                productPhase.UpdateAvailableQuantity(productPhase.AvailableQuantity - quantity);
                productPhase.UpdateQuantity(productPhase.Quantity - quantity);
                break;
            case QuantityType.ERROR_QUANTITY:
                if (productPhase.ErrorAvailableQuantity < quantity)
                {
                    throw new MyValidationException("Số lượng không đủ để cập nhật.");
                }
                productPhase.UpdateErrorAvailableQuantity(productPhase.ErrorAvailableQuantity - quantity);
                productPhase.UpdateErrorQuantity(productPhase.ErrorQuantity - quantity);
                break;
            case QuantityType.FAILURE_QUANTITY:
                if (productPhase.FailureAvailabeQuantity < quantity)
                {
                    throw new MyValidationException("Số lượng không đủ để cập nhật.");
                }
                productPhase.UpdateFailureAvailableQuantity(productPhase.FailureAvailabeQuantity - quantity);
                productPhase.UpdateFailureQuantity(productPhase.FailureQuantity - quantity);
                break;
            case QuantityType.BROKEN_QUANTITY:
                if (productPhase.BrokenAvailableQuantity < quantity)
                {
                    throw new MyValidationException("Số lượng không đủ để cập nhật.");
                }
                productPhase.UpdateBrokenAvailableQuantity(productPhase.BrokenAvailableQuantity - quantity);
                productPhase.UpdateBrokenQuantity(productPhase.BrokenQuantity - quantity);
                break;
        }

        switch (to)
        {
            case QuantityType.QUANTITY:
                productPhase.UpdateAvailableQuantity(productPhase.AvailableQuantity + quantity);
                productPhase.UpdateQuantity(productPhase.Quantity + quantity);
                break;
            case QuantityType.ERROR_QUANTITY:
                productPhase.UpdateErrorAvailableQuantity(productPhase.ErrorAvailableQuantity + quantity);
                productPhase.UpdateErrorQuantity(productPhase.ErrorQuantity + quantity);
                break;
            case QuantityType.FAILURE_QUANTITY:
                productPhase.UpdateFailureAvailableQuantity(productPhase.FailureAvailabeQuantity + quantity);
                productPhase.UpdateFailureQuantity(productPhase.FailureQuantity + quantity);
                break;
            case QuantityType.BROKEN_QUANTITY:
                productPhase.UpdateBrokenAvailableQuantity(productPhase.BrokenAvailableQuantity + quantity);
                productPhase.UpdateBrokenQuantity(productPhase.BrokenQuantity + quantity);
                break;
        }

        _productPhaseRepository.UpdateProductPhase(productPhase);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success.Update();
    }
}
