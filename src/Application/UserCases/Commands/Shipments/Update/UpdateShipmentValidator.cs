using Application.Abstractions.Data;
using Application.Utils;
using Contract.Services.Material.Share;
using Contract.Services.ProductPhase.ShareDto;
using Contract.Services.Shipment.Share;
using Contract.Services.Shipment.Update;
using Contract.Services.ShipmentDetail.Share;
using FluentValidation;

namespace Application.UserCases.Commands.Shipments.Update;

public class UpdateShipmentValidator : AbstractValidator<UpdateShipmentRequest>
{
    public UpdateShipmentValidator(
        IShipmentRepository shipmentRepository,
        ICompanyRepository companyRepository,
        IUserRepository userRepository)
    {
        RuleFor(req => req.ShipmentId)
            .MustAsync(async (shipmentId, _) =>
            {
                return await shipmentRepository.IsShipmentIdExistAndNotAcceptedAsync(shipmentId);
            }).WithMessage("Không tìm thấy đơn hàng tương ứng hoặc đơn hàng đã được chốt");

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
    }
}
