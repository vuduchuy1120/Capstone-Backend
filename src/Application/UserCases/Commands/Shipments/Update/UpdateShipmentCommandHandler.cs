using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Shipment.Share;
using Contract.Services.Shipment.Update;
using Domain.Exceptions.Shipments;

namespace Application.UserCases.Commands.Shipments.Update;

internal sealed class UpdateShipmentCommandHandler(
    IShipmentRepository _shipmentRepository,
    IShipmentDetailRepository _shipmentDetailRepository, 
    IUnitOfWork _unitOfWork) : ICommandHandler<UpdateShipmentCommand>
{
    public Task<Result.Success> Handle(UpdateShipmentCommand request, CancellationToken cancellationToken)
    {
        var updateRequest = request.UpdateShipmentRequest;
        if (updateRequest.ShipmentId != request.Id)
        {
            throw new ShipmentIdConflictException();
        }

        if (!Enum.IsDefined(typeof(Status), updateRequest.Status))
        {
            throw new ShipmentStatusNotFoundException();
        }

        throw new ShipmentStatusNotFoundException();
    }
}
