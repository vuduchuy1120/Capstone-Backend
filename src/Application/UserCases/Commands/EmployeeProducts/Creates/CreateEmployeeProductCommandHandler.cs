using Application.Abstractions.Data;
using Application.Utils;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.EmployeeProduct.Creates;
using Contract.Services.ProductPhase.Creates;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using Domain.Exceptions.Users;
using FluentValidation;

namespace Application.UserCases.Commands.EmployeeProducts.Creates;

public sealed class CreateEmployeeProductCommandHandler
    (IEmployeeProductRepository _employeeProductRepository,
    IUserRepository _userRepository,
    ICompanyRepository _companyRepository,
    IProductPhaseRepository _productPhaseRepository,
    IValidator<CreateEmployeeProductRequest> _validator,
    IUnitOfWork _unitOfWork) : ICommandHandler<CreateEmployeeProductComand>
{
    public async Task<Result.Success> Handle(CreateEmployeeProductComand request, CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await _validator.ValidateAsync(request.createEmployeeProductRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }

        var slotId = request.createEmployeeProductRequest.SlotId;
        var date = DateUtil.ConvertStringToDateTimeOnly(request.createEmployeeProductRequest.Date);
        var roleName = request.roleNameClaim;
        var companyId = request.companyIdClaim;

        // Permission check
        if (roleName != "MAIN_ADMIN" && companyId != request.createEmployeeProductRequest.CompanyId)
        {
            var isUserValid = await _userRepository
                .IsAllUserActiveByCompanyId(
                request.createEmployeeProductRequest.CreateQuantityProducts
                .Select(c => c.UserId).Distinct().ToList(), companyId);
            if (!isUserValid)
                throw new UserNotPermissionException("You don't have permission to create employee products for other companies.");
        }

        var quantityProducts = request.createEmployeeProductRequest.CreateQuantityProducts;

        // Group by ProductId and PhaseId
        var groupedByProductAndPhase = quantityProducts.GroupBy(qp => new { qp.ProductId, qp.PhaseId })
            .Select(g => (g.Key.ProductId, g.Key.PhaseId, g.Sum(x => x.Quantity)))
            .ToList();

        var company = request.createEmployeeProductRequest.CompanyId;


        // Deleting existing EmployeeProducts
        var empProDeletes = await _employeeProductRepository.GetEmployeeProductsByDateAndSlotId(slotId, date, request.createEmployeeProductRequest.CompanyId);
        var groupedByProductAndPhaseDelete = empProDeletes.GroupBy(qp => new { qp.ProductId, qp.PhaseId })
            .Select(g => (g.Key.ProductId, g.Key.PhaseId, g.Sum(x => x.Quantity)))
            .ToList();

        var phaseProductsNew = new List<ProductPhase>();
        var phaseProductsUpdate = new Dictionary<(Guid ProductId, Guid PhaseId), ProductPhase>();

        if (empProDeletes.Any())
        {
            _employeeProductRepository.DeleteRangeEmployeeProduct(empProDeletes);
            await UpdateProductPhaseQuantities(groupedByProductAndPhaseDelete, company, phaseProductsUpdate, decrement: true);
        }

        var employeeProducts = new List<EmployeeProduct>();

        foreach (var quantityProduct in quantityProducts)
        {
            var employeeProduct = EmployeeProduct.Create(quantityProduct, request.createEmployeeProductRequest.SlotId, request.createEmployeeProductRequest.Date, request.createdBy);
            employeeProducts.Add(employeeProduct);
        }

        _employeeProductRepository.AddRangeEmployeeProduct(employeeProducts);
        await UpdateProductPhaseQuantities(groupedByProductAndPhase, company, phaseProductsUpdate, phaseProductsNew);

        if (phaseProductsNew.Any())
            _productPhaseRepository.AddProductPhaseRange(phaseProductsNew);
        if (phaseProductsUpdate.Any())
            _productPhaseRepository.UpdateProductPhaseRange(phaseProductsUpdate.Values.ToList());

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success.Create();
    }
    private async Task UpdateProductPhaseQuantities(
     List<(Guid ProductId, Guid PhaseId, int Quantity)> groupedByProductAndPhase,
     Guid companyId,
     Dictionary<(Guid ProductId, Guid PhaseId), ProductPhase> phaseProductsUpdate,
     List<ProductPhase> phaseProductsNew = null,
     bool decrement = false)
    {
        foreach (var item in groupedByProductAndPhase)
        {
            var key = (item.ProductId, item.PhaseId);
            if (!phaseProductsUpdate.TryGetValue(key, out var productPhase))
            {
                productPhase = await _productPhaseRepository.GetByProductIdPhaseIdCompanyID(item.ProductId, item.PhaseId, companyId);
                if (productPhase == null)
                {
                    productPhase = ProductPhase.Create(new CreateProductPhaseRequest
                    (
                        ProductId: item.ProductId,
                        PhaseId: item.PhaseId,
                        Quantity: item.Quantity,
                        AvailableQuantity: item.Quantity,
                        CompanyId: companyId
                    ));
                    phaseProductsNew?.Add(productPhase);
                }
                else
                {
                    phaseProductsUpdate[key] = productPhase;
                }
            }

            // Chỉ cập nhật nếu productPhase đã tồn tại trước đó
            if (phaseProductsUpdate.ContainsKey(key))
            {
                productPhase.Quantity += decrement ? -item.Quantity : item.Quantity;
                productPhase.AvailableQuantity += decrement ? -item.Quantity : item.Quantity;
            }
        }
    }
}