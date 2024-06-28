using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.ProductPhase.Updates;
using Domain.Abstractions.Exceptions;
using FluentValidation;

namespace Application.UserCases.Commands.ProductPhases.Updates;

public sealed class UpdateProductPhaseCommandHandler
    (IProductPhaseRepository _productPhaseRepository,
    IUnitOfWork _unitOfWork,
    IValidator<UpdateProductPhaseRequest> _validator
    ) : ICommandHandler<UpdateProductPhaseCommand>
{

    public async Task<Result.Success> Handle(UpdateProductPhaseCommand request, CancellationToken cancellationToken)
    {
        var validator = await _validator.ValidateAsync(request.updateProductPhaseRequest, cancellationToken);
        if (!validator.IsValid)
        {
            throw new MyValidationException(validator.ToDictionary());
        }
        var productPhase = await _productPhaseRepository.GetProductPhaseByPhaseIdAndProductId(request.updateProductPhaseRequest.PhaseId, request.updateProductPhaseRequest.ProductId);
        productPhase.Update(request.updateProductPhaseRequest);
        _productPhaseRepository.UpdateProductPhase(productPhase);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success.Update();
    }
}

