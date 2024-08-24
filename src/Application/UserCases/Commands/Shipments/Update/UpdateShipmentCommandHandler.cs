using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Shipment.Update;
using Contract.Services.ShipmentDetail.Share;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using Domain.Exceptions.Materials;
using Domain.Exceptions.Phases;
using Domain.Exceptions.ProductPhases;
using Domain.Exceptions.ShipmentDetails;
using Domain.Exceptions.Shipments;
using FluentValidation;

namespace Application.UseCases.Commands.Shipments.Update
{
    internal sealed class UpdateShipmentCommandHandler(
        IShipmentRepository shipmentRepository,
        IShipmentDetailRepository shipmentDetailRepository,
        IMaterialRepository materialRepository,
        IPhaseRepository phaseRepository,
        ICompanyRepository companyRepository,
        IProductPhaseRepository productPhaseRepository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateShipmentRequest> validator) : ICommandHandler<UpdateShipmentCommand>
    {
        public async Task<Result.Success> Handle(UpdateShipmentCommand request, CancellationToken cancellationToken)
        {
            var updateRequest = request.UpdateShipmentRequest;

            await ValidateInput(request.Id, updateRequest);

            var shipment = await shipmentRepository.GetByIdAndShipmentDetailAsync(request.Id)
                ?? throw new ShipmentNotFoundException();

            await UpdateShipment(shipment, updateRequest, request.UpdatedBy);
            await unitOfWork.SaveChangesAsync();

            return Result.Success.Update();
        }

        private async Task ValidateInput(Guid id, UpdateShipmentRequest updateShipmentRequest)
        {
            if (updateShipmentRequest.ShipmentId != id)
            {
                throw new ShipmentIdConflictException();
            }

            var validatorResult = await validator.ValidateAsync(updateShipmentRequest);

            if (!validatorResult.IsValid)
            {
                throw new MyValidationException(validatorResult.ToDictionary());
            }
        }

        private List<Guid> GetAllProductIds(UpdateShipmentRequest updateShipmentRequest, List<ShipmentDetail> shipmentDetails)
        {
            var oldIds = shipmentDetails
                .Where(detail => detail.ProductId != null && detail.ProductId != Guid.Empty)
                .Select(detail => (Guid)detail.ProductId)
                .ToList() ?? new List<Guid>();

            var newIds = updateShipmentRequest.ShipmentDetailRequests
                .Where(req => req.KindOfShip == KindOfShip.SHIP_FACTORY_PRODUCT)
                .Select(req => req.ItemId)
                .ToList() ?? new List<Guid>();

            return oldIds.Union(newIds).ToList();
        }

        private List<Guid> GetAllMaterialIds(UpdateShipmentRequest updateShipmentRequest, List<ShipmentDetail> shipmentDetails)
        {
            var oldIds = shipmentDetails
                .Where(detail => detail.MaterialId != null && detail.MaterialId != Guid.Empty)
                .Select(detail => (Guid)detail.MaterialId)
                .ToList() ?? new List<Guid>();

            var newIds = updateShipmentRequest.ShipmentDetailRequests
                .Where(req => req.KindOfShip == KindOfShip.SHIP_FACTORY_MATERIAL)
                .Select(req => req.ItemId)
                .ToList() ?? new List<Guid>();

            return oldIds.Union(newIds).ToList();
        }

        private async Task UpdateShipment(Shipment shipment, UpdateShipmentRequest updateShipmentRequest, string updatedBy)
        {
            if (shipment.FromId == updateShipmentRequest.FromId)
            {
                await UpdateShipmentWithSameFromCompany(shipment, updateShipmentRequest, updatedBy);
            }
            else
            {
                await UpdateShipmentWithoutSameFromCompany(shipment, updateShipmentRequest, updatedBy);
            }

            var newShipmentDetails = CreateNewShipmentDetail(updateShipmentRequest.ShipmentDetailRequests, shipment.Id);

            shipment.Update(updateShipmentRequest, updatedBy, new List<ShipmentDetail>());

            shipmentRepository.Update(shipment);
            shipmentDetailRepository.AddRange(newShipmentDetails);
        }

        private List<ShipmentDetail> CreateNewShipmentDetail(List<ShipmentDetailRequest> shipmentDetailRequests, Guid shipmentId)
        {
            return shipmentDetailRequests.Select(req =>
            {
                return req.KindOfShip switch
                {
                    KindOfShip.SHIP_FACTORY_PRODUCT => ShipmentDetail.CreateShipmentProductDetail(shipmentId, req),
                    KindOfShip.SHIP_FACTORY_MATERIAL => ShipmentDetail.CreateShipmentMaterialDetail(shipmentId, req),
                    _ => throw new KindOfShipNotFoundException(),
                };
            }).ToList();
        }

        private async Task UpdateShipmentWithSameFromCompany(Shipment shipment, UpdateShipmentRequest updateShipmentRequest, string updatedBy)
        {
            var isFromCompanyThirdParty = await companyRepository.IsThirdPartyCompanyAsync(updateShipmentRequest.FromId);

            var shipmentDetails = shipment.ShipmentDetails ?? throw new ShipmentDetailNotFoundException();

            var allMaterialIds = GetAllMaterialIds(updateShipmentRequest, shipmentDetails);
            if (isFromCompanyThirdParty && allMaterialIds is not null && allMaterialIds.Count > 0)
            {
                throw new ShipmentBadRequestException("Third-party companies cannot send materials.");
            }

            if (allMaterialIds is not null && allMaterialIds.Count > 0)
            {
                var materials = await materialRepository.GetMaterialsByIdsAsync(allMaterialIds);

                foreach (var detail in shipmentDetails)
                {
                    if (detail.MaterialId is not null)
                    {
                        var material = materials.SingleOrDefault(material => material.Id == detail.MaterialId)
                            ?? throw new MaterialNotFoundException();

                        material.UpdateAvailableQuantity(material.AvailableQuantity + detail.Quantity);
                    }
                }

                foreach (var req in updateShipmentRequest.ShipmentDetailRequests)
                {
                    if (req.KindOfShip == KindOfShip.SHIP_FACTORY_MATERIAL)
                    {
                        var material = materials.SingleOrDefault(material => material.Id == req.ItemId)
                           ?? throw new MaterialNotFoundException();

                        var materialAvailableQuantity = material.AvailableQuantity - req.Quantity;

                        if (materialAvailableQuantity < 0) throw new ShipmentBadRequestException($"Not enough material {req.ItemId}.");

                        material.UpdateAvailableQuantity(materialAvailableQuantity);
                    }
                }

                materialRepository.UpdateRange(materials);
            }

            var allProductIds = GetAllProductIds(updateShipmentRequest, shipmentDetails);
            if (allProductIds is not null && allProductIds.Count > 0)
            {
                var productPhases = await productPhaseRepository.GetByProductIdsAndCompanyIdAsync(allProductIds, updateShipmentRequest.FromId);

                if (isFromCompanyThirdParty)
                {
                    var phase = await phaseRepository.GetPhaseByName("PH_001") ?? throw new PhaseNotFoundException();
                    foreach (var detail in shipmentDetails)
                    {
                        if (detail.ProductId is not null && detail.PhaseId is not null)
                        {
                            var productPhase = productPhases.SingleOrDefault(p => p.ProductId == detail.ProductId && p.PhaseId == phase.Id)
                                 ?? throw new ProductPhaseNotFoundException();

                            productPhase.UpdateAvailableQuantity(productPhase.AvailableQuantity + (int)detail.Quantity);
                        }
                    }

                    foreach (var request in updateShipmentRequest.ShipmentDetailRequests)
                    {
                        if (request.KindOfShip == KindOfShip.SHIP_FACTORY_PRODUCT)
                        {
                            var products = productPhases.Where(ph => ph.ProductId == request.ItemId).ToList();

                            var totalQuantity = products.Sum(ph => ph.AvailableQuantity + ph.ErrorAvailableQuantity);

                            if (request.Quantity > totalQuantity)
                            {
                                throw new ItemAvailableNotEnoughException();
                            }

                            int remainingQuantity = (int)request.Quantity;

                            foreach (var ph in products)
                            {
                                remainingQuantity = UpdateQuantity(ph, remainingQuantity);
                                if (remainingQuantity == 0)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (var detail in shipmentDetails)
                    {
                        if (detail.ProductId is not null)
                        {
                            var productPhase = productPhases.SingleOrDefault(p => p.ProductId == detail.ProductId && p.PhaseId == detail.PhaseId)
                                ?? throw new ProductPhaseNotFoundException();

                            switch (detail.ProductPhaseType)
                            {
                                case ProductPhaseType.NO_PROBLEM:
                                    productPhase.UpdateAvailableQuantity(productPhase.AvailableQuantity + (int)detail.Quantity);
                                    break;
                                case ProductPhaseType.THIRD_PARTY_ERROR:
                                    productPhase.UpdateErrorAvailableQuantity(productPhase.ErrorAvailableQuantity + (int)detail.Quantity);
                                    break;
                                default:
                                    throw new ShipmentBadRequestException("Các lô hàng từ cơ sở phải là sản phẩm không bị lỗi hoặc sản phẩm bị lỗi của bên thứ ba.");
                            }
                        }
                    }

                    foreach (var request in updateShipmentRequest.ShipmentDetailRequests)
                    {
                        if (request.KindOfShip == KindOfShip.SHIP_FACTORY_PRODUCT)
                        {
                            var productPhase = productPhases.SingleOrDefault(p => p.ProductId == request.ItemId && p.PhaseId == request.PhaseId)
                                 ?? throw new ProductPhaseNotFoundException();

                            switch (request.ProductPhaseType)
                            {
                                case ProductPhaseType.NO_PROBLEM:
                                    int availableQuantity = productPhase.AvailableQuantity - (int)request.Quantity;
                                    if (availableQuantity < 0) throw new ShipmentBadRequestException($"Không có đủ sản phẩm có id: {request.ItemId} của giai đoạn {request.PhaseId} trong kho.");
                                    productPhase.UpdateAvailableQuantity(availableQuantity);
                                    break;
                                case ProductPhaseType.THIRD_PARTY_ERROR:
                                    int errorAvailableQuantity = productPhase.ErrorAvailableQuantity - (int)request.Quantity;
                                    if (errorAvailableQuantity < 0) throw new ShipmentBadRequestException($"Không có đủ sản phẩm lỗi có id: {request.ItemId} của giai đoạn {request.PhaseId} trong kho.");
                                    productPhase.UpdateErrorAvailableQuantity(errorAvailableQuantity);
                                    break;
                                default:
                                    throw new ShipmentBadRequestException("Các lô hàng từ cơ sở phải là sản phẩm không bị lỗi hoặc sản phẩm bị lỗi của bên thứ ba.");
                            }
                        }
                    }
                }

                productPhaseRepository.UpdateProductPhaseRange(productPhases);
            }
        }

        private async Task UpdateShipmentWithoutSameFromCompany(Shipment shipment, UpdateShipmentRequest updateShipmentRequest, string updatedBy)
        {
            





            //var allMaterialIds = GetAllMaterialIds(updateShipmentRequest, shipmentDetails);
            //if (allMaterialIds is not null && allMaterialIds.Count > 0)
            //{
            //    var materials = await materialRepository.GetMaterialsByIdsAsync(allMaterialIds);

            //    foreach (var detail in shipmentDetails)
            //    {
            //        if (detail.MaterialId is not null)
            //        {
            //            var material = materials.SingleOrDefault(material => material.Id == detail.MaterialId)
            //                ?? throw new MaterialNotFoundException();

            //            material.UpdateAvailableQuantity(material.AvailableQuantity + detail.Quantity);
            //        }
            //    }

            //    materialRepository.UpdateRange(materials);
            //}

            //var allProductIds = GetAllProductIds(updateShipmentRequest, shipmentDetails);
            //if (allProductIds is not null && allProductIds.Count > 0)
            //{
            //    var productPhases = await productPhaseRepository.GetByProductIdsAndCompanyIdAsync(allProductIds, shipment.FromId);

            //    foreach (var detail in shipmentDetails)
            //    {
            //        if (detail.ProductId is not null)
            //        {
            //            var productPhase = productPhases.SingleOrDefault(p => p.ProductId == detail.ProductId && p.PhaseId == detail.PhaseId)
            //                ?? throw new ProductPhaseNotFoundException();

            //            switch (detail.ProductPhaseType)
            //            {
            //                case ProductPhaseType.NO_PROBLEM:
            //                    productPhase.UpdateAvailableQuantity(productPhase.AvailableQuantity + (int)detail.Quantity);
            //                    break;
            //                case ProductPhaseType.THIRD_PARTY_ERROR:
            //                    productPhase.UpdateErrorAvailableQuantity(productPhase.ErrorAvailableQuantity + (int)detail.Quantity);
            //                    break;
            //                default:
            //                    throw new ShipmentBadRequestException("Các lô hàng từ cơ sở phải là sản phẩm không bị lỗi hoặc sản phẩm bị lỗi của bên thứ ba.");
            //            }
            //        }
            //    }

            //    productPhaseRepository.UpdateProductPhaseRange(productPhases);
            //}
        }

        private int UpdateQuantity(ProductPhase productPhase, int remainingQuantity)
        {
            if (remainingQuantity == 0)
            {
                return 0;
            }

            if (productPhase.AvailableQuantity >= remainingQuantity)
            {
                productPhase.UpdateAvailableQuantity(productPhase.AvailableQuantity - remainingQuantity);
                return 0;
            }
            else
            {
                remainingQuantity -= productPhase.AvailableQuantity;
                productPhase.UpdateAvailableQuantity(0);
                return remainingQuantity;
            }
        }
    }
}
