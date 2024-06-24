using Application.Abstractions.Data;
using Contract.Services.Shipment.Create;
using Contract.Services.ShipmentDetail.Share;
using FluentValidation;

namespace Application.UserCases.Commands.Shipments.Create;

public class CreateShipmentValidator : AbstractValidator<CreateShipmentRequest>
{
    public CreateShipmentValidator(
        ICompanyRepository companyRepository,
        IProductRepository productRepository, 
        ISetRepository setRepository,
        IUserRepository userRepository)
    {
        RuleFor(req => req.FromId)
            .MustAsync(async (fromId, _) =>
            {
                // check company is not CUSTOMER_COMPANY
                return await companyRepository.IsCompanyExistAsync(fromId);
            }).WithMessage("Công ty không tồn tại");

        RuleFor(req => req.ToId)
            .Must((req, toId, _) =>
            {
                return req.FromId != toId;
            }).WithMessage("Công ty gửi phải khác công ty nhận");

        RuleFor(req => req.ToId)
            .MustAsync(async (ToId, _) =>
            {
                return await companyRepository.IsCompanyExistAsync(ToId);
            }).WithMessage("Công ty không tồn tại");

        RuleFor(req => req.ShipperId)
            .NotEmpty().WithMessage("Không được để trống người giao hàng")
            .Matches(@"^\d{12}$").WithMessage("Id must be exactly 12 digits")
            .MustAsync(async (id, _) =>
            {
                //check is id input is ShipperID
                return await userRepository.IsUserActiveAsync(id);
            }).WithMessage("Người giao hàng không tồn tại");

        RuleFor(req => req.ShipDate)
            .NotEmpty().WithMessage("Không được để trống ngày giao hàng")
            .Must((req, shipDate) =>
            {
                return shipDate >= DateTime.UtcNow;
            }).WithMessage("Ngày giao hàng không được trước ngày hiện tại");

        RuleForEach(req => req.ShipmentDetailRequests)
            .NotNull().WithMessage("Vật phẩm giao không được để trống")
            .Must((shipmentDetailRequest) =>
            {
                return shipmentDetailRequest.KindOfShip == KindOfShip.SHIP_FACTORY_MATERIAL
                        || shipmentDetailRequest.KindOfShip == KindOfShip.SHIP_FACTORY_SET
                        || shipmentDetailRequest.KindOfShip == KindOfShip.SHIP_FACTORY_PRODUCT;
            }).WithMessage("Loại đơn giao phải là giao hàng cho các cơ sở")
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
                var shipSet = requests
                .Where(s => s.KindOfShip == KindOfShip.SHIP_FACTORY_SET)
                .Select(s => s.ItemId)
                .ToList();

                if (shipSet is null)
                {
                    return false;
                }

                return await setRepository.IsAllSetExistAsync(shipSet);
            }).WithMessage("Có một vài mã bộ không hợp lệ");

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
