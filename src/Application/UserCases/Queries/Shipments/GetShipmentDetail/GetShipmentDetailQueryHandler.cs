using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Company.Shared;
using Contract.Services.Company.ShareDtos;
using Contract.Services.MaterialHistory.ShareDto;
using Contract.Services.Phase.ShareDto;
using Contract.Services.Product.SharedDto;
using Contract.Services.Set.GetSet;
using Contract.Services.Shipment.GetShipmentDetail;
using Contract.Services.Shipment.Share;
using Contract.Services.ShipmentDetail.Share;
using Contract.Services.User.SharedDto;
using Domain.Entities;
using Domain.Exceptions.Companies;
using Domain.Exceptions.MaterialHistories;
using Domain.Exceptions.Phases;
using Domain.Exceptions.Products;
using Domain.Exceptions.Sets;
using Domain.Exceptions.Shipments;

namespace Application.UserCases.Queries.Shipments.GetShipmentDetail;

internal sealed class GetShipmentDetailQueryHandler(
    IShipmentRepository _shipmentRepository,
    ICompanyRepository _companyRepository,
    IProductRepository _productRepository,
    IPhaseRepository _phaseRepository,
    ISetRepository _setRepository,
    IMaterialHistoryRepository _materialHistoryRepository,
    IMapper _mapper) : IQueryHandler<GetShipmentDetailQuery, ShipmentDetailResponse>
{
    public async Task<Result.Success<ShipmentDetailResponse>> Handle(
        GetShipmentDetailQuery request, 
        CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(request.ShipmentId)
            ?? throw new ShipmentNotFoundException();

        var from = await _companyRepository.GetByIdAsync(shipment.FromId)
            ?? throw new CompanyNotFoundException();

        var to = await _companyRepository.GetByIdAsync(shipment.ToId)
            ?? throw new CompanyNotFoundException();

        var fromResponse = _mapper.Map<CompanyResponse>(from);
        var toResponse = _mapper.Map<CompanyResponse>(to);

        var detailsTasks = shipment?.ShipmentDetails?.Select(sd => MapShipmentDetailToResponse(sd)).ToList();
        var details = await Task.WhenAll(detailsTasks);

        var shipper = _mapper.Map<UserResponse>(shipment?.Shipper);

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

            return new DetailResponse(productResponse, phaseResponse, null, null, shipmentDetail.Quantity);
        }
        else if (shipmentDetail.Set is not null)
        {
            var setResponse = _mapper.Map<SetResponse>(shipmentDetail.Set);

            return new DetailResponse(null, null, setResponse, null, shipmentDetail.Quantity);
        }
        else if (shipmentDetail.MaterialHistory is not null)
        {
            var materialHistoryResponse = _mapper.Map<MaterialHistoryResponse>(shipmentDetail.MaterialHistory);

            return new DetailResponse(null, null, null, materialHistoryResponse, shipmentDetail.Quantity);
        }

        throw new ShipDetailItemNullException();
    }

    private async Task<DetailResponse> MapShipmentDetailToResponse_2(ShipmentDetail shipmentDetail)
    {
        if (shipmentDetail.ProductId is not null && shipmentDetail.PhaseId is not null)
        {
            Guid productId = shipmentDetail.ProductId ?? throw new ProductNotFoundException();
            var product = await _productRepository.GetProductById(productId)
                ?? throw new ProductNotFoundException();

            Guid phaseId = shipmentDetail.PhaseId ?? throw new PhaseNotFoundException();
            var phase = await _phaseRepository.GetPhaseById(phaseId)
                ?? throw new PhaseNotFoundException();

            var phaseResponse = _mapper.Map<PhaseResponse>(phase);
            var productResponse = _mapper.Map<ProductResponse>(product);

            return new DetailResponse(productResponse, phaseResponse, null, null, shipmentDetail.Quantity);
        }
        else if (shipmentDetail.SetId is not null)
        {
            Guid setId = shipmentDetail.SetId ?? throw new SetNotFoundException();
            var set = await _setRepository.GetByIdAsync(setId) ?? throw new SetNotFoundException();

            var setResponse = _mapper.Map<SetResponse>(set);

            return new DetailResponse(null, null, setResponse, null, shipmentDetail.Quantity);
        }
        else if (shipmentDetail.MaterialHistoryId is not null)
        {
            var materialHistoryId = shipmentDetail.MaterialHistoryId
                ?? throw new MaterialHistoryNotFoundException();

            var materialHistory = await _materialHistoryRepository
                .GetMaterialHistoryByIdAsync(materialHistoryId)
                ?? throw new MaterialHistoryNotFoundException();

            var materialHistoryResponse = _mapper.Map<MaterialHistoryResponse>(materialHistory);

            return new DetailResponse(null, null, null, materialHistoryResponse, shipmentDetail.Quantity);
        }

        throw new ShipDetailItemNullException();
    }

}
