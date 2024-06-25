using Application.Abstractions.Data;
using Application.Utils;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.EmployeeProduct.Creates;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using Domain.Exceptions.Users;
using FluentValidation;

namespace Application.UserCases.Commands.EmployeeProducts.Creates;

public sealed class CreateEmployeeProductCommandHandler
    (IEmployeeProductRepository _employeeProductRepository,
    IUserRepository _userRepository,
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
        var slotId = request.createEmployeeProductRequest.SlotId;
        var date = DateUtil.ConvertStringToDateTimeOnly(request.createEmployeeProductRequest.Date);
        var roleName = request.roleNameClaim;
        var companyId = request.companyIdClaim;


        if (roleName != "MAIN_ADMIN" && companyId != request.createEmployeeProductRequest.CompanyId)
        {
            var isUserValid = await _userRepository
                .IsAllUserActiveByCompanyId(
                request
                .createEmployeeProductRequest
                .CreateQuantityProducts
                .Select(c => c.UserId).Distinct().ToList(), companyId);
            if(!isUserValid)
                throw new UserNotPermissionException("You dont have permission create employee product of other companyID");
        }

        var empProDeletes = await _employeeProductRepository.GetEmployeeProductsByDateAndSlotId(slotId, date, request.createEmployeeProductRequest.CompanyId);
        if (empProDeletes.Count > 0)
        {
            _employeeProductRepository.DeleteRangeEmployeeProduct(empProDeletes);
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
