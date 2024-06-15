using Application.Abstractions.Data;
using Domain.Abstractions.Exceptions;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.EmployeeProduct.Updates;
using Domain.Entities;
using FluentValidation;

namespace Application.UserCases.Commands.EmployeeProducts.Updates
{
    public sealed class UpdateEmployeeProductCommandHandler : ICommandHandler<UpdateEmployeeProductCommand>
    {
        private readonly IEmployeeProductRepository _employeeProductRepository;
        private readonly IValidator<UpdateEmployeeProductRequest> _validator;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateEmployeeProductCommandHandler(
            IEmployeeProductRepository employeeProductRepository,
            IValidator<UpdateEmployeeProductRequest> validator,
            IUnitOfWork unitOfWork)
        {
            _employeeProductRepository = employeeProductRepository;
            _validator = validator;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result.Success> Handle(UpdateEmployeeProductCommand request, CancellationToken cancellationToken)
        {
            // Validate the request
            var validationResult = await _validator.ValidateAsync(request.UpdateEmployeeProductRequest, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new MyValidationException(validationResult.ToDictionary());
            }

            var updateRequests = request.UpdateEmployeeProductRequest.UpdateQuantityProductRequests;

            // Extract composite keys
            var keys = updateRequests.Select(r => new CompositeKey
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

            // Fetch existing EmployeeProducts from the database
            var employeeProducts = await _employeeProductRepository.GetEmployeeProductsByKeysAsync(keys);

            // Update fields
            foreach (var updateRequest in updateRequests)
            {
                var employeeProduct = employeeProducts.FirstOrDefault(ep =>
                    ep.Date == ConvertStringToDateTimeOnly(updateRequest.Date) &&
                    ep.SlotId == updateRequest.SlotId &&
                    ep.ProductId == updateRequest.ProductId &&
                    ep.PhaseId == updateRequest.PhaseId &&
                    ep.UserId == updateRequest.UserId);

                if (employeeProduct != null)
                {
                    employeeProduct.Quantity = updateRequest.Quantity;
                    employeeProduct.IsMold = updateRequest.IsMold;
                    employeeProduct.UpdatedBy = request.UpdatedBy;
                    employeeProduct.UpdatedDate = DateTime.UtcNow; // assuming you have these fields
                }
            }

            // Save changes to the database
             _employeeProductRepository.UpdateRangeEmployeeProduct(employeeProducts);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success.Update();
        }
        private DateOnly ConvertStringToDateTimeOnly(string dateString)
        {
            string format = "dd/MM/yyyy";

            DateTime dateTime;
            if (DateTime.TryParseExact(dateString, format, null, System.Globalization.DateTimeStyles.None, out dateTime))
            {
                return new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);
            }
            else
            {
                throw new MyValidationException("Date is wrong format");
            }
        }
    }
}
