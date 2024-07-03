﻿using Contract.Services.Shipment.Share;
using Contract.Services.ShipOrder.Create;
using Contract.Services.ShipOrder.Share;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class ShipOrder : EntityAuditBase<Guid>
{
    public Guid OrderId { get; private set; }
    public Order Order { get; private set; }    
    public string ShipperId { get; private set; }
    public User Shipper { get; private set; }
    public DateTime ShipDate { get; private set; }
    public Status Status { get; private set; }
    public DeliveryMethod DeliveryMethod { get; private set; }
    public List<ShipOrderDetail>? ShipOrderDetails { get; private set; }

    private ShipOrder()
    {
    }

    public static ShipOrder Create(string createdBy, CreateShipOrderRequest request)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            CreatedBy = createdBy,
            CreatedDate = DateTime.UtcNow,
            OrderId = request.OrderId,
            ShipperId = request.ShipperId,
            ShipDate = request.ShipDate,
            Status = Status.WAIT_FOR_SHIP,
            DeliveryMethod = request.KindOfShipOrder
        };
    }
}
