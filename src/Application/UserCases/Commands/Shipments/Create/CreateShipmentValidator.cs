using Application.Abstractions.Data;
using Application.Utils;
using Contract.Services.Material.Share;
using Contract.Services.ProductPhase.ShareDto;
using Contract.Services.Shipment.Create;
using Contract.Services.ShipmentDetail.Share;
using FluentValidation;

namespace Application.UserCases.Commands.Shipments.Create;

public class CreateShipmentValidator : AbstractValidator<CreateShipmentRequest>
{
    public CreateShipmentValidator(
        ICompanyRepository companyRepository,
        IMaterialRepository materialRepository,
        IProductPhaseRepository productPhaseRepository,
        IUserRepository userRepository)
    {
        RuleFor(req => req.FromId)
            .MustAsync(async (fromId, _) =>
            {
                return await companyRepository.IsCompanyNotCustomerCompanyAsync(fromId);
            }).WithMessage("Cơ sở hay công ty bên thứ 3 không tồn tại");

        RuleFor(req => req.ToId)
            .Must((req, toId) => req.FromId != toId)
            .WithMessage("Công ty gửi phải khác công ty nhận")
            .MustAsync(async (req, toId, _) =>
            {
                var companyTypes = await companyRepository.GetCompanyTypeByCompanyIdsAsync(new List<Guid> { toId, req.FromId });
                if (companyTypes.Count != 2) return false;
                return companyTypes[0] != companyTypes[1];
            }).WithMessage("Công ty nhận không được cùng loại với công ty gửi");

        RuleFor(req => req.ToId)
            .MustAsync(async (ToId, _) =>
            {
                return await companyRepository.IsCompanyNotCustomerCompanyAsync(ToId);
            }).WithMessage("Cơ sở hay công ty bên thứ 3 không tồn tại");

        RuleFor(req => req.ShipperId)
            .NotEmpty().WithMessage("Người giao hàng không được để trống")
            .Matches(@"^\d{9}$|^\d{12}$").WithMessage("ID người giao hàng phải là 9 hoặc 12 chữ số")
            .MustAsync(async (id, _) =>
            {
                return await userRepository.IsShipperExistAsync(id);
            }).WithMessage("Người giao hàng không tồn tại");

        RuleFor(req => req.ShipDate)
            .NotEmpty().WithMessage("Không được để trống ngày giao hàng")
            .Must((req, shipDate) =>
            {
                var clientDate = DateUtil.FromDateTimeClientToDateTimeUtc(shipDate);
                var now = DateTime.UtcNow;
                return DateUtil.FromDateTimeClientToDateTimeUtc(shipDate) >= DateTime.UtcNow;
            }).WithMessage("Ngày giao hàng không được trước ngày hiện tại");

        RuleForEach(req => req.ShipmentDetailRequests)
            .NotNull().WithMessage("Vật phẩm giao không được để trống")
            .Must((shipmentDetailRequest) =>
            {
                return Enum.IsDefined(typeof(KindOfShip), shipmentDetailRequest.KindOfShip);
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
            .MustAsync(async (req, requests, _) =>
            {
                var shipProduct = requests
                    .Where(request => request.KindOfShip == KindOfShip.SHIP_FACTORY_PRODUCT && request.PhaseId != null)
                    .Select(request => new CheckQuantityInstockEnoughRequest(
                        request.ItemId,
                        (Guid)request.PhaseId,
                        req.FromId,
                        (int)request.Quantity))
                    .ToList();

                var duplicateGroups = shipProduct
                    .GroupBy(p => new { p.ProductId, p.PhaseId, p.FromCompanyId })
                    .Where(g => g.Count() > 1)
                    .ToList();

                if (duplicateGroups.Any())
                {
                    return false;
                }

                //if (shipProduct is null || shipProduct.Count == 0)
                //{
                //    return true;
                //}

                return true;
            }).WithMessage("Có một vài mã sản phẩm không hợp lệ hoặc không đủ số lượng trong kho");

        RuleFor(req => req.ShipmentDetailRequests)
            .MustAsync(async (requests, _) =>
            {
                var shipMaterial = requests
                .Where(s => s.KindOfShip == KindOfShip.SHIP_FACTORY_MATERIAL)
                .Select(s => new MaterialCheckQuantityRequest(s.ItemId, s.Quantity))
                .ToList();

                var duplicateGroups = shipMaterial
                    .GroupBy(p => new { p.id })
                    .Where(g => g.Count() > 1)
                    .ToList();

                if (duplicateGroups.Any())
                {
                    return false;
                }

                if (shipMaterial is null || shipMaterial.Count == 0)
                {
                    return true;
                }

                return  await materialRepository.IsMaterialEnoughAsync(shipMaterial);
            }).WithMessage("Có một vài mã nguyên liệu không hợp lệ hoặc trong kho không đủ");
    }
}
