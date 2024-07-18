using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Shipment.Create;
using Contract.Services.ShipmentDetail.Share;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using Domain.Exceptions.Materials;
using Domain.Exceptions.ProductPhases;
using Domain.Exceptions.ShipmentDetails;
using Domain.Exceptions.Shipments;
using FluentValidation;
namespace Application.UserCases.Commands.Shipments.Create;

internal sealed class CreateShipmentCommandHandler(
    IShipmentRepository _shipmentRepository,
    IShipmentDetailRepository _shipmentDetailRepository,
    IProductPhaseRepository _productPhaseRepository,
    ICompanyRepository _companyRepository,
    IMaterialRepository _materialRepository,
    IUnitOfWork _unitOfWork,
    IValidator<CreateShipmentRequest> _validator) : ICommandHandler<CreateShipmentCommand>
{
    private List<ProductPhase> _products;

    public async Task<Result.Success> Handle(
        CreateShipmentCommand request,
        CancellationToken cancellationToken)
    {
        var createShipmentRequest = request.CreateShipmentRequest;
        await ValidateRequest(createShipmentRequest);

        var shipment = Shipment.Create(createShipmentRequest, request.CreatedBy);

        _products = new();

        var shipmentDetails = await CreateShipmentDetails(
            createShipmentRequest.ShipmentDetailRequests,
            shipment.Id,
            createShipmentRequest.FromId);

        _shipmentRepository.Add(shipment);
        _shipmentDetailRepository.AddRange(shipmentDetails);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success.Create();
    }

    private async Task ValidateRequest(CreateShipmentRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }
    }

    private async Task<List<ShipmentDetail>> CreateShipmentDetails(
        List<ShipmentDetailRequest> shipmentDetailRequests,
        Guid shipmentId,
        Guid fromCompany)
    {
        var isFromCompanyIsThirdPartyCompany = await _companyRepository.IsThirdPartyCompanyAsync(fromCompany);

        if (isFromCompanyIsThirdPartyCompany)
        {
            var uniqueItemIds = shipmentDetailRequests
                .Select(s => s.ItemId)
                .Distinct()
                .ToList();

            var productPhasesTasks = uniqueItemIds
            .Select(uniqueItem => _productPhaseRepository.GetByProductIdAndCompanyIdAsync(uniqueItem, fromCompany))
            .ToList();

            var productPhasesResults = await Task.WhenAll(productPhasesTasks);

            var productDictionary = uniqueItemIds
                .Zip(productPhasesResults, (itemId, productPhases) =>
                {
                    if (productPhases is null || productPhases.Count == 0)
                    {
                        throw new ProductPhaseNotFoundException();
                    }
                    return new { itemId, productPhases };
                })
                .ToDictionary(x => x.itemId, x => x.productPhases);

            var shipmentDetails = shipmentDetailRequests.Select(detailRequest =>
            {
                var productPhases = productDictionary.GetValueOrDefault(detailRequest.ItemId) ?? throw new ProductPhaseNotFoundException();
                return CreateShipmentDetailFromThirdPartyCompany(detailRequest, shipmentId, productPhases);
            });

            return shipmentDetails.ToList();
        }
        else
        {
            var shipmentDetails = new List<ShipmentDetail>();

            foreach(var request in shipmentDetailRequests)
            {
                var shipmentDetail = await CreateShipmentDetailFromFactory(request, shipmentId, fromCompany);
            }

            //var shipmentDetailTasks = shipmentDetailRequests
            //.Select(detailRequest => CreateShipmentDetailFromFactory(detailRequest, shipmentId, fromCompany));
            //var shipmentDetails = await Task.WhenAll(shipmentDetailTasks);

            return shipmentDetails.ToList();
        }
    }

    private ShipmentDetail CreateShipmentDetailFromThirdPartyCompany(ShipmentDetailRequest request, Guid shipmentId, List<ProductPhase> productPhases)
    {
        if (request.KindOfShip != KindOfShip.SHIP_FACTORY_PRODUCT)
        {
            if (request.KindOfShip == KindOfShip.SHIP_FACTORY_MATERIAL)
            {
                throw new ShipmentBadRequestException("Công ty hợp tác bên thứ 3 không gửi được nguyên liệu");
            }
            else
            {
                throw new KindOfShipNotFoundException();
            }
        }

        var totalQuantity = productPhases.Sum(ph => ph.AvailableQuantity + ph.ErrorAvailableQuantity);

        if (request.Quantity > totalQuantity)
        {
            throw new ItemAvailableNotEnoughException();
        }

        int remainingQuantity = (int)request.Quantity;

        foreach (var ph in productPhases)
        {
            remainingQuantity = UpdateQuantity(ph, remainingQuantity);
            if (remainingQuantity == 0)
            {
                break;
            }
        }

        _productPhaseRepository.UpdateProductPhaseRange(productPhases);

        return ShipmentDetail.CreateShipmentProductDetail(shipmentId, request);
    }

    private int UpdateQuantity(ProductPhase ph, int remainingQuantity)
    {
        if (ph.AvailableQuantity > 0)
        {
            int quantityToDeduct = Math.Min(remainingQuantity, ph.AvailableQuantity);
            ph.UpdateAvailableQuantity(ph.AvailableQuantity - quantityToDeduct);
            remainingQuantity -= quantityToDeduct;
        }

        if (remainingQuantity > 0 && ph.ErrorAvailableQuantity > 0)
        {
            int quantityToDeduct = Math.Min(remainingQuantity, ph.ErrorAvailableQuantity);
            ph.UpdateErrorAvailableQuantity(ph.ErrorAvailableQuantity - quantityToDeduct);
            remainingQuantity -= quantityToDeduct;
        }

        return remainingQuantity;
    }


    private async Task<ShipmentDetail> CreateShipmentDetailFromFactory(ShipmentDetailRequest request, Guid shipmentId, Guid fromCompany)
    {
        switch (request.KindOfShip)
        {
            case KindOfShip.SHIP_FACTORY_PRODUCT:
                // chi gui duoc hang NO_PROBLEM va hang THIRD_PARTY_ERROR
                var phaseId = request.PhaseId ?? throw new ProductPhaseNotFoundException();

                var productPhase = _products
                    .Where(p => p.PhaseId == phaseId && p.CompanyId == fromCompany && p.ProductId == request.ItemId)
                    .FirstOrDefault();

                if (productPhase == null)
                {
                    productPhase = await _productPhaseRepository.GetByProductIdPhaseIdAndCompanyIdAsync(request.ItemId, phaseId, fromCompany)
                        ?? throw new ProductPhaseNotFoundException();

                    _products.Add(productPhase);
                }

                if(request.ProductPhaseType == ProductPhaseType.NO_PROBLEM)
                {
                    if (productPhase.AvailableQuantity < request.Quantity)
                    {
                        throw new ItemAvailableNotEnoughException();
                    }

                    productPhase.UpdateAvailableQuantity(productPhase.AvailableQuantity - (int)request.Quantity);
                }
                else if(request.ProductPhaseType == ProductPhaseType.THIRD_PARTY_ERROR)
                {
                    if(productPhase.ErrorAvailableQuantity < request.Quantity)
                    {
                        throw new ItemAvailableNotEnoughException();
                    }

                    productPhase.UpdateErrorAvailableQuantity(productPhase.ErrorAvailableQuantity - (int)request.Quantity);
                }
                else
                {
                    throw new ShipmentBadRequestException
                        ("Khi tạo đơn hàng gửi cho bên thứ 3 thì chỉ tạo từ sản phẩm bình thường hoặc sản phẩm hỏng do bên thứ 3");
                }

                _productPhaseRepository.UpdateProductPhase(productPhase);

                return ShipmentDetail.CreateShipmentProductDetail(shipmentId, request);

            case KindOfShip.SHIP_FACTORY_MATERIAL:
                var material = await _materialRepository.GetMaterialByIdAsync(request.ItemId)
                    ?? throw new MaterialNotFoundException();

                if (material.AvailableQuantity < request.Quantity)
                {
                    throw new ItemAvailableNotEnoughException();
                }

                material.UpdateAvailableQuantity(material.AvailableQuantity - request.Quantity);
                _materialRepository.UpdateMaterial(material);

                return ShipmentDetail.CreateShipmentMaterialDetail(shipmentId, request);

            default:
                throw new KindOfShipNotFoundException();
        }
    }
}
