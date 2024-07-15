using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Shipment.Update;
using Contract.Services.ShipmentDetail.Share;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using Domain.Exceptions.Materials;
using Domain.Exceptions.Phases;
using Domain.Exceptions.ProductPhases;
using Domain.Exceptions.ShipmentDetails;
using Domain.Exceptions.Shipments;
using FluentValidation;

namespace Application.UserCases.Commands.Shipments.Update;

internal sealed class UpdateShipmentCommandHandler(
    IShipmentRepository _shipmentRepository,
    IMaterialRepository _materialRepository,
    IPhaseRepository _phaseRepository,
    ICompanyRepository _companyRepository,
    IProductPhaseRepository _productPhaseRepository,
    IUnitOfWork _unitOfWork,
    IValidator<UpdateShipmentRequest> _validator) : ICommandHandler<UpdateShipmentCommand>
{
    public async Task<Result.Success> Handle(UpdateShipmentCommand request, CancellationToken cancellationToken)
    {
        var updateRequest = request.UpdateShipmentRequest;

        await ValidateInput(request.Id, updateRequest);

        var shipment = await _shipmentRepository.GetByIdAndShipmentDetailAsync(request.Id)
            ?? throw new ShipmentNotFoundException();

        await UpdateShipment(shipment, updateRequest, request.UpdatedBy);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success.Update();
    }

    private async Task ValidateInput(Guid id, UpdateShipmentRequest updateShipmentRequest)
    {
        if (updateShipmentRequest.ShipmentId != id)
        {
            throw new ShipmentIdConflictException();
        }

        var validatorResult = await _validator.ValidateAsync(updateShipmentRequest);

        if (!validatorResult.IsValid) throw new MyValidationException(validatorResult.ToDictionary());
    }

    private List<Guid> GetAllProductIds(UpdateShipmentRequest updateShipmentRequest, List<ShipmentDetail> shipmentDetails)
    {
        var oldIds = shipmentDetails
            .Where(detail => detail.ProductId != null && detail.ProductId != Guid.Empty)
            .Select(detail => (Guid)detail.ProductId)
            .ToList() ?? new List<Guid>();

        var newIds = updateShipmentRequest.ShipmentDetailRequests
            .Where(req => req.KindOfShip == KindOfShip.SHIP_FACTORY_PRODUCT)
            .Select(req => req.ItemId)
            .ToList() ?? new List<Guid>();

        return oldIds.Union(newIds).ToList();
    }

    private List<Guid> GetAllMaterialIds(UpdateShipmentRequest updateShipmentRequest, List<ShipmentDetail> shipmentDetails)
    {
        var oldIds = shipmentDetails
            .Where(detail => detail.MaterialId != null && detail.MaterialId != Guid.Empty)
            .Select(detail => (Guid)detail.MaterialId)
            .ToList() ?? new List<Guid>();

        var newIds = updateShipmentRequest.ShipmentDetailRequests
            .Where(req => req.KindOfShip == KindOfShip.SHIP_FACTORY_MATERIAL)
            .Select(req => req.ItemId)
            .ToList() ?? new List<Guid>();

        return oldIds.Union(newIds).ToList();
    }

    private async Task UpdateShipment(Shipment shipment, UpdateShipmentRequest updateShipmentRequest, string updatedBy)
    {
        if (shipment.FromId == updateShipmentRequest.FromId)
        {
            await UpdateShipmentWithSameFromCompany(shipment, updateShipmentRequest, updatedBy);
        }
        else
        {
            await UpdateShipmentWithoutSameFromCompany(shipment, updateShipmentRequest, updatedBy);
        }

        var newShipmentDetails = CreateNewShipmentDetail(updateShipmentRequest.ShipmentDetailRequests, shipment.Id);

        shipment.Update(updateShipmentRequest, updatedBy, newShipmentDetails);

        _shipmentRepository.Update(shipment);
    }

    private List<ShipmentDetail> CreateNewShipmentDetail(List<ShipmentDetailRequest> shipmentDetailRequests, Guid shipmentId)
    {
        return shipmentDetailRequests.Select(req =>
        {
            return req.KindOfShip switch
            {
                KindOfShip.SHIP_FACTORY_PRODUCT => ShipmentDetail.CreateShipmentProductDetail(shipmentId, req),
                KindOfShip.SHIP_FACTORY_MATERIAL => ShipmentDetail.CreateShipmentMaterialDetail(shipmentId, req),
                _ => throw new KindOfShipNotFoundException(),
            };
        }).ToList();
    }

    private async Task UpdateShipmentWithSameFromCompany(Shipment shipment, UpdateShipmentRequest updateShipmentRequest, string updatedBy)
    {
        var isFromCompanyIsThirdPartyCompany = await _companyRepository.IsThirdPartyCompanyAsync(updateShipmentRequest.FromId);

        var shipmentDetails = shipment.ShipmentDetails ?? throw new ShipmentDetailNotFoundException();

        var allMaterialIds = GetAllMaterialIds(updateShipmentRequest, shipmentDetails);
        if (isFromCompanyIsThirdPartyCompany && allMaterialIds is not null && allMaterialIds.Count > 0)
        {
            throw new ShipmentBadRequestException("Công ty bên thứ 3 không được gửi nguyên liệu");
        }

        if (allMaterialIds is not null && allMaterialIds.Count > 0)
        {
            var materials = await _materialRepository.GetMaterialsByIdsAsync(allMaterialIds);

            foreach (var detail in shipmentDetails)
            {
                if (detail.MaterialId is not null)
                {
                    var material = materials.SingleOrDefault(material => material.Id == detail.MaterialId)
                        ?? throw new MaterialNotFoundException();

                    material.UpdateAvailableQuantity(material.AvailableQuantity + detail.Quantity);
                }
            }

            foreach (var req in updateShipmentRequest.ShipmentDetailRequests)
            {
                if (req.KindOfShip == KindOfShip.SHIP_FACTORY_MATERIAL)
                {
                    var material = materials.SingleOrDefault(material => material.Id == req.ItemId)
                       ?? throw new MaterialNotFoundException();

                    var materialAvailableQuantity = material.AvailableQuantity - req.Quantity;

                    if (materialAvailableQuantity < 0) throw new ShipmentBadRequestException($"Không đủ số lượng nguyên liệu {req.ItemId}");

                    material.UpdateAvailableQuantity(materialAvailableQuantity);
                }
            }

            _materialRepository.UpdateRange(materials);
        }

        var allProductIds = GetAllProductIds(updateShipmentRequest, shipmentDetails);
        if (allProductIds is not null && allProductIds.Count > 0)
        {
            var productPhases = await _productPhaseRepository.GetByProductIdsAndCompanyIdAsync(allProductIds, updateShipmentRequest.FromId);

            if (isFromCompanyIsThirdPartyCompany)
            {
                var phase = await _phaseRepository.GetPhaseByName("PH_001") ?? throw new PhaseNotFoundException();
                foreach (var detail in shipmentDetails)
                {
                    if (detail.ProductId is not null && detail.PhaseId is not null)
                    {
                        var productPhase = productPhases.SingleOrDefault(p => p.ProductId == detail.ProductId && p.PhaseId == phase.Id)
                             ?? throw new ProductPhaseNotFoundException();

                        productPhase.UpdateAvailableQuantity(productPhase.Quantity + (int)detail.Quantity);
                    }
                }

                foreach (var request in updateShipmentRequest.ShipmentDetailRequests)
                {
                    if (request.KindOfShip == KindOfShip.SHIP_FACTORY_PRODUCT)
                    {
                        var products = productPhases.Where(ph => ph.ProductId == request.ItemId).ToList();

                        var totalQuantity = products.Sum(ph => ph.AvailableQuantity + ph.ErrorAvailableQuantity);

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
                    }
                }
            }
            else
            {
                foreach (var detail in shipmentDetails)
                {
                    if (detail.ProductId is not null)
                    {
                        var productPhase = productPhases.SingleOrDefault(p => p.ProductId == detail.ProductId && p.PhaseId == detail.PhaseId)
                            ?? throw new ProductPhaseNotFoundException();

                        switch (detail.ProductPhaseType)
                        {
                            case ProductPhaseType.NO_PROBLEM:
                                productPhase.UpdateAvailableQuantity(productPhase.AvailableQuantity + (int)detail.Quantity);
                                break;
                            case ProductPhaseType.THIRD_PARTY_ERROR:
                                productPhase.UpdateErrorAvailableQuantity(productPhase.ErrorAvailableQuantity + (int)detail.Quantity);
                                break;
                            default:
                                throw new ShipmentBadRequestException("Đơn hàng từ cơ sở chỉ được là sản phẩm không lỗi hoặc lỗi do bên thứ 3");
                        }
                    }
                }

                foreach (var request in updateShipmentRequest.ShipmentDetailRequests)
                {
                    if (request.KindOfShip == KindOfShip.SHIP_FACTORY_PRODUCT)
                    {
                        var productPhase = productPhases.SingleOrDefault(p => p.ProductId == request.ItemId && p.PhaseId == request.PhaseId)
                             ?? throw new ProductPhaseNotFoundException();

                        switch (request.ProductPhaseType)
                        {
                            case ProductPhaseType.NO_PROBLEM:
                                int availableQuantity = productPhase.AvailableQuantity - (int)request.Quantity;
                                if (availableQuantity < 0) throw new ShipmentBadRequestException($"Cở sở không có đủ sản phầm {request.ItemId} trong giai đoạn {request.PhaseId} trong kho");
                                productPhase.UpdateAvailableQuantity(availableQuantity);
                                break;
                            case ProductPhaseType.THIRD_PARTY_ERROR:
                                int errorAvailableQuantity = productPhase.ErrorAvailableQuantity - (int)request.Quantity;
                                if (errorAvailableQuantity < 0) throw new ShipmentBadRequestException($"Cở sở không có đủ sản phầm tình trạng lỗi {request.ItemId} trong giai đoạn {request.PhaseId} trong kho");
                                productPhase.UpdateErrorAvailableQuantity(errorAvailableQuantity);
                                break;
                            default:
                                throw new ShipmentBadRequestException("Đơn hàng từ cơ sở chỉ được là sản phẩm không lỗi hoặc lỗi do bên thứ 3");
                        }
                    }
                }
            }

            _productPhaseRepository.UpdateProductPhaseRange(productPhases);
        }
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

    private async Task UpdateShipmentWithoutSameFromCompany(Shipment shipment, UpdateShipmentRequest updateShipmentRequest, string updatedBy)
    {
        await UpdateProductPhaseOfOldShipment(shipment);

        await UpdateProductPhaseAndMaterialOfNewRequest(updateShipmentRequest, shipment);
    }
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
    private async Task UpdateProductPhaseOfOldShipment(Shipment shipment)
    {
        var isFromCompanyIsThirdPartyCompany = await _companyRepository.IsThirdPartyCompanyAsync(shipment.FromId);

        var shipmentDetails = shipment.ShipmentDetails ?? throw new ShipmentDetailNotFoundException();

        var productIds = shipmentDetails
            .Where(s => s.ProductId != null)
            .Select(s => (Guid)s.ProductId)
            .Distinct()
            .ToList();

        if (productIds is not null && productIds.Count() > 0)
        {
            var productPhases = await _productPhaseRepository.GetByProductIdsAndCompanyIdAsync(productIds, shipment.FromId);

            if (isFromCompanyIsThirdPartyCompany)
            {
                var phase = await _phaseRepository.GetPhaseByName("PH_001") ?? throw new PhaseNotFoundException();

                foreach (var detail in shipmentDetails)
                {
                    if (detail.ProductId is not null && detail.PhaseId is not null)
                    {
                        var productPhase = productPhases.SingleOrDefault(p => p.ProductId == detail.ProductId && p.PhaseId == phase.Id)
                             ?? throw new ProductPhaseNotFoundException();

                        productPhase.UpdateAvailableQuantity(productPhase.Quantity + (int)detail.Quantity);
                    }
                }
            }
            else
            {
                foreach (var detail in shipmentDetails)
                {
                    var productPhase = productPhases.SingleOrDefault(p => p.ProductId == detail.ProductId && p.PhaseId == detail.PhaseId)
                        ?? throw new ProductPhaseNotFoundException();

                    switch (detail.ProductPhaseType)
                    {
                        case ProductPhaseType.NO_PROBLEM:
                            productPhase.UpdateAvailableQuantity(productPhase.AvailableQuantity + (int)detail.Quantity);
                            break;
                        case ProductPhaseType.THIRD_PARTY_ERROR:
                            productPhase.UpdateErrorAvailableQuantity(productPhase.ErrorAvailableQuantity + (int)detail.Quantity);
                            break;
                        default:
                            throw new ShipmentBadRequestException("Đơn hàng từ cơ sở chỉ được là sản phẩm không lỗi hoặc lỗi do bên thứ 3");
                    }
                }
            }

            _productPhaseRepository.UpdateProductPhaseRange(productPhases);
        }

        if (!isFromCompanyIsThirdPartyCompany)
        {
            var materialIds = shipmentDetails
            .Where(s => s.MaterialId != null)
            .Select(s => (Guid)s.MaterialId)
            .Distinct()
            .ToList();

            var materials = await _materialRepository.GetMaterialsByIdsAsync(materialIds)
                ?? throw new MaterialNotFoundException();

            foreach (var detail in shipmentDetails)
            {
                if (detail.MaterialId != null)
                {
                    var material = materials.SingleOrDefault(material => material.Id == detail.MaterialId)
                        ?? throw new MaterialNotFoundException();

                    material.UpdateAvailableQuantity(material.AvailableQuantity + detail.Quantity);
                }
            }

            _materialRepository.UpdateRange(materials);
        }
    }

    private async Task UpdateProductPhaseAndMaterialOfNewRequest(UpdateShipmentRequest updateShipmentRequest, Shipment shipment)
    {
        var shipmentDetails = shipment.ShipmentDetails ?? throw new ShipmentDetailNotFoundException();

        var isFromCompanyIsThirdPartyCompany = await _companyRepository.IsThirdPartyCompanyAsync(updateShipmentRequest.FromId);

        var updateMaterialIds = updateShipmentRequest.ShipmentDetailRequests
            .Where(s => s.KindOfShip == KindOfShip.SHIP_FACTORY_MATERIAL)
            .Select(s => s.ItemId)
            .Distinct()
            .ToList();
        if (isFromCompanyIsThirdPartyCompany && updateMaterialIds is not null && updateMaterialIds.Count > 0)
        {
            throw new ShipmentBadRequestException("Công ty bên thứ 3 không được gửi nguyên liệu");
        }

        var allMaterialIds = GetAllMaterialIds(updateShipmentRequest, shipmentDetails);
        if (allMaterialIds is not null && allMaterialIds.Count > 0)
        {
            var materials = await _materialRepository.GetMaterialsByIdsAsync(allMaterialIds);

            foreach (var detail in shipmentDetails)
            {
                if (detail.MaterialId is not null)
                {
                    var material = materials.SingleOrDefault(material => material.Id == detail.MaterialId)
                        ?? throw new MaterialNotFoundException();

                    material.UpdateAvailableQuantity(material.AvailableQuantity + detail.Quantity);
                }
            }

            foreach (var req in updateShipmentRequest.ShipmentDetailRequests)
            {
                if (req.KindOfShip == KindOfShip.SHIP_FACTORY_MATERIAL)
                {
                    var material = materials.SingleOrDefault(material => material.Id == req.ItemId)
                       ?? throw new MaterialNotFoundException();

                    var materialAvailableQuantity = material.AvailableQuantity - req.Quantity;

                    if (materialAvailableQuantity < 0) throw new ShipmentBadRequestException($"Không đủ số lượng nguyên liệu {req.ItemId}");

                    material.UpdateAvailableQuantity(materialAvailableQuantity);
                }
            }

            _materialRepository.UpdateRange(materials);
        }

        var productIds = updateShipmentRequest.ShipmentDetailRequests
            .Where(s => s.KindOfShip == KindOfShip.SHIP_FACTORY_PRODUCT)
            .Select(s => s.ItemId)
            .Distinct()
            .ToList();

        if (productIds is not null && productIds.Count() > 0)
        {
            var productPhases = await _productPhaseRepository.GetByProductIdsAndCompanyIdAsync(productIds, updateShipmentRequest.FromId);

            if (isFromCompanyIsThirdPartyCompany)
            {
                var phase = await _phaseRepository.GetPhaseByName("PH_001") ?? throw new PhaseNotFoundException();

                foreach (var request in updateShipmentRequest.ShipmentDetailRequests)
                {
                    if (request.KindOfShip == KindOfShip.SHIP_FACTORY_PRODUCT)
                    {
                        var products = productPhases.Where(ph => ph.ProductId == request.ItemId).ToList();

                        var totalQuantity = products.Sum(ph => ph.AvailableQuantity + ph.ErrorAvailableQuantity);

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
                    }
                }
            }
            else
            {
                foreach (var request in updateShipmentRequest.ShipmentDetailRequests)
                {
                    if (request.KindOfShip == KindOfShip.SHIP_FACTORY_PRODUCT)
                    {
                        var productPhase = productPhases.SingleOrDefault(p => p.ProductId == request.ItemId && p.PhaseId == request.PhaseId)
                             ?? throw new ProductPhaseNotFoundException();

                        switch (request.ProductPhaseType)
                        {
                            case ProductPhaseType.NO_PROBLEM:
                                int availableQuantity = productPhase.AvailableQuantity - (int)request.Quantity;
                                if (availableQuantity < 0) throw new ShipmentBadRequestException($"Cở sở không có đủ sản phầm {request.ItemId} trong giai đoạn {request.PhaseId} trong kho");
                                productPhase.UpdateAvailableQuantity(availableQuantity);
                                break;
                            case ProductPhaseType.THIRD_PARTY_ERROR:
                                int errorAvailableQuantity = productPhase.ErrorAvailableQuantity - (int)request.Quantity;
                                if (errorAvailableQuantity < 0) throw new ShipmentBadRequestException($"Cở sở không có đủ sản phầm tình trạng lỗi {request.ItemId} trong giai đoạn {request.PhaseId} trong kho");
                                productPhase.UpdateErrorAvailableQuantity(errorAvailableQuantity);
                                break;
                            default:
                                throw new ShipmentBadRequestException("Đơn hàng từ cơ sở chỉ được là sản phẩm không lỗi hoặc lỗi do bên thứ 3");
                        }
                    }
                }
            }

            _productPhaseRepository.UpdateProductPhaseRange(productPhases);
        }
    }
}
