﻿using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Company.ShareDtos;
using Contract.Services.Material.ShareDto;
using Contract.Services.MaterialHistory.ShareDto;
using Contract.Services.Phase.ShareDto;
using Contract.Services.Product.SharedDto;
using Contract.Services.Shipment.GetShipmentDetail;
using Contract.Services.Shipment.Share;
using Contract.Services.Shipment.ShipperGetShipmentDetail;
using Contract.Services.ShipmentDetail.Share;
using Contract.Services.User.SharedDto;
using Domain.Entities;
using Domain.Exceptions.Shipments;

namespace Application.UserCases.Queries.Shipments.ShipperGetShipmentDetail;

internal sealed class ShipperGetShipmentDetailQueryHandler(
    IShipmentRepository _shipmentRepository,
    ICompanyRepository _companyRepository,
    IMapper _mapper) : IQueryHandler<ShipperGetShipmentDetailQuery, ShipmentDetailResponse>
{
    public async Task<Result.Success<ShipmentDetailResponse>> Handle(ShipperGetShipmentDetailQuery request, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetByIdAndShipperIdAsync(request.shipmentId, request.shipperId)
             ?? throw new ShipmentNotFoundException();

        var fromResponse = _mapper.Map<CompanyResponse>(shipment?.FromCompany);
        var toResponse = _mapper.Map<CompanyResponse>(shipment?.ToCompany);

        var detailsTasks = shipment?.ShipmentDetails?.Select(sd => MapShipmentDetailToResponse(sd)).ToList();
        var details = await Task.WhenAll(detailsTasks);

        var shipper = _mapper.Map<UserResponseWithoutSalary>(shipment?.Shipper);

        var shipmentDetailResponse = new ShipmentDetailResponse(
            fromResponse,
            toResponse,
            shipper,
            shipment.ShipDate,
            shipment.Status.GetDescription(),
            shipment.Status,
            details.ToList()
        );

        return Result.Success<ShipmentDetailResponse>.Get(shipmentDetailResponse);
    }

    private async Task<DetailResponse> MapShipmentDetailToResponse(ShipmentDetail shipmentDetail)
    {
        if (shipmentDetail.Product is not null && shipmentDetail.Phase is not null)
        {
            var phaseResponse = _mapper.Map<PhaseResponse>(shipmentDetail.Phase);
            var productResponse = _mapper.Map<ProductResponse>(shipmentDetail.Product);

            return new DetailResponse(
                productResponse,
                phaseResponse,
                null,
                shipmentDetail.Quantity,
                shipmentDetail.ProductPhaseType,
                shipmentDetail.ProductPhaseType.GetDescription());
        }
        else if (shipmentDetail.Material is not null)
        {
            var materialResponse = _mapper.Map<MaterialResponse>(shipmentDetail.Material);

            return new DetailResponse(
                null,
                null,
                materialResponse,
                shipmentDetail.Quantity,
                shipmentDetail.ProductPhaseType,
                shipmentDetail.ProductPhaseType.GetDescription());
        }

        throw new ShipDetailItemNullException();
    }
}