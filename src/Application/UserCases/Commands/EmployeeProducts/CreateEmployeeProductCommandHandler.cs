
using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.EmployeeProduct.Creates;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using FluentValidation;

namespace Application.UserCases.Commands.EmployeeProducts;

public sealed class CreateEmployeeProductCommandHandler
    (IEmployeeProductRepository _employeeProductRepository,
    IValidator<CreateEmployeeProductRequest> _validator,
    IUnitOfWork _unitOfWork) : ICommandHandler<CreateEmployeeProductComand>
{
    public async Task<Result.Success> Handle(CreateEmployeeProductComand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request.createEmployeeProductRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }
        var quantityProducts = request.createEmployeeProductRequest.CreateQuantityProducts;
        var employeeProducts = new List<EmployeeProduct>();

        foreach (var quantityProduct in quantityProducts)
        {
            var employeeProduct = EmployeeProduct.Create(quantityProduct, request.createEmployeeProductRequest.SlotId, request.createEmployeeProductRequest.Date, request.createdBy);
            employeeProducts.Add(employeeProduct);
        }
        await _employeeProductRepository.AddRangeEmployeeProduct(employeeProducts);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success.Create();


    }
}
