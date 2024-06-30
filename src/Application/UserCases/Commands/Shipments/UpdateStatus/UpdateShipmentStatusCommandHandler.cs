using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.ProductPhase.Creates;
using Contract.Services.Shipment.Share;
using Contract.Services.Shipment.UpdateStatus;
using Domain.Entities;
using Domain.Exceptions.Phases;
using Domain.Exceptions.Products;
using Domain.Exceptions.ShipmentDetails;
using Domain.Exceptions.Shipments;

namespace Application.UserCases.Commands.Shipments.UpdateStatus;

internal sealed class UpdateShipmentStatusCommandHandler(
    IShipmentRepository _shipmentRepository,
    IProductPhaseRepository _productPhaseRepository,
    IMaterialRepository _materialRepository,
    IUnitOfWork _unitOfWork) : ICommandHandler<UpdateShipmentStatusCommand>
{
    public async Task<Result.Success> Handle(UpdateShipmentStatusCommand request, CancellationToken cancellationToken)
    {
        var updateRequest = request.UpdateStatusRequest;

        var shipment = await GetAndValidateInput(updateRequest, request.Id);

        shipment.UpdateStatus(request.UpdatedBy, updateRequest.Status);

        var shipmentDetails = shipment.ShipmentDetails ?? throw new ShipDetailItemNullException();

        var productShipments = shipmentDetails.Where(s
            => s.ProductId is not null && s.PhaseId is not null && s.MaterialId is null)
            .ToList();

        var materialShipments = shipmentDetails.Where(s
            => s.ProductId is null && s.PhaseId is null && s.MaterialId is not null)
            .ToList();

        if (shipment.Status == Status.SHIPPED)
        {
            await UpdateRealQuantityWhenSuccess(productShipments, materialShipments, shipment.FromId, shipment.ToId);
        } 
        else if(shipment.Status == Status.CANCEL)
        {
            await UpdateAvailableWhenCancel(productShipments, materialShipments, shipment.FromId);
        }

        _shipmentRepository.Update(shipment);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success.Update();
    }
    private async Task UpdateRealQuantityWhenSuccess(
        List<ShipmentDetail> productShipments,
        List<ShipmentDetail> materialShipments,
        Guid fromCompanyId,
        Guid toCompanyId)
    {
        var updateFromCompanyTask = UpdateRealProductQuantityOfFromCompany(productShipments, fromCompanyId);
        var updateToCompanyTask = UpdateRealProductQuantityOfToCompany(productShipments, toCompanyId);
        var updateMaterialTask = UpdateRealMaterial(materialShipments);

        await Task.WhenAll(updateFromCompanyTask, updateToCompanyTask, updateMaterialTask);
    }

    private async Task UpdateAvailableWhenCancel(
        List<ShipmentDetail> productShipments,
        List<ShipmentDetail> materialShipments,
        Guid fromCompanyId)
    {
        var updateProductPhaseTask = UpdateAvailableQuantityProductWhenCancel(productShipments, fromCompanyId);
        var updateMaterialTask = UpdateAvailableQuantityMaterialWhenCancel(materialShipments);

        await Task.WhenAll(updateMaterialTask, updateProductPhaseTask);
    }

    private async Task UpdateAvailableQuantityMaterialWhenCancel(List<ShipmentDetail> materialShipments)
    {
        if (materialShipments is null || materialShipments.Count == 0)
        {
            return;
        }

        var materialIds = materialShipments.Select(materialShipments => materialShipments.Id).ToList();

        var materials = await _materialRepository.GetMaterialsByIdsAsync(materialIds);

        if (materials is null || materials.Count != materialShipments.Count)
        {
            throw new ShipmentDetailNotFoundException();
        }

        foreach (var shipmentDetail in materialShipments)
        {
            var material = materials.SingleOrDefault(m => m.Id == shipmentDetail.MaterialId);

            if (material is null)
            {
                throw new ItemAvailableNotEnoughException();
            }

            material.UpdateAvailableQuantity(material.AvailableQuantity + shipmentDetail.Quantity);
        }

        _materialRepository.UpdateRange(materials);
    }

    private async Task UpdateAvailableQuantityProductWhenCancel(List<ShipmentDetail> productShipments, Guid companyId)
    {
        if (productShipments is null || productShipments.Count == 0)
        {
            return;
        }

        var productPhases = await _productPhaseRepository
            .GetProductPhaseByShipmentDetailAsync(productShipments, companyId);

        if (productPhases is null || productPhases.Count != productShipments.Count)
        {
            throw new ShipmentDetailNotFoundException();
        }

        foreach (var shipmentDetail in productShipments)
        {
            var productPhase = productPhases.SingleOrDefault(ph
            => ph.ProductId == shipmentDetail.ProductId
                && ph.PhaseId == shipmentDetail.PhaseId
                && ph.CompanyId == companyId);

            if (productPhase is null)
            {
                throw new ItemAvailableNotEnoughException();
            }

            productPhase.UpdateAvailableQuantity(productPhase.AvailableQuantity + (int)shipmentDetail.Quantity);
        }

        _productPhaseRepository.UpdateProductPhaseRange(productPhases);
    }

    private async Task UpdateRealMaterial(List<ShipmentDetail> materialShipments)
    {
        if (materialShipments is null || materialShipments.Count == 0)
        {
            return;
        }

        var materialIds = materialShipments.Select(materialShipments => materialShipments.Id).ToList();

        var materials = await _materialRepository.GetMaterialsByIdsAsync(materialIds);

        if (materials is null || materials.Count != materialShipments.Count)
        {
            throw new ShipmentDetailNotFoundException();
        }

        foreach (var shipmentDetail in materialShipments)
        {
            var material = materials.SingleOrDefault(m => m.Id ==  shipmentDetail.MaterialId);

            if(material is null || material.QuantityInStock < shipmentDetail.Quantity)
            {
                throw new ItemAvailableNotEnoughException();
            }

            material.UpdateQuantityInStock(material.QuantityInStock - shipmentDetail.Quantity);
        }

        _materialRepository.UpdateRange(materials);
    }

    private async Task UpdateRealProductQuantityOfToCompany(List<ShipmentDetail> productShipments, Guid companyId)
    {
        if (productShipments is null || productShipments.Count == 0)
        {
            return;
        }

        var productPhases = await _productPhaseRepository
            .GetProductPhaseByShipmentDetailAsync(productShipments, companyId);

        if(productPhases is null)
        {
            productPhases = new List<ProductPhase>();
        }

        foreach (var shipmentDetail in productShipments)
        {
            var productId = shipmentDetail.ProductId ?? throw new ProductNotFoundException();
            var phaseId = shipmentDetail.PhaseId ?? throw new PhaseNotFoundException();
            var productPhase = productPhases.SingleOrDefault(ph
            => ph.ProductId == productId
                && ph.PhaseId == phaseId
                && ph.CompanyId == companyId);

            if(productPhase is null)
            {
                productPhase = ProductPhase.Create(
                    new CreateProductPhaseRequest(productId, phaseId, (int) shipmentDetail.Quantity, (int)shipmentDetail.Quantity, companyId));
            }
            else
            {
                productPhase.UpdateQuantity(productPhase.Quantity + (int) shipmentDetail.Quantity);
            }
        }
    }

    private async Task UpdateRealProductQuantityOfFromCompany(
        List<ShipmentDetail> productShipments, 
        Guid companyId)
    {
        if (productShipments is null || productShipments.Count == 0)
        {
            return;
        }

        var productPhases = await _productPhaseRepository
            .GetProductPhaseByShipmentDetailAsync(productShipments, companyId);

        if(productPhases is null || productPhases.Count != productShipments.Count)
        {
            throw new ShipmentDetailNotFoundException();
        }

        foreach(var shipmentDetail in productShipments)
        {
            var productPhase = productPhases.SingleOrDefault(ph
            => ph.ProductId == shipmentDetail.ProductId
                && ph.PhaseId == shipmentDetail.PhaseId
                && ph.CompanyId == companyId);

            if (productPhase is null || productPhase.Quantity < shipmentDetail.Quantity)
            {
                throw new ItemAvailableNotEnoughException();
            }

            productPhase.UpdateQuantity(productPhase.Quantity - (int) shipmentDetail.Quantity);
        }

        _productPhaseRepository.UpdateProductPhaseRange(productPhases);
    }

    private async Task<Shipment> GetAndValidateInput(UpdateStatusRequest updateRequest, Guid shipmentId)
    {
        if (shipmentId != updateRequest.ShipmentId)
        {
            throw new ShipmentIdConflictException();
        }

        if (!Enum.IsDefined(typeof(Status), updateRequest.Status))
        {
            throw new ShipmentStatusNotFoundException();
        }

        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId);
        if (shipment == null)
        {
            throw new ShipmentNotFoundException();
        }

        if (shipment.Status == Status.SHIPPED || shipment.Status == Status.CANCEL)
        {
            throw new ShipmentAlreadyDoneException();
        }

        return shipment;
    }
}
