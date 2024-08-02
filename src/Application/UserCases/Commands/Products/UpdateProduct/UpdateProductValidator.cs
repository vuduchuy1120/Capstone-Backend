using Application.Abstractions.Data;
using Contract.Services.Product.UpdateProduct;
using FluentValidation;

namespace Application.UserCases.Commands.Products.UpdateProduct;

public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator(IProductImageRepository productImageRepository)
    {
        RuleFor(req => req.UpdateProductRequest)
    .NotNull().WithMessage("Yêu cầu cập nhật sản phẩm không được để trống");

        RuleFor(req => req.UpdateProductRequest.Id)
            .NotEmpty().WithMessage("ID sản phẩm không được để trống");

        RuleFor(req => req.UpdateProductRequest.Code)
            .NotEmpty().WithMessage("Mã sản phẩm không được để trống")
            .Matches(@"^[a-zA-Z]{2}\d+$").WithMessage("Mã sản phẩm phải bắt đầu bằng hai ký tự và theo sau là các số");

        RuleFor(req => req.UpdateProductRequest.PricePhase1)
            .GreaterThan(0).WithMessage("Giá sản phẩm phải lớn hơn 0");

        RuleFor(req => req.UpdateProductRequest.PricePhase2)
            .GreaterThan(0).WithMessage("Giá sản phẩm phải lớn hơn 0");

        RuleFor(req => req.UpdateProductRequest.PriceFinished)
            .Must((request, PriceFinished) =>
            {
                return PriceFinished >= request.UpdateProductRequest.PricePhase1 + request.UpdateProductRequest.PricePhase2;
            }).WithMessage("Giá hàng hoàn thiện phải lớn hơn hoặc bằng tổng giá của hai giai đoạn");

        RuleFor(req => req.UpdateProductRequest.Size)
            .NotEmpty().WithMessage("Kích thước sản phẩm không được để trống");

        RuleFor(req => req.UpdateProductRequest.Name)
            .NotEmpty().WithMessage("Mô tả tên không được để trống");

        RuleFor(req => req.UpdateProductRequest.RemoveImageIds)
            .MustAsync(async (req, imageIds, _) =>
            {
                if (imageIds is null || imageIds.Count == 0)
                {
                    return true;
                }

                return await productImageRepository.IsAllImageIdExist(imageIds, req.ProductId);
            }).WithMessage("Có ít nhất một hình ảnh không tồn tại");

    }
}
