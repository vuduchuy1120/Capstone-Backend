using Application.Abstractions.Data;
using Contract.Services.Product.CreateProduct;
using FluentValidation;

namespace Application.UserCases.Commands.Products.CreateProduct
{
    public class CreateProductValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductValidator(IProductRepository _productRepository)
        {
            RuleFor(req => req.Code)
                .NotEmpty().WithMessage("Mã sản phẩm không được trống")
                .Matches(@"^[a-zA-Z]{2}\d+$").WithMessage("Mã sản phẩm phải bắt đầu bằng hai ký tự, theo sau là số")
                .MustAsync(async (code, _) => !await _productRepository.IsProductCodeExist(code))
                .WithMessage("Mã sản phẩm đã tồn tại");

            RuleFor(req => req.PriceFinished)
                .Must((request, PriceFinished) =>
                {
                    return PriceFinished >= request.PricePhase1 + request.PricePhase2;
                }).WithMessage("Giá hàng hoàn thiện phải  >= tổng giá của 2 giai đoạn");

            RuleFor(req => req.PricePhase1)
                .GreaterThan(0).WithMessage("Giá phải lớn hơn 0");
            RuleFor(req => req.PricePhase2)
                .GreaterThan(0).WithMessage("Giá phải lơn hơn 0");

            RuleFor(req => req.Size)
                .NotEmpty().WithMessage("Kích thước sản phẩm không được để trống");

            RuleFor(req => req.Name)
                .NotEmpty().WithMessage("Tên sản phẩm không được để trống");

            RuleFor(req => req.ImageRequests)
                .Must(imageRequests =>
                {
                    if (imageRequests is null || imageRequests.Count == 0)
                    {
                        return false;
                    }
                    return imageRequests.Count(image => image.IsMainImage == true) == 1;
                }).WithMessage("Sản phẩm phải có 1 ảnh chính");
        }
    }
}
