using Application.Abstractions.Data;
using Contract.Services.EmployeeProduct.Creates;
using FluentValidation;

namespace Application.UserCases.Commands.EmployeeProducts.CreateEmployeeProduct
{
    public sealed class CreateEmployeeProductRequestValidator : AbstractValidator<CreateEmployeeProductRequest>
    {
        public CreateEmployeeProductRequestValidator(IProductRepository productRepository, IPhaseRepository phaseRepository, ISlotRepository slotRepository, IUserRepository userRepository)
        {
            RuleFor(req => req.Date)
                .NotEmpty().WithMessage("Date is required")
                .Matches(@"^\d{2}/\d{2}/\d{4}$").WithMessage("Date must be in the format dd/MM/yyyy")
                .Must(BeAValidDate).WithMessage("Date must be a valid date in the format dd/MM/yyyy");

            RuleForEach(req => req.CreateQuantityProducts)
                    .NotEmpty().WithMessage("CreateQuantityProductRequest is required")
                    .Must(createQuantityProductRequest =>
                    {
                        return createQuantityProductRequest.Quantity > 0;
                    }).WithMessage("Quantity must be greater than 0");

            RuleFor(req => req)
                .CustomAsync(async (request, context, cancellationToken) =>
                {
                    var userIds = request.CreateQuantityProducts.Select(c => c.UserId).Distinct().ToList();
                    var productIds = request.CreateQuantityProducts.Select(c => c.ProductId).Distinct().ToList();
                    var phaseIds = request.CreateQuantityProducts.Select(c => c.PhaseId).Distinct().ToList();
                    var slotId = request.SlotId;

                    var invalidUserIds = await GetInvalidUserIdsAsync(userIds, userRepository);
                    var invalidProductIds = await GetInvalidProductIdsAsync(productIds, productRepository);
                    var invalidPhaseIds = await GetInvalidPhaseIdsAsync(phaseIds, phaseRepository);
                    var isSlotValid = await slotRepository.IsSlotExisted(slotId);

                    foreach (var createQuantityProduct in request.CreateQuantityProducts)
                    {
                        if (invalidUserIds.Contains(createQuantityProduct.UserId))
                        {
                            context.AddFailure($"UserId {createQuantityProduct.UserId} is invalid or inactive");
                        }
                        if (invalidProductIds.Contains(createQuantityProduct.ProductId))
                        {
                            context.AddFailure($"ProductId {createQuantityProduct.ProductId} is invalid");
                        }
                        if (invalidPhaseIds.Contains(createQuantityProduct.PhaseId))
                        {
                            context.AddFailure($"PhaseId {createQuantityProduct.PhaseId} is invalid");
                        }
                    }

                    if (!isSlotValid)
                    {
                        context.AddFailure($"SlotId {request.SlotId} is invalid");
                    }
                });
        }

        private async Task<HashSet<string>> GetInvalidUserIdsAsync(List<string> userIds, IUserRepository userRepository)
        {
            var invalidUserIds = new HashSet<string>();

            foreach (var userId in userIds)
            {
                var isActive = await userRepository.IsUserActiveAsync(userId);
                if (!isActive)
                {
                    invalidUserIds.Add(userId);
                }
            }

            return invalidUserIds;
        }

        private async Task<HashSet<Guid>> GetInvalidProductIdsAsync(List<Guid> productIds, IProductRepository productRepository)
        {
            var invalidProductIds = new HashSet<Guid>();

            foreach (var productId in productIds)
            {
                var exists = await productRepository.IsProductIdExist(productId);
                if (!exists)
                {
                    invalidProductIds.Add(productId);
                }
            }

            return invalidProductIds;
        }

        private async Task<HashSet<Guid>> GetInvalidPhaseIdsAsync(List<Guid> phaseIds, IPhaseRepository pharseRepository)
        {
            var invalidPhaseIds = new HashSet<Guid>();

            foreach (var phaseId in phaseIds)
            {
                var exists = await pharseRepository.IsExistById(phaseId);
                if (!exists)
                {
                    invalidPhaseIds.Add(phaseId);
                }
            }

            return invalidPhaseIds;
        }
        private bool BeAValidDate(string date)
        {
            return DateTime.TryParseExact(date, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out _);
        }
    }

}
