using Application.Abstractions.Data;
using Contract.Services.EmployeeProduct.Deletes;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UserCases.Commands.EmployeeProducts.Deletes
{
    public sealed class DeleteEmployeeProductRequestValidator : AbstractValidator<DeleteEmployeeProductRequest>
    {
        public DeleteEmployeeProductRequestValidator(IUserRepository userRepository, IProductRepository productRepository, IPhaseRepository phaseRepository, ISlotRepository slotRepository)
        {
            RuleForEach(req => req.DeleteQuantityProductRequests)
                .NotEmpty().WithMessage("DeleteQuantityProductRequest is required");

            RuleForEach(req => req.DeleteQuantityProductRequests)
                .Must(deleteQuantityProductRequest =>
                {
                    return BeAValidDate(deleteQuantityProductRequest.Date);
                }).WithMessage("Date must be a valid date in the format dd/MM/yyyy");

            RuleForEach(req => req.DeleteQuantityProductRequests)
                .MustAsync(async (deleteQuantityProductRequest, cancellationToken) =>
                {
                    var slotIds = new List<int> { deleteQuantityProductRequest.SlotId };
                    return await slotRepository.IsAllSlotExist(slotIds);
                }).WithMessage("SlotId is invalid");

            RuleForEach(req => req.DeleteQuantityProductRequests)
                .MustAsync(async (deleteQuantityProductRequest, cancellationToken) =>
                {
                    var userIds = new List<string> { deleteQuantityProductRequest.UserId };
                    return await userRepository.IsAllUserActiveAsync(userIds);
                }).WithMessage("UserId is invalid");

            RuleForEach(req => req.DeleteQuantityProductRequests)
                .MustAsync(async (deleteQuantityProductRequest, cancellationToken) =>
                {
                    var productIds = new List<Guid> { deleteQuantityProductRequest.ProductId };
                    return await productRepository.IsAllProductIdsExistAsync(productIds);
                }).WithMessage("ProductId is invalid");

            RuleForEach(req => req.DeleteQuantityProductRequests)
                .MustAsync(async (deleteQuantityProductRequest, cancellationToken) =>
                {
                    var phaseIds = new List<Guid> { deleteQuantityProductRequest.PhaseId };
                    return await phaseRepository.IsAllPhaseExistByIdAsync(phaseIds);
                }).WithMessage("PhaseId is invalid");
        }

        private bool BeAValidDate(string date)
        {
            return DateTime.TryParseExact(date, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out _);
        }
    }
}
