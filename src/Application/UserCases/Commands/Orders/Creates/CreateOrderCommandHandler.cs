using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Order.Creates;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UserCases.Commands.Orders.Creates;

public sealed class CreateOrderCommandHandler
    (IOrderRepository _orderRepository,
    ICompanyRepository _companyRepository,
    IValidator<CreateOrderRequest> _validator,
    IUnitOfWork _unitOfWork
    ) : ICommandHandler<CreateOrderCommand>
{
    public async Task<Result.Success> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request.CreateOrderRequest);
        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }

        var oder = Order.Create(request.CreateOrderRequest, request.CreatedBy);

        _orderRepository.AddOrder(oder);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success.Create();
    }
}
