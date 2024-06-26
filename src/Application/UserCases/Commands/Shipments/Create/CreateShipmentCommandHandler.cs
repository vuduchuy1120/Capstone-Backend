using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Shipment.Create;
using Contract.Services.ShipmentDetail.Share;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using Domain.Exceptions.ProductPhases;
using Domain.Exceptions.ShipmentDetails;
using Domain.Exceptions.Shipments;
using FluentValidation;
using MediatR;

namespace Application.UserCases.Commands.Shipments.Create;

internal sealed class CreateShipmentCommandHandler(
    IShipmentRepository _shipmentRepository,
    IShipmentDetailRepository _shipmentDetailRepository,
    IProductPhaseRepository _productPhaseRepository,
    IUnitOfWork _unitOfWork,
    IValidator<CreateShipmentRequest> _validator) : ICommandHandler<CreateShipmentCommand>
{
    public async Task<Result.Success> Handle(
        CreateShipmentCommand request, 
        CancellationToken cancellationToken)
    {
        var createShipmentRequest = request.CreateShipmentRequest;
        await ValidateRequest(createShipmentRequest);

        var shipment = CreateShipment(createShipmentRequest, request.CreatedBy);

        var shipmentDetails = await CreateShipmentDetails(createShipmentRequest.ShipmentDetailRequests, shipment.Id);

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
        Guid shipmentId)
    {
        var shipmentDetailTasks = shipmentDetailRequests.Select(detailRequest => CreateShipmentDetail(detailRequest, shipmentId));
        var shipmentDetails = await Task.WhenAll(shipmentDetailTasks);

        return shipmentDetails.ToList();
    }

    private Shipment CreateShipment(CreateShipmentRequest createShipmentRequest, string createdBy)
    {
        return Shipment.Create( createShipmentRequest, createdBy);
    }

    private async Task<ShipmentDetail> CreateShipmentDetail(ShipmentDetailRequest request, Guid shipmentId)
    {
        switch (request.KindOfShip)
        {
            case KindOfShip.SHIP_FACTORY_PRODUCT:
                var phaseId = request.PhaseId ?? throw new ProductPhaseNotFoundException();
                var productPhase = await _productPhaseRepository
                    .GetProductPhaseByPhaseIdAndProductId(request.ItemId, phaseId) 
                    ?? throw new ProductPhaseNotFoundException();

                if(productPhase.AvailableQuantity < request.Quantity)
                {
                    throw new ItemAvailableNotEnoughException();
                }

                productPhase.UpdateAvailableQuantity(productPhase.AvailableQuantity - request.Quantity);

                _productPhaseRepository.UpdateProductPhase(productPhase);

                return ShipmentDetail.CreateShipmentProductDetail(shipmentId, request);

            case KindOfShip.SHIP_FACTORY_MATERIAL:
                return ShipmentDetail.CreateShipmentMaterialDetail (shipmentId, request);

            default:
                throw new KindOfShipNotFoundException();
        }
    }
}
