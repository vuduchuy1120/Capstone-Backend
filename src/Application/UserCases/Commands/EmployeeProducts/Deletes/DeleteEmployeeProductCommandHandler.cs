using Application.Abstractions.Data;
using Domain.Abstractions.Exceptions;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.EmployeeProduct.Deletes;
using Domain.Entities;
using FluentValidation;

namespace Application.UserCases.Commands.EmployeeProducts.Deletes
{
    public sealed class DeleteEmployeeProductCommandHandler : ICommandHandler<DeleteEmployeeProductCommand>
    {
        private readonly IEmployeeProductRepository _employeeProductRepository;
        private readonly IValidator<DeleteEmployeeProductRequest> _validator;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteEmployeeProductCommandHandler(
            IEmployeeProductRepository employeeProductRepository,
            IValidator<DeleteEmployeeProductRequest> validator,
            IUnitOfWork unitOfWork)
        {
            _employeeProductRepository = employeeProductRepository;
            _validator = validator;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result.Success> Handle(DeleteEmployeeProductCommand request, CancellationToken cancellationToken)
        {
            // Validate the request
            var validationResult = await _validator.ValidateAsync(request.DeleteEmployeeProductRequest, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new MyValidationException(validationResult.ToDictionary());
            }

            var deleteRequests = request.DeleteEmployeeProductRequest.DeleteQuantityProductRequests;

            // Extract composite keys
            var keys = deleteRequests.Select(r => new CompositeKey
            {
                Date = r.Date,
                SlotId = r.SlotId,
                ProductId = r.ProductId,
                PhaseId = r.PhaseId,
                UserId = r.UserId
            }).ToList();

            // Check if all employee products exist
            bool allExist = await _employeeProductRepository.IsAllEmployeeProductExistAsync(keys);
            if (!allExist)
            {
                throw new MyValidationException("One or more EmployeeProduct records do not exist.");
            }

            // Delete employee products
            var employeeProductsToDelete = await _employeeProductRepository.GetEmployeeProductsByKeysAsync(keys);
            _employeeProductRepository.DeleteRangeEmployeeProduct(employeeProductsToDelete);

            // Save changes to the database
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success.Delete();
        }
       
    }
}
