using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Shipment.Share;
using Contract.Services.Shipment.UpdateAccepted;
using Contract.Services.ShipmentDetail.Share;
using Domain.Entities;
using Domain.Exceptions.Materials;
using Domain.Exceptions.ProductPhases;
using Domain.Exceptions.ShipmentDetails;
using Domain.Exceptions.Shipments;

namespace Application.UserCases.Commands.Shipments.UpdateAccepted;

internal sealed class UpdateAcceptedCommandHandler(
    IShipmentRepository _shipmentRepository,
    ICompanyRepository _companyRepository,
    IMaterialRepository _materialRepository,
    IProductPhaseRepository _productPhaseRepository,
    IUnitOfWork _unitOfWork) : ICommandHandler<UpdateAcceptedCommand>
{
    public async Task<Result.Success> Handle(UpdateAcceptedCommand request, CancellationToken cancellationToken)
    {
        var shipment = await GetAndValidateInput(request);

        await UpdateQuantities(shipment);

        shipment.UpdateAccepted(request.updatedBy);

        _shipmentRepository.Update(shipment);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success.Update();
    }

    private async Task<Shipment> GetAndValidateInput(UpdateAcceptedCommand request)
    {
        var shipment = await _shipmentRepository.GetByIdAndShipmentDetailAsync(request.shipmentId)
            ?? throw new ShipmentNotFoundException();

        if (shipment.Status == Status.WAIT_FOR_SHIP || shipment.Status == Status.SHIPPING)
        {
            throw new ShipmentBadRequestException("Chỉ chốt được đơn hàng khi đơn hàng đã hoàn thành hoặc đã bị hủy");
        }

        if (shipment.IsAccepted)
        {
            throw new ShipmentBadRequestException("Đơn hàng đã được chốt");
        }

        return shipment;
    }

    private async Task UpdateQuantities(Shipment shipment)
    {
        var shipmentDetails = shipment.ShipmentDetails ?? throw new ShipmentDetailNotFoundException();

        var isFromCompanyThirdParty = await _companyRepository.IsThirdPartyCompanyAsync(shipment.FromId);

        if (isFromCompanyThirdParty)
        {
            await UpdateQuantitiesForThirdPartyCompany(shipmentDetails, shipment.ToId, shipment.FromId);
        }
        else
        {
            await UpdateQuantitiesForFactory(shipmentDetails, shipment.ToId, shipment.FromId);
        }

        await UpdateMaterialQuantities(shipmentDetails, isFromCompanyThirdParty);
    }

    private async Task UpdateQuantitiesForFactory(List<ShipmentDetail> shipmentDetails, Guid toId, Guid fromId)
    {
        var productPhases = new Dictionary<(Guid productId, Guid phaseId, Guid companyId), ProductPhase>();

        foreach (var detail in shipmentDetails)
        {
            if (detail.ProductId != null && detail.PhaseId != null)
            {
                var productPhaseFromCompany = await GetOrAddProductPhase(productPhases, (detail.ProductId.Value, detail.PhaseId.Value, fromId));
                var productPhaseToCompany = await GetOrAddProductPhase(productPhases, (detail.ProductId.Value, detail.PhaseId.Value, toId));

                UpdateProductPhaseQuantities(detail, productPhaseFromCompany, productPhaseToCompany);
            }
        }

        _productPhaseRepository.UpdateProductPhaseRange(productPhases.Values.ToList());
    }

    private async Task<ProductPhase> GetOrAddProductPhase(Dictionary<(Guid productId, Guid phaseId, Guid companyId), ProductPhase> productPhases, (Guid productId, Guid phaseId, Guid companyId) key)
    {
        if (!productPhases.TryGetValue(key, out var productPhase))
        {
            productPhase = await _productPhaseRepository.GetByProductIdPhaseIdAndCompanyIdAsync(key.productId, key.phaseId, key.companyId)
                ?? throw new ProductPhaseNotFoundException();

            productPhases[key] = productPhase;
        }

        return productPhase;
    }

    private void UpdateProductPhaseQuantities(ShipmentDetail detail, ProductPhase from, ProductPhase to)
    {
        if (detail.ProductPhaseType == ProductPhaseType.NO_PROBLEM)
        {
            var quantity = from.Quantity - (int) detail.Quantity;
            if (quantity < 0)
            {
                throw new ItemAvailableNotEnoughException($"Số lượng sản phẩm không đủ - id sản phẩm: {detail.ProductId}");
            }
            from.UpdateQuantity(quantity);
            to.UpdateQuantity(to.Quantity + (int)detail.Quantity);
        }
        else if (detail.ProductPhaseType == ProductPhaseType.THIRD_PARTY_ERROR)
        {
            var quantity = from.ErrorQuantity - (int)detail.Quantity;
            if (quantity < 0)
            {
                throw new ItemAvailableNotEnoughException($"Số lượng sản phẩm không đủ - id sản phẩm: {detail.ProductId}");
            }
            from.UpdateErrorAvailableQuantity(quantity);
            to.UpdateQuantity(to.ErrorQuantity + (int)detail.Quantity);
        }
        else
        {
            throw new ShipmentBadRequestException("Cơ sở chỉ được gửi sản phẩm bình thường và sản phẩm lỗ do bên thứ 3");
        }
    }

    private async Task UpdateQuantitiesForThirdPartyCompany(List<ShipmentDetail> shipmentDetails, Guid toId, Guid fromId)
    {
        var productPhases = new Dictionary<(Guid productId, Guid companyId), List<ProductPhase>>();

        foreach (var detail in shipmentDetails)
        {
            if (detail.ProductId != null)
            {
                var productPhasesFromCompany = await GetOrAddProductPhases(productPhases, (detail.ProductId.Value, fromId));
                var productPhaseToCompany = await GetOrAddProductPhase(new Dictionary<(Guid, Guid, Guid), ProductPhase>(), (detail.ProductId.Value, detail.PhaseId.Value, toId));

                UpdateThirdPartyProductPhaseQuantities(detail, productPhasesFromCompany, productPhaseToCompany);
            }
        }

        _productPhaseRepository.UpdateProductPhaseRange(productPhases.Values.SelectMany(p => p).ToList());
    }

    private async Task<List<ProductPhase>> GetOrAddProductPhases(Dictionary<(Guid productId, Guid companyId), List<ProductPhase>> productPhases, (Guid productId, Guid companyId) key)
    {
        if (!productPhases.TryGetValue(key, out var productPhaseList))
        {
            productPhaseList = await _productPhaseRepository.GetByProductIdAndCompanyIdAsync(key.productId, key.companyId)
                ?? throw new ProductPhaseNotFoundException();

            productPhases[key] = productPhaseList;
        }

        return productPhaseList;
    }

    private void UpdateThirdPartyProductPhaseQuantities(ShipmentDetail detail, List<ProductPhase> fromList, ProductPhase to)
    {
        var totalQuantity = fromList.Sum(ph => ph.Quantity + ph.ErrorQuantity);
        if (totalQuantity < detail.Quantity)
        {
            throw new ItemAvailableNotEnoughException($"Số lượng sản phẩm không đủ - id sản phẩm: {detail.ProductId}");
        }

        int remainQuantity = (int)detail.Quantity;
        foreach (var productPhase in fromList)
        {
            remainQuantity = DeductQuantity(productPhase, remainQuantity, ProductPhaseType.NO_PROBLEM)
                ?? DeductQuantity(productPhase, remainQuantity, ProductPhaseType.THIRD_PARTY_ERROR)
                ?? remainQuantity;
            if (remainQuantity == 0) break;
        }

        if (remainQuantity > 0)
        {
            throw new ItemAvailableNotEnoughException($"Số lượng sản phẩm không đủ - id sản phẩm: {detail.ProductId}");
        }

        if(detail.ProductPhaseType == ProductPhaseType.NO_PROBLEM)
        {
            to.UpdateQuantity(to.Quantity + (int)detail.Quantity);
        }
        else if(detail.ProductPhaseType == ProductPhaseType.THIRD_PARTY_NO_FIX_ERROR)
        {
            to.UpdateBrokenQuantity(to.BrokenQuantity + (int)detail.Quantity);
        }
        else if(detail.ProductPhaseType == ProductPhaseType.FACTORY_ERROR)
        {
            to.UpdateFailureQuantity(to.FailureQuantity + (int)detail.Quantity);
        }
        else
        {
            throw new ShipmentBadRequestException("Bên thứ 3 chỉ được gửi sản phẩm bình thường, " +
                        "sản phẩm lỗi do cơ sở hoặc sản phẩm hỏng hằn");
        }
    }

    private int? DeductQuantity(ProductPhase productPhase, int remainQuantity, ProductPhaseType phaseType)
    {
        var quantity = phaseType switch
        {
            ProductPhaseType.NO_PROBLEM => productPhase.Quantity,
            ProductPhaseType.THIRD_PARTY_ERROR => productPhase.ErrorQuantity,
            _ => throw new ShipmentBadRequestException("Bên thứ 3 chỉ được gửi sản phẩm bình thường, " +
                        "sản phẩm lỗi do cơ sở hoặc sản phẩm hỏng hằn")
        };

        if (quantity < remainQuantity)
        {
            remainQuantity -= quantity;
            productPhase.UpdateQuantity(0);
        }
        else
        {
            productPhase.UpdateQuantity(quantity - remainQuantity);
            remainQuantity = 0;
        }

        return remainQuantity == 0 ? (int?)null : remainQuantity;
    }

    private async Task UpdateMaterialQuantities(List<ShipmentDetail> shipmentDetails, bool isFromCompanyThirdParty)
    {
        var materialIds = shipmentDetails
            .Where(s => s.MaterialId != null)
            .Select(s => (Guid)s.MaterialId)
            .ToList();

        if (isFromCompanyThirdParty && materialIds.Any())
        {
            throw new ShipmentBadRequestException("Công ty bên thứ 3 không gửi được nguyên liệu");
        }

        if (!materialIds.Any())
        {
            return;
        }

        var materials = await _materialRepository.GetMaterialsByIdsAsync(materialIds);

        foreach (var detail in shipmentDetails)
        {
            if (detail.MaterialId != null)
            {
                var material = materials.SingleOrDefault(m => m.Id == detail.MaterialId)
                    ?? throw new MaterialNotFoundException();

                var quantity = material.QuantityInStock - detail.Quantity;

                if (quantity < 0) throw new ItemAvailableNotEnoughException();

                material.UpdateQuantityInStock(quantity);
            }
        }

        _materialRepository.UpdateRange(materials);
    }
}
