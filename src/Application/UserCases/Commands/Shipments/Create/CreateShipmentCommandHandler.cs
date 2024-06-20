using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Shipment.Create;
using Contract.Services.ShipmentDetail.Share;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using Domain.Exceptions.ShipmentDetails;
using FluentValidation;
using MediatR;

namespace Application.UserCases.Commands.Shipments.Create;

internal sealed class CreateShipmentCommandHandler(
    IShipmentRepository _shipmentRepository,
    IShipmentDetailRepository _shipmentDetailRepository,
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

        var shipmentDetails = CreateShipmentDetails(createShipmentRequest.ShipmentDetailRequests, shipment.Id);

        // Change quantity in product phase
        // Nếu from Id là third party company và to Id là factory
        // thì sẽ trừ các phase cũ và cộng các phase mới

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

    private List<ShipmentDetail> CreateShipmentDetails(
        List<ShipmentDetailRequest> shipmentDetailRequests,
        Guid shipmentId)
    {
        return shipmentDetailRequests.ConvertAll(s => CreateShipmentDetail(s, shipmentId)).ToList();
    }

    private Shipment CreateShipment(CreateShipmentRequest createShipmentRequest, string createdBy)
    {
        return Shipment.Create(
            createShipmentRequest.FromId,
            createShipmentRequest.ToId,
            createdBy);
    }

    private ShipmentDetail CreateShipmentDetail(ShipmentDetailRequest request, Guid shipmentId)
    {
        switch (request.KindOfShip)
        {
            case KindOfShip.SHIP_FACTORY_PRODUCT:
                return ShipmentDetail.CreateShipmentProductDetail(shipmentId, request.ItemId, request.PhaseId, request.Quantity);

            case KindOfShip.SHIP_FACTORY_SET:
                return ShipmentDetail.CreateShipmentSetDetail(shipmentId, request.ItemId, request.Quantity);

            case KindOfShip.SHIP_FACTORY_MATERIAL:
                return ShipmentDetail.CreateShipmentMaterialDetail (shipmentId, request.ItemId, request.Quantity);

            default:
                throw new KindOfShipNotFoundException();
        }
    }
}
