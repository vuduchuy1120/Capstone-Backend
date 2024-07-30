using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.ProductPhase.Creates;
using Contract.Services.Shipment.Share;
using Contract.Services.Shipment.UpdateAccepted;
using Contract.Services.ShipmentDetail.Share;
using Domain.Entities;
using Domain.Exceptions.Materials;
using Domain.Exceptions.ProductPhases;
using Domain.Exceptions.ShipmentDetails;
using Domain.Exceptions.Shipments;
using System.Collections.Generic;

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

        await UpdateQuantityOfProductPhaseAndMaterial(shipment);

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

    private async Task UpdateQuantityOfProductPhaseAndMaterial(Shipment shipment)
    {
        var shipmentDetails = shipment.ShipmentDetails ?? throw new ShipmentDetailNotFoundException();

        if(shipment.Status == Status.CANCEL)
        {
            await UpdateAvailableQuantityWhenCancel(shipmentDetails, shipment.FromId);
            return;
        }

        var isFromCompanyThirdParty = await _companyRepository.IsThirdPartyCompanyAsync(shipment.FromId);

        if (isFromCompanyThirdParty)
        {
            await UpdateQuantityOfProductPhaseWhenSendFromThirdPartyCompany(shipmentDetails, shipment.ToId, shipment.FromId);
        }
        else
        {
            await UpdateQuantityOfProductPhaseWhenSendFromFactory(shipmentDetails, shipment.ToId, shipment.FromId);
        }

        await UpdateQuantityOfMaterial(shipmentDetails, isFromCompanyThirdParty, true);
    }

    private async Task UpdateAvailableQuantityWhenCancel(List<ShipmentDetail> shipmentDetails, Guid fromId)
    {
        // update product when cancel
        await UpdateAvailableProductQuantityWhenCancel(shipmentDetails, fromId);

        var isFromCompanyThirdParty = await _companyRepository.IsThirdPartyCompanyAsync(fromId);

        //update material when cancel
        await UpdateQuantityOfMaterial(shipmentDetails, isFromCompanyThirdParty, false);
    }

    private async Task UpdateAvailableProductQuantityWhenCancel(List<ShipmentDetail> shipmentDetails, Guid fromId)
    {
        var _productsPhases = new List<ProductPhase>();

        foreach (var detail in shipmentDetails)
        {
            if (detail.ProductId != null && detail.PhaseId != null)
            {
                var productPhaseFromCompany = _productsPhases
                    .Where(p => p.PhaseId == detail.PhaseId && p.CompanyId == fromId && p.ProductId == detail.ProductId)
                    .FirstOrDefault();

                if (productPhaseFromCompany == null)
                {
                    productPhaseFromCompany = await _productPhaseRepository.GetByProductIdPhaseIdAndCompanyIdAsync(
                        (Guid)detail.ProductId,
                        (Guid)detail.PhaseId,
                        fromId) ?? throw new ProductPhaseNotFoundException();

                    _productsPhases.Add(productPhaseFromCompany);
                }

                switch (detail.ProductPhaseType)
                {
                    case ProductPhaseType.NO_PROBLEM:
                        productPhaseFromCompany.UpdateAvailableQuantity(productPhaseFromCompany.AvailableQuantity + (int) detail.Quantity);
                        break;
                    case ProductPhaseType.THIRD_PARTY_ERROR:
                        productPhaseFromCompany.UpdateErrorAvailableQuantity(productPhaseFromCompany.ErrorAvailableQuantity + (int)detail.Quantity);
                        break;
                    case ProductPhaseType.THIRD_PARTY_NO_FIX_ERROR:
                        productPhaseFromCompany.UpdateBrokenAvailableQuantity(productPhaseFromCompany.BrokenAvailableQuantity + (int)detail.Quantity);
                        break;
                    case ProductPhaseType.FACTORY_ERROR:
                        productPhaseFromCompany.UpdateFailureAvailableQuantity(productPhaseFromCompany.FailureAvailabeQuantity + (int)detail.Quantity);
                        break;
                    default:
                        throw new ShipmentBadRequestException("Không tìm thấy loại sản phẩm phù hợp");
                }
            }
        }

        _productPhaseRepository.UpdateProductPhaseRange(_productsPhases);
    }


    private async Task UpdateQuantityOfProductPhaseWhenSendFromFactory(List<ShipmentDetail> shipmentDetails, Guid toId, Guid fromId)
    {
        var _productsPhases = new List<ProductPhase>();
        var _newProductPhases = new List<ProductPhase>();

        foreach (var detail in shipmentDetails)
        {
            if (detail.ProductId != null && detail.PhaseId != null)
            {
                var productPhaseFromCompany = _productsPhases
                    .Where(p => p.PhaseId == detail.PhaseId && p.CompanyId == fromId && p.ProductId == detail.ProductId)
                    .FirstOrDefault();

                if (productPhaseFromCompany == null)
                {
                    productPhaseFromCompany = await _productPhaseRepository.GetByProductIdPhaseIdAndCompanyIdAsync(
                        (Guid)detail.ProductId,
                        (Guid)detail.PhaseId,
                        fromId) ?? throw new ProductPhaseNotFoundException();

                    _productsPhases.Add(productPhaseFromCompany);
                }

                var productPhaseToCompany = _productsPhases
                    .Where(p => p.PhaseId == detail.PhaseId && p.CompanyId == toId && p.ProductId == detail.ProductId)
                    .FirstOrDefault();

                if (productPhaseToCompany == null)
                {
                    productPhaseToCompany = _newProductPhases
                        .Where(p => p.PhaseId == detail.PhaseId && p.CompanyId == toId && p.ProductId == detail.ProductId)
                        .FirstOrDefault();
                }

                if (productPhaseToCompany == null)
                {
                    productPhaseToCompany = await _productPhaseRepository.GetByProductIdPhaseIdAndCompanyIdAsync(
                        (Guid)detail.ProductId,
                        (Guid)detail.PhaseId,
                        toId);

                    if (productPhaseToCompany == null)
                    {
                        productPhaseToCompany = ProductPhase.Create(new CreateProductPhaseRequest(
                            (Guid)detail.ProductId,
                            (Guid)detail.PhaseId,
                            0,
                            0,
                            toId));

                        _newProductPhases.Add(productPhaseToCompany);
                    }
                    else
                    {
                        _productsPhases.Add(productPhaseToCompany);
                    }
                }

                if (detail.ProductPhaseType == ProductPhaseType.NO_PROBLEM)
                {
                    var quantityOfFromCompany = productPhaseFromCompany.Quantity - (int)detail.Quantity;
                    if (quantityOfFromCompany < 0)
                        throw new ItemAvailableNotEnoughException($"Số lượng sản phẩm không đủ - id sản phẩm: {detail.ProductId} - NO_PROBLEM");
                    productPhaseFromCompany.UpdateQuantity(quantityOfFromCompany);

                    productPhaseToCompany.UpdateQuantity(productPhaseToCompany.Quantity + (int)detail.Quantity);
                }
                else if (detail.ProductPhaseType == ProductPhaseType.THIRD_PARTY_ERROR)
                {
                    var quantityOfFromCompany = productPhaseFromCompany.ErrorQuantity - (int)detail.Quantity;
                    if (quantityOfFromCompany < 0)
                        throw new ItemAvailableNotEnoughException($"Số lượng sản phẩm không đủ - id sản phẩm: {detail.ProductId} - ERROR");
                    productPhaseFromCompany.UpdateErrorAvailableQuantity(quantityOfFromCompany);

                    productPhaseToCompany.UpdateErrorAvailableQuantity(productPhaseToCompany.ErrorQuantity + (int)detail.Quantity);
                }
                else
                {
                    throw new ShipmentBadRequestException("Cơ sở chỉ được gửi sản phẩm bình thường và sản phẩm lỗ do bên thứ 3");
                }

            }
        }

        _productPhaseRepository.UpdateProductPhaseRange(_productsPhases);
        _productPhaseRepository.AddProductPhaseRange(_newProductPhases);
    }

    private async Task UpdateQuantityOfProductPhaseWhenSendFromThirdPartyCompany(List<ShipmentDetail> shipmentDetails, Guid toId, Guid fromId)
    {
        var _productsPhases = new List<ProductPhase>();

        foreach (var detail in shipmentDetails)
        {
            if (detail.ProductId != null && detail.PhaseId != null)
            {
                var productPhasesFromCompany = _productsPhases
                    .Where(p => p.CompanyId == fromId && p.ProductId == detail.ProductId)
                    .ToList();

                if (productPhasesFromCompany == null)
                {
                    productPhasesFromCompany = await _productPhaseRepository.GetByProductIdAndCompanyIdAsync(
                        (Guid)detail.ProductId,
                        fromId);

                    if (productPhasesFromCompany is null || productPhasesFromCompany.Count == 0)
                    {
                        throw new ProductPhaseNotFoundException();
                    }

                    foreach (var p in productPhasesFromCompany)
                    {
                        _productsPhases.Add(p);
                    }
                }

                var totalQuantity = productPhasesFromCompany.Sum(ph => ph.Quantity + ph.ErrorQuantity);
                if (totalQuantity < detail.Quantity)
                {
                    throw new ItemAvailableNotEnoughException($"Số lượng sản phẩm không đủ - id sản phẩm: {detail.ProductId}");
                }

                int remainQuantity = (int)detail.Quantity;
                foreach (var productPhase in productPhasesFromCompany)
                {
                    if (productPhase.Quantity < remainQuantity)
                    {
                        remainQuantity = remainQuantity - productPhase.Quantity;
                        productPhase.UpdateQuantity(0);
                    }
                    else
                    {
                        productPhase.UpdateQuantity(productPhase.Quantity - remainQuantity);
                        break;
                    }

                    if (productPhase.ErrorQuantity < remainQuantity)
                    {
                        remainQuantity = remainQuantity - productPhase.ErrorQuantity;
                        productPhase.UpdateQuantity(0);
                    }
                    else
                    {
                        productPhase.UpdateQuantity(productPhase.ErrorQuantity - remainQuantity);
                        break;
                    }
                }

                if (remainQuantity > 0)
                {
                    throw new ItemAvailableNotEnoughException($"Số lượng sản phẩm không đủ - id sản phẩm: {detail.ProductId}");
                }

                var productPhaseToCompany = _productsPhases
                    .Where(p => p.PhaseId == detail.PhaseId && p.CompanyId == toId && p.ProductId == detail.ProductId)
                    .FirstOrDefault();

                if (productPhaseToCompany == null)
                {
                    productPhaseToCompany = await _productPhaseRepository.GetByProductIdPhaseIdAndCompanyIdAsync(
                        (Guid)detail.ProductId,
                        (Guid)detail.PhaseId,
                        toId) ?? throw new ProductPhaseNotFoundException();

                    _productsPhases.Add(productPhaseToCompany);
                }

                if (detail.ProductPhaseType == ProductPhaseType.NO_PROBLEM)
                {
                    productPhaseToCompany.UpdateQuantity(productPhaseToCompany.Quantity + (int)detail.Quantity);
                }
                else if (detail.ProductPhaseType == ProductPhaseType.FACTORY_ERROR)
                {
                    productPhaseToCompany.UpdateFailureQuantity(productPhaseToCompany.FailureQuantity + (int)detail.Quantity);
                }
                else if (detail.ProductPhaseType == ProductPhaseType.THIRD_PARTY_NO_FIX_ERROR)
                {
                    productPhaseToCompany.UpdateErrorQuantity(productPhaseToCompany.BrokenQuantity + (int)detail.Quantity);
                }
                else
                {
                    throw new ShipmentBadRequestException("Bên thứ 3 chỉ được gửi sản phẩm bình thường, " +
                        "sản phẩm lỗi do cơ sở hoặc sản phẩm hỏng hằn");
                }
            }
        }

        _productPhaseRepository.UpdateProductPhaseRange(_productsPhases);
    }

    private async Task UpdateQuantityOfMaterial(List<ShipmentDetail> shipmentDetails, bool isFromCompanyThirdParty, bool isShipSuccess)
    {
        var materialIds = shipmentDetails
            .Where(s => s.MaterialId != null)
            .Select(s => (Guid)s.MaterialId)
            .ToList();

        if (isFromCompanyThirdParty && materialIds is not null && materialIds.Count > 0)
        {
            throw new ShipmentBadRequestException("Công ty bên thứ 3 không gửi được nguyên liệu");
        }

        if (materialIds is null || materialIds.Count == 0)
        {
            return;
        }

        var materials = await _materialRepository.GetMaterialsByIdsAsync(materialIds);

        foreach (var detail in shipmentDetails)
        {
            if (detail.MaterialId is not null)
            {
                var material = materials.SingleOrDefault(m => m.Id == detail.MaterialId)
                    ?? throw new MaterialNotFoundException();

                if (isShipSuccess)
                {
                    var quantity = material.QuantityInStock - detail.Quantity;
                    if (quantity < 0) throw new ItemAvailableNotEnoughException();

                    material.UpdateQuantityInStock(quantity);
                }
                else
                {
                    var quantity = material.AvailableQuantity + detail.Quantity;
                    material.UpdateAvailableQuantity(quantity);
                }
            }
        }

        _materialRepository.UpdateRange(materials);
    }
}