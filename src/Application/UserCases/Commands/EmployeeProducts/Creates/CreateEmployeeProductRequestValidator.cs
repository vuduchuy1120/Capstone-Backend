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
                .NotEmpty().WithMessage("Ngày là bắt buộc")
                .NotNull().WithMessage("Ngày không được bỏ trống")
                .Matches(@"^\d{2}/\d{2}/\d{4}$").WithMessage("Ngày phải theo định dạng dd/MM/yyyy")
                .Must(BeAValidDate).WithMessage("Ngày không hợp lệ.")
                .Must(date =>
                {
                    return DateUtil.ConvertStringToDateTimeOnly(date) <= DateOnly.FromDateTime(DateTime.Now);
                }).WithMessage("Ngày phải nhỏ hơn hoặc bằng hôm nay");

            RuleFor(req => req.SlotId)
                .NotEmpty().WithMessage("SlotId là bắt buộc")
                .NotNull().WithMessage("SlotId không được bỏ trống")
                .MustAsync(async (slotId, cancellationToken) =>
                {
                    return await slotRepository.IsSlotExisted(slotId);
                }).WithMessage("SlotId không hợp lệ");

            RuleForEach(req => req.CreateQuantityProducts)
                .NotEmpty().WithMessage("CreateQuantityProductRequest là bắt buộc")
                .NotNull().WithMessage("CreateQuantityProductRequest không được bỏ trống")
                .Must(createQuantityProductRequest =>
                {
                    return createQuantityProductRequest.Quantity > 0;
                }).WithMessage("Số lượng phải lớn hơn 0");

            RuleFor(req => req.CreateQuantityProducts)
                .MustAsync(async (createQuantityProducts, cancellationToken) =>
                {
                    var userIds = createQuantityProducts.Select(c => c.UserId).Distinct().ToList();
                    return await userRepository.IsAllUserActiveAsync(userIds);
                }).WithMessage("UserIds không hợp lệ");

            RuleFor(req => req.CreateQuantityProducts)
                .MustAsync(async (createQuantityProducts, cancellationToken) =>
                {
                    var productIds = createQuantityProducts.Select(c => c.ProductId).Distinct().ToList();
                    return await productRepository.IsAllProductIdsExistAsync(productIds);
                }).WithMessage("ProductIds không hợp lệ");

            RuleFor(req => req.CreateQuantityProducts)
                .MustAsync(async (createQuantityProducts, cancellationToken) =>
                {
                    var phaseIds = createQuantityProducts.Select(c => c.PhaseId).Distinct().ToList();
                    return await phaseRepository.IsAllPhaseExistByIdAsync(phaseIds);
                }).WithMessage("PhaseIds không hợp lệ");

            RuleFor(req => req.CreateQuantityProducts)
                .MustAsync(async (request, createQuantityProducts, cancellationToken) =>
                {
                    var userIds = createQuantityProducts.Select(c => c.UserId).Distinct().ToList();
                    return await userRepository.IsAllUserActiveByCompanyId(userIds, request.CompanyId);
                }).WithMessage("Một hoặc nhiều người dùng thuộc công ty khác, bạn không có quyền tạo sản phẩm cho nhân viên đó");
        }

        private bool BeAValidDate(string date)
        {
            try
            {
                return DateTime.TryParseExact(date, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out _);
            }
            catch (ArgumentException)
            {
                throw new MyValidationException("Ngày không hợp lệ.");
            }
        }
    }
}
