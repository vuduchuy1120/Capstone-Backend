using Application.Abstractions.Data;
using Application.Utils;
using Contract.Services.EmployeeProduct.Creates;
using Domain.Abstractions.Exceptions;
using FluentValidation;

namespace Application.UserCases.Commands.EmployeeProducts.Creates
{
    public sealed class CreateEmployeeProductRequestValidator : AbstractValidator<CreateEmployeeProductRequest>
    {
        public CreateEmployeeProductRequestValidator(IProductRepository productRepository, IPhaseRepository phaseRepository, ISlotRepository slotRepository, IUserRepository userRepository)
        {
            RuleFor(req => req.Date)
                .NotEmpty().WithMessage("Date is required")
                .Matches(@"^\d{2}/\d{2}/\d{4}$").WithMessage("Date must be in the format dd/MM/yyyy")
                .Must(BeAValidDate).WithMessage("Date must be a valid date in the format dd/MM/yyyy")
                .Must(date =>
                {
                    return DateUtil.ConvertStringToDateTimeOnly(date) <= DateOnly.FromDateTime(DateTime.Now);
                }).WithMessage("Date must be less than or equal to today");

            RuleFor(req => req.SlotId)
                .NotEmpty().WithMessage("SlotId is required")
                .MustAsync(async (slotId, cancellationToken) =>
                {
                    return await slotRepository.IsSlotExisted(slotId);
                }).WithMessage("SlotId is invalid");
            RuleForEach(req => req.CreateQuantityProducts)
                    .NotEmpty().WithMessage("CreateQuantityProductRequest is required")
                    .Must(createQuantityProductRequest =>
                    {
                        return createQuantityProductRequest.Quantity > 0;
                    }).WithMessage("Quantity must be greater than 0");
            RuleFor(req => req.CreateQuantityProducts)
                .MustAsync(async (createQuantityProducts, cancellationToken) =>
                {
                    var userIds = createQuantityProducts.Select(c => c.UserId).Distinct().ToList();
                    return await userRepository.IsAllUserActiveAsync(userIds);
                }).WithMessage("UserIds are invalid");
            RuleFor(req => req.CreateQuantityProducts)
                .MustAsync(async (createQuantityProducts, cancellationToken) =>
                {
                    var productIds = createQuantityProducts.Select(c => c.ProductId).Distinct().ToList();
                    return await productRepository.IsAllProductIdsExistAsync(productIds);
                }).WithMessage("ProductIds are invalid");
            RuleFor(req => req.CreateQuantityProducts)
                .MustAsync(async (createQuantityProducts, cancellationToken) =>
                {
                    var phaseIds = createQuantityProducts.Select(c => c.PhaseId).Distinct().ToList();
                    return await phaseRepository.IsAllPhaseExistByIdAsync(phaseIds);
                }).WithMessage("PhaseIds are invalid");

        }

        private bool BeAValidDate(string date)
        {
            try
            {
                return DateTime.TryParseExact(date, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out _);
            }
            catch (ArgumentException)
            {
                throw new MyValidationException("Date must be a valid date in the format dd/MM/yyyy");
            }

        }
    }

}
