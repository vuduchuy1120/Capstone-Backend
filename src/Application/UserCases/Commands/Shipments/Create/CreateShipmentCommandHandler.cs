using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Shipment.Create;
using Contract.Services.ShipmentDetail.Share;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using Domain.Exceptions.Materials;
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
    ICompanyRepository _companyRepository,
    IMaterialRepository _materialRepository,
    IUnitOfWork _unitOfWork,
    IValidator<CreateShipmentRequest> _validator) : ICommandHandler<CreateShipmentCommand>
{
    public async Task<Result.Success> Handle(
        CreateShipmentCommand request,
        CancellationToken cancellationToken)
    {
        var createShipmentRequest = request.CreateShipmentRequest;
        await ValidateRequest(createShipmentRequest);

        var shipment = Shipment.Create(createShipmentRequest, request.CreatedBy);

        var shipmentDetails = await CreateShipmentDetails(
            createShipmentRequest.ShipmentDetailRequests,
            shipment.Id,
            createShipmentRequest.FromId);

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
        Guid shipmentId,
        Guid fromCompany)
    {
        var shipmentDetailTasks = shipmentDetailRequests
            .Select(detailRequest => CreateShipmentDetail(detailRequest, shipmentId, fromCompany));
        var shipmentDetails = await Task.WhenAll(shipmentDetailTasks);

        return shipmentDetails.ToList();
    }

    private async Task<ShipmentDetail> CreateShipmentDetail(ShipmentDetailRequest request, Guid shipmentId, Guid fromCompany)
    {
        var isFromCompanyIsThirdPartyCompany = await _companyRepository.IsThirdPartyCompanyAsync(fromCompany);

        if (isFromCompanyIsThirdPartyCompany)
        {
            return await CreateShipmentDetailFromThirdPartyCompany(request, shipmentId, fromCompany);   
        }
        else
        {
            return await CreateShipmentDetailFromFactory(request, shipmentId, fromCompany);
        }
    }

    private async Task<ShipmentDetail> CreateShipmentDetailFromThirdPartyCompany(ShipmentDetailRequest request, Guid shipmentId, Guid fromCompany)
    {
        switch(request.KindOfShip)
        {
            case KindOfShip.SHIP_FACTORY_PRODUCT:
                var phaseId = request.PhaseId ?? throw new ProductPhaseNotFoundException();
                var productPhase = await _productPhaseRepository
                .GetByProductIdPhaseIdAndCompanyIdAsync(request.ItemId, phaseId, fromCompany)
                    ?? throw new ProductPhaseNotFoundException();

                if(request.ProductPhaseType == ProductPhaseType.NO_PROBLEM)
                {
                    if (productPhase.AvailableQuantity < request.Quantity)
                    {
                        throw new ItemAvailableNotEnoughException();
                    }

                    productPhase.UpdateAvailableQuantity(productPhase.AvailableQuantity - (int)request.Quantity);
                }
                else if(request.ProductPhaseType == ProductPhaseType.FACTORY_ERROR)
                {
                    if (productPhase.FailureAvailabeQuantity < request.Quantity)
                    {
                        throw new ItemAvailableNotEnoughException();
                    }

                    productPhase.UpdateFailureAvailableQuantity(productPhase.FailureAvailabeQuantity - (int)request.Quantity);
                }
                else if (request.ProductPhaseType == ProductPhaseType.THIRD_PARTY_NO_FIX_ERROR)
                {
                    if (productPhase.BrokenAvailableQuantity < request.Quantity)
                    {
                        throw new ItemAvailableNotEnoughException();
                    }

                    productPhase.UpdateBrokenAvailableQuantity(productPhase.BrokenAvailableQuantity - (int)request.Quantity);
                }
                else
                {
                    throw new ShipmentBadRequestException
                        ("Khi tạo đơn hàng gửi cho bên cơ sở thì chỉ tạo từ sản phẩm bình thường " +
                        "hoặc sản phẩm hỏng do bên cơ sở hoặc sản phẩm hỏng hẳn");
                }

                _productPhaseRepository.UpdateProductPhase(productPhase);

                return ShipmentDetail.CreateShipmentProductDetail(shipmentId, request);

            case KindOfShip.SHIP_FACTORY_MATERIAL:
                throw new ShipmentBadRequestException("Công ty hợp tác bên thứ 3 không gửi được nguyên liệu");

            default:
                throw new KindOfShipNotFoundException();
            }
        } 

    private async Task<ShipmentDetail> CreateShipmentDetailFromFactory(ShipmentDetailRequest request, Guid shipmentId, Guid fromCompany)
    {
        switch (request.KindOfShip)
        {
            case KindOfShip.SHIP_FACTORY_PRODUCT:
                // chi gui duoc hang NO_PROBLEM va hang THIRD_PARTY_ERROR
                var phaseId = request.PhaseId ?? throw new ProductPhaseNotFoundException();
                var productPhase = await _productPhaseRepository
                .GetByProductIdPhaseIdAndCompanyIdAsync(request.ItemId, phaseId, fromCompany)
                    ?? throw new ProductPhaseNotFoundException();

                if(request.ProductPhaseType == ProductPhaseType.NO_PROBLEM)
                {
                    if (productPhase.AvailableQuantity < request.Quantity)
                    {
                        throw new ItemAvailableNotEnoughException();
                    }

                    productPhase.UpdateAvailableQuantity(productPhase.AvailableQuantity - (int)request.Quantity);
                }
                else if(request.ProductPhaseType == ProductPhaseType.THIRD_PARTY_ERROR)
                {
                    if(productPhase.ErrorAvailableQuantity < request.Quantity)
                    {
                        throw new ItemAvailableNotEnoughException();
                    }

                    productPhase.UpdateErrorAvailableQuantity(productPhase.ErrorAvailableQuantity - (int)request.Quantity);
                }
                else
                {
                    throw new ShipmentBadRequestException
                        ("Khi tạo đơn hàng gửi cho bên thứ 3 thì chỉ tạo từ sản phẩm bình thường hoặc sản phẩm hỏng do bên thứ 3");
                }

                _productPhaseRepository.UpdateProductPhase(productPhase);

                return ShipmentDetail.CreateShipmentProductDetail(shipmentId, request);

            case KindOfShip.SHIP_FACTORY_MATERIAL:
                var material = await _materialRepository.GetMaterialByIdAsync(request.ItemId)
                    ?? throw new MaterialNotFoundException();

                if (material.AvailableQuantity < request.Quantity)
                {
                    throw new ItemAvailableNotEnoughException();
                }

                material.UpdateAvailableQuantity(material.AvailableQuantity - request.Quantity);
                _materialRepository.UpdateMaterial(material);

                return ShipmentDetail.CreateShipmentMaterialDetail(shipmentId, request);

            default:
                throw new KindOfShipNotFoundException();
        }
    }
}
