using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Shipment.UpdateReturnQuantity;
using Contract.Services.ShipmentDetail.UpdateReturnQuantity;
using Domain.Abstractions.Exceptions;
using Domain.Exceptions.ShipmentDetails;
using Domain.Exceptions.Shipments;
using FluentValidation;

namespace Application.UserCases.Commands.Shipments.UpdateReturnQuantity;

internal sealed class UpdateShipmentReturnQuantityCommandHandler(
    IShipmentRepository _shipmentRepository,
    IShipmentDetailRepository _shipmentDetailRepository, 
    IUnitOfWork _unitOfWork, 
    IValidator<UpdateShipmentReturnQuantityCommand> _validator)
    : ICommandHandler<UpdateShipmentReturnQuantityCommand>
{
    public async Task<Result.Success> Handle(
        UpdateShipmentReturnQuantityCommand request,
        CancellationToken cancellationToken)
    {
        await ValidateRequest(request);

        await GetAndUpdateReturnQuantity(request.ShipmentId, request.UpdateReturnQuantityRequest.UpdateQuantityRequests);
        await UpdateAuditShipment(request.ShipmentId, request.UpdatedBy);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success.Update();
    }

    private async Task ValidateRequest(UpdateShipmentReturnQuantityCommand request)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }
    }

    private async Task GetAndUpdateReturnQuantity(Guid shipmentId, List<UpdateQuantityRequest> updateQuantityRequests)
    {
        var updateReturnQuantityDictionary = updateQuantityRequests.ToDictionary(s => s.ShipDetailId);
        var shipmentDetailIds = updateReturnQuantityDictionary.Keys.ToList();
        var shipmentDetails = await _shipmentDetailRepository.GetByShipmentIdAndIdsAsync(
            shipmentId,
            shipmentDetailIds) ?? throw new ShipmentDetailNotFoundException();

        shipmentDetails.ForEach(shipmentDetail =>
        {
            if (updateReturnQuantityDictionary.TryGetValue(shipmentDetail.Id, out var updateRequest))
            {
                //get and update quantity in product phase
                //shipmentDetail.UpdateReturnQuantity(updateRequest.Quantity);
            }
            else
            {
                throw new ShipmentDetailNotFoundException();
            }
        });

        _shipmentDetailRepository.UpdateRange(shipmentDetails);
    }

    private async Task UpdateAuditShipment(Guid shipmentId, string updatedBy)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId)
            ?? throw new ShipmentNotFoundException();
        shipment.Update(updatedBy);

        _shipmentRepository.Update(shipment);
    }
}
