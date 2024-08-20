using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.MonthlyCompanySalary.Creates;
using Contract.Services.ShipmentDetail.Share;
using Domain.Entities;

namespace Application.UserCases.Commands.MonthlyCompanySalaries.Creates;

public sealed class CreateMonthlyCompanySalaryCommandHandler
    (IShipmentRepository _shipmentRepository,
    IMonthlyCompanySalaryRepository _monthlyCompanySalaryRepository,
    IProductPhaseSalaryRepository _productPhaseSalaryRepository,
    ICompanyRepository _companyRepository,
    IPhaseRepository _phaseRepository,
    IUnitOfWork _unitOfWork
    ) : ICommandHandler<CreateMonthlyCompanySalaryCommand>
{
    public async Task<Result.Success> Handle(CreateMonthlyCompanySalaryCommand request, CancellationToken cancellationToken)
    {
        var thirdPartyCompanies = await _companyRepository.GetThirdPartyCompany();
        var companyIds = thirdPartyCompanies.Select(x => x.Id).ToList();
        int month = request.Month;
        int year = request.Year;

        var monthlyCompanySalaries = new List<MonthlyCompanySalary>();

        foreach (var companyId in companyIds)
        {
            var receivedShipments = await _shipmentRepository.GetShipmentByCompanyIdAndMonthAndYearAsync(companyId, month, year, true);
            var sendShipments = await _shipmentRepository.GetShipmentByCompanyIdAndMonthAndYearAsync(companyId, month, year, false);
            var productPhaseSalaries = await _productPhaseSalaryRepository.GetAllProductPhaseSalaryAsync();
            var phase1 = await _phaseRepository.GetPhaseByName("PH_001");
            var phaseId1 = phase1.Id;
            var phase2 = await _phaseRepository.GetPhaseByName("PH_002");
            var phaseId2 = phase2.Id;
            decimal materialPrice = 0;
            decimal productSalary = 0;
            decimal productBrokenSalary = 0;
            decimal totalSalary = 0;


            if (receivedShipments != null)
            {
                var shipmentDetails = receivedShipments
                    .SelectMany(x => x.ShipmentDetails).Where(x => x.ProductPhaseType == ProductPhaseType.NO_PROBLEM && x.PhaseId == phaseId2)
                    .ToList();

                if (shipmentDetails.Any())
                {
                    productSalary = CalculateProductSalary(shipmentDetails, productPhaseSalaries);
                }

                var productBrokenShipmentDetails = receivedShipments
                    .SelectMany(x => x.ShipmentDetails)
                    .Where(x => x.ProductPhaseType == ProductPhaseType.THIRD_PARTY_NO_FIX_ERROR).ToList();
                if (productBrokenShipmentDetails.Any())
                {
                    productBrokenSalary = CalculateProductSalary(productBrokenShipmentDetails, productPhaseSalaries, phaseId1);
                }

            }
            if (sendShipments != null)
            {
                var shipmentDetails = sendShipments
                    .SelectMany(x => x.ShipmentDetails)
                    .Where(x => x.ProductPhaseType == ProductPhaseType.NO_PROBLEM).ToList();

                if (shipmentDetails.Any())
                {
                    materialPrice = CalculateMaterial(shipmentDetails);
                }
            }

            totalSalary = productSalary - materialPrice - productBrokenSalary;

            var monthlyCompanySalary = MonthlyCompanySalary
                    .Create(new CreateMonthlyCompanySalaryRequest
                                (
                                    CompanyId: companyId,
                                    Month: month,
                                    Year: year,
                                    Salary: totalSalary
                                ));
            monthlyCompanySalaries.Add(monthlyCompanySalary);
        }

        _monthlyCompanySalaryRepository.AddRange(monthlyCompanySalaries);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success.Create();
    }

    private decimal CalculateProductSalary(List<ShipmentDetail> shipmentDetails, List<ProductPhaseSalary> productPhaseSalaries)
    {
        return shipmentDetails
            .Join<ShipmentDetail, ProductPhaseSalary, (Guid, Guid), (double, decimal)>(
                productPhaseSalaries,
                sd => (sd.ProductId.Value, sd.PhaseId.Value),
                pps => (pps.ProductId, pps.PhaseId),
                (sd, pps) => (sd.Quantity, pps.SalaryPerProduct)
            )
            .Where(x => x.Item1 > 0 && x.Item2 > 0 && decimal.Multiply((decimal)x.Item1, x.Item2) > 0)
            .Sum(x => decimal.Multiply((decimal)x.Item1, x.Item2));
    }

    private decimal CalculateProductSalary(List<ShipmentDetail> shipmentDetails, List<ProductPhaseSalary> productPhaseSalaries, Guid phaseId)
    {
        // Nhóm các quantity theo ProductId
        var groupedShipmentDetails = shipmentDetails
            .GroupBy(sd => sd.ProductId.Value)
            .Select(g => new
            {
                ProductId = g.Key,
                TotalQuantity = g.Sum(sd => sd.Quantity)
            })
            .ToList();

        // Tính toán lại như cũ
        return groupedShipmentDetails
            .Join(
                productPhaseSalaries,
                sd => (sd.ProductId, phaseId),
                pps => (pps.ProductId, pps.PhaseId),
                (sd, pps) => (sd.TotalQuantity, pps.SalaryPerProduct)
            )
            .Where(x => x.TotalQuantity > 0 && x.SalaryPerProduct > 0)
            .Sum(x => decimal.Multiply((decimal)x.TotalQuantity, x.SalaryPerProduct));
    }

    private decimal CalculateMaterial(List<ShipmentDetail> shipmentDetails)
    {
        return shipmentDetails
            .Where(x => x.MaterialId.HasValue)
            .Sum(x => decimal.Multiply(x.MaterialPrice, (decimal)x.Quantity));
    }
}

