using Application.Abstractions.Data;
using Application.Utils;
using Contract.Services.Shipment.Create;
using Contract.Services.ShipmentDetail.Share;
using FluentValidation;

namespace Application.UserCases.Commands.Shipments.Create;

public class CreateShipmentValidator : AbstractValidator<CreateShipmentRequest>
{
    public CreateShipmentValidator(
        ICompanyRepository companyRepository,
        IProductRepository productRepository, 
        IUserRepository userRepository)
    {
        RuleFor(req => req.FromId)
            .MustAsync(async (fromId, _) =>
            {
                return await companyRepository.IsCompanyNotCustomerCompanyAsync(fromId);
            }).WithMessage("Cơ sở hay công ty bên thứ 3 không tồn tại");

        RuleFor(req => req.ToId)
            .Must((req, toId, _) =>
            {
                return req.FromId != toId;
            }).WithMessage("Công ty gửi phải khác công ty nhận");

        RuleFor(req => req.ToId)
            .MustAsync(async (ToId, _) =>
            {
                return await companyRepository.IsCompanyNotCustomerCompanyAsync(ToId);
            }).WithMessage("Cơ sở hay công ty bên thứ 3 không tồn tại");

        RuleFor(req => req.ShipperId)
            .NotEmpty().WithMessage("Không được để trống người giao hàng")
            .Matches(@"^\d{12}$").WithMessage("Id must be exactly 12 digits")
            .MustAsync(async (id, _) =>
            {
                return await userRepository.IsShipperExistAsync(id);
            }).WithMessage("Người giao hàng không tồn tại");

        RuleFor(req => req.ShipDate)
            .NotEmpty().WithMessage("Không được để trống ngày giao hàng")
            .Must((req, shipDate) =>
            {
                return DateUtil.FromDateTimeClientToDateTimeUtc(shipDate) >= DateTime.UtcNow;
            }).WithMessage("Ngày giao hàng không được trước ngày hiện tại");

        RuleForEach(req => req.ShipmentDetailRequests)
            .NotNull().WithMessage("Vật phẩm giao không được để trống")
            .Must((shipmentDetailRequest) =>
            {
                return Enum.IsDefined(typeof(KindOfShip), shipmentDetailRequest);
            }).WithMessage("Loại đơn hàng không tồn tại")
            .Must((shipmentDetailRequest) =>
            {
                return shipmentDetailRequest.Quantity > 0;
            }).WithMessage("Số lượng hàng hóa phải lớn hơn 0")
            .Must((shipmentDetailRequest) =>
            {
                if (shipmentDetailRequest.KindOfShip != KindOfShip.SHIP_FACTORY_PRODUCT)
                {
                    return true;
                }

                return shipmentDetailRequest.PhaseId != null;
            }).WithMessage("Sản phẩm phải có trạng thái")
            .Must((shipmentDetailRequest) =>
            {
                return shipmentDetailRequest?.ItemId != null;
            }).WithMessage("Mã vật phẩm không được để trống");

        RuleFor(req => req.ShipmentDetailRequests)
            .MustAsync(async (requests, _) =>
            {
                var shipProduct = requests
                .Where(s => s.KindOfShip == KindOfShip.SHIP_FACTORY_PRODUCT)
                .Select(s => s.ItemId)
                .ToList();

                if(shipProduct is null)
                {
                    return false;
                }

                return await productRepository.IsAllSubProductIdsExist(shipProduct);
            }).WithMessage("Có một vài mã sản phẩm không hợp lệ");

        RuleFor(req => req.ShipmentDetailRequests)
            .MustAsync(async (requests, _) =>
            {
                var shipMaterial = requests
                .Where(s => s.KindOfShip == KindOfShip.SHIP_FACTORY_MATERIAL)
                .Select(s => s.ItemId)
                .ToList();

                if (shipMaterial is null)
                {
                    return false;
                }

                return await setRepository.IsAllSetExistAsync(shipMaterial);
            }).WithMessage("Có một vài mã nguyên liệu không hợp lệ");
    }
}
