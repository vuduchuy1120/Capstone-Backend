using Application.Abstractions.Data;
using Application.Utils;
using Contract.Services.ShipOrder.Create;
using Contract.Services.ShipOrder.Share;
using FluentValidation;

namespace Application.UserCases.Commands.ShipOrders.Create;

public class CreateShipOrderValidator : AbstractValidator<CreateShipOrderRequest>
{
    public CreateShipOrderValidator(IUserRepository userRepository, IOrderDetailRepository orderDetailRepository)
    {
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

        RuleFor(req => req.KindOfShipOrder)
            .Must((req, kind) => Enum.IsDefined(typeof(DeliveryMethod), kind))
            .WithMessage("Không tìm thấy loại giao hàng phù hợp");

        RuleFor(req => req.ShipOrderDetailRequests)
            .MustAsync(async (req, ShipOrderDetailRequests, _) =>
            {
                var productIds = ShipOrderDetailRequests
                                    .Where(r => r.ItemKind ==ItemKind.PRODUCT)
                                    .Select(r => r.ItemId)
                                    .ToList();

                if (productIds.Any())
                {
                    return true;
                }

                return await orderDetailRepository.IsAllOrderDetailProductIdsExistedAsync(req.OrderId, productIds);
            }).WithMessage("Trong các sản phẩm có sản phẩm không nằm trong đơn hàng đã đặt");

        RuleFor(req => req.ShipOrderDetailRequests)
            .Must((ShipOrderDetailRequests) =>
            {
                var productIdsDistinct = ShipOrderDetailRequests
                                    .Where(r => r.ItemKind == ItemKind.PRODUCT)
                                    .Select(r => r.ItemId)
                                    .Distinct()
                                    .ToList();

                var productIds = ShipOrderDetailRequests
                                    .Where(r => r.ItemKind == ItemKind.PRODUCT)
                                    .Select(r => r.ItemId)
                                    .ToList();

                return productIdsDistinct.Count() != productIds.Count();
            }).WithMessage("Trong các sản phẩm có sản phẩm bị lặp lại");

        RuleFor(req => req.ShipOrderDetailRequests)
           .MustAsync(async (req, ShipOrderDetailRequests, _) =>
           {
               var setIds = ShipOrderDetailRequests
                                   .Where(r => r.ItemKind == ItemKind.SET)
                                   .Select(r => r.ItemId)
                                   .ToList();

               if (setIds.Any())
               {
                   return true;
               }

               return await orderDetailRepository.IsAllOrderDetailSetIdsExistedAsync(req.OrderId, setIds);
           }).WithMessage("Trong các bộ có bộ không nằm trong đơn hàng đã đặt");

        RuleFor(req => req.ShipOrderDetailRequests)
           .Must((ShipOrderDetailRequests) =>
           {
               var setIdsDistinct = ShipOrderDetailRequests
                                   .Where(r => r.ItemKind == ItemKind.SET)
                                   .Select(r => r.ItemId)
                                   .Distinct()
                                   .ToList();

               var setIds = ShipOrderDetailRequests
                                   .Where(r => r.ItemKind == ItemKind.SET)
                                   .Select(r => r.ItemId)
                                   .ToList();

               return setIdsDistinct.Count() != setIds.Count();
           }).WithMessage("Trong các sản phẩm có sản phẩm bị lặp lại");


        RuleForEach(req => req.ShipOrderDetailRequests)
            .Must((req, ShipOrderDetailRequest) => Enum.IsDefined(typeof(ItemKind), ShipOrderDetailRequest.ItemKind))
            .WithMessage("Vật phẩm giao phải là sản phầm (0) hoặc bộ (1)");
    }
}
