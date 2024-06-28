using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.ProductPhase.Creates;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UserCases.Commands.ProductPhases.Creates;

public sealed class CreateProductPhaseCommandHandler
    (IProductPhaseRepository _productPhaseRepository,
    IUnitOfWork _unitOfWork,
    IValidator<CreateProductPhaseRequest> _validator
    ) : ICommandHandler<CreateProductPhaseCommand>
{
    public async Task<Result.Success> Handle(CreateProductPhaseCommand request, CancellationToken cancellationToken)
    {
        var validator = await _validator.ValidateAsync(request.createProductPhaseRequest, cancellationToken);
        if (!validator.IsValid)
        {
            throw new MyValidationException(validator.ToDictionary());
        }
        var productPhase = ProductPhase.Create(request.createProductPhaseRequest);
        _productPhaseRepository.AddProductPhase(productPhase);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success.Create();
    }
}

