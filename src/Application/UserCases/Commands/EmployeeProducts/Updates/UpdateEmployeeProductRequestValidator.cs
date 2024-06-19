using Application.Abstractions.Data;
using Contract.Services.EmployeeProduct.Updates;
using FluentValidation;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UserCases.Commands.EmployeeProducts.UpdateEmployeeProduct
{
    public sealed class UpdateEmployeeProductRequestValidator : AbstractValidator<UpdateEmployeeProductRequest>
    {
        public UpdateEmployeeProductRequestValidator(IProductRepository productRepository, IPhaseRepository phaseRepository, ISlotRepository slotRepository, IUserRepository userRepository)
        {
            RuleForEach(req => req.UpdateQuantityProductRequests)
                .NotEmpty().WithMessage("UpdateQuantityProductRequest is required")
                .Must(updateQuantityProductRequest =>
                {
                    return updateQuantityProductRequest.Quantity > 0;
                }).WithMessage("Quantity must be greater than 0");

            RuleFor(req => req.UpdateQuantityProductRequests)
                .MustAsync(async (updateQuantityProductRequests, cancellationToken) =>
                {
                    var userIds = updateQuantityProductRequests.Select(c => c.UserId).Distinct().ToList();
                    return await userRepository.IsAllUserActiveAsync(userIds);
                }).WithMessage("UserIds are invalid");

            RuleFor(req => req.UpdateQuantityProductRequests)
                .MustAsync(async (updateQuantityProductRequests, cancellationToken) =>
                {
                    var productIds = updateQuantityProductRequests.Select(c => c.ProductId).Distinct().ToList();
                    return await productRepository.IsAllProductIdsExistAsync(productIds);
                }).WithMessage("ProductIds are invalid");

            RuleFor(req => req.UpdateQuantityProductRequests)
                .MustAsync(async (updateQuantityProductRequests, cancellationToken) =>
                {
                    var phaseIds = updateQuantityProductRequests.Select(c => c.PhaseId).Distinct().ToList();
                    return await phaseRepository.IsAllPhaseExistByIdAsync(phaseIds);
                }).WithMessage("PhaseIds are invalid");

            RuleForEach(req => req.UpdateQuantityProductRequests)
                .MustAsync(async (updateQuantityProductRequest, cancellationToken) =>
                {
                    return await slotRepository.IsSlotExisted(updateQuantityProductRequest.SlotId);
                }).WithMessage("SlotId is invalid");

            RuleForEach(req => req.UpdateQuantityProductRequests)
                .Must(updateQuantityProductRequest =>
                {
                    return BeAValidDate(updateQuantityProductRequest.Date);
                }).WithMessage("Date must be a valid date in the format dd/MM/yyyy");
        }

        private bool BeAValidDate(string date)
        {
            return DateTime.TryParseExact(date, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out _);
        }
    }
}
