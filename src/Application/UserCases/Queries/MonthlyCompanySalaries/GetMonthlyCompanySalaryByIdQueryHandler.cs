using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.MonthlyCompanySalary.Queries;
using Contract.Services.MonthlyCompanySalary.ShareDtos;
using Contract.Services.ShipmentDetail.Share;
using Domain.Entities;
using Domain.Exceptions.MonthlyCompanySalaries;
using Domain.Exceptions.Shipments;

namespace Application.UserCases.Queries.MonthlyCompanySalaries;

internal sealed class GetMonthlyCompanySalaryByIdQueryHandler
    (IMonthlyCompanySalaryRepository _monthlyCompanySalaryRepository,
    IShipmentRepository _shipmentRepository,
    IProductPhaseSalaryRepository _productPhaseSalaryRepository,
    IPhaseRepository _phaseRepository,
    ICloudStorage _cloudStorage
    ) : IQueryHandler<GetMonthlyCompanySalaryByIdQuery, MonthlyCompanySalaryDetailResponse>
{
    public async Task<Result.Success<MonthlyCompanySalaryDetailResponse>> Handle(GetMonthlyCompanySalaryByIdQuery request, CancellationToken cancellationToken)
    {
        var monthlyCompanySalary = await _monthlyCompanySalaryRepository.GetMonthlyCompanySalaryByCompanyIdMonthAndYear(request.CompanyId, request.Month, request.Year);
        var receivedShipments = await _shipmentRepository.GetShipmentByCompanyIdAndMonthAndYearAsync(request.CompanyId, request.Month, request.Year, true);
        var sendShipments = await _shipmentRepository.GetShipmentByCompanyIdAndMonthAndYearAsync(request.CompanyId, request.Month, request.Year, false);
        var productPhaseSalaries = await _productPhaseSalaryRepository.GetAllProductPhaseSalaryAsync();
        var phase1 = await _phaseRepository.GetPhaseByName("PH_001");
        var phaseId1 = phase1.Id;

        var phase2 = await _phaseRepository.GetPhaseByName("PH_002");
        var phaseId2 = phase2.Id;
        if (monthlyCompanySalary == null)
        {
            throw new MonthlyCompanySalaryNotFoundException(request.Month, request.Year);
        }


        if (receivedShipments == null && sendShipments == null)
        {
            throw new ShipmentNotFoundException();
        }

        int monthPrevious = request.Month == 1 ? 12 : request.Month - 1;
        int yearPrevious = request.Month == 1 ? request.Year - 1 : request.Year;


        var receivedShipmentsPre = await _shipmentRepository.GetShipmentByCompanyIdAndMonthAndYearAsync(request.CompanyId, monthPrevious, yearPrevious, true);
        var sendShipmentsPre = await _shipmentRepository.GetShipmentByCompanyIdAndMonthAndYearAsync(request.CompanyId, monthPrevious, yearPrevious, false);


        double totalProduct = 0;
        decimal totalSalaryProduct = 0;

        var productBrokenResponses = new List<ProductExportResponse>();
        double totalBroken = 0;

        var productExportResponses = new List<ProductExportResponse>();
        if (receivedShipments != null)
        {
            productExportResponses = await GetProductExportResponses(receivedShipments, productPhaseSalaries, ProductPhaseType.NO_PROBLEM,phaseId2);
            totalProduct = productExportResponses.Sum(pe => pe.Quantity);
            totalSalaryProduct = productExportResponses.Sum(pe => Decimal.Multiply((decimal)pe.Quantity, pe.Price));

            productBrokenResponses = await GetProductBrokenResponses(receivedShipments, productPhaseSalaries, ProductPhaseType.THIRD_PARTY_NO_FIX_ERROR,phaseId1);
            totalBroken = productBrokenResponses.Sum(pb => pb.Quantity);
        }
        var materialResponses = new List<MaterialExportReponse>();
        double totalMaterial = 0;



        if (sendShipments != null)
        {
            var materialResponseTasks = sendShipments
                .SelectMany(sh => sh.ShipmentDetails)
                .Where(sd => sd.MaterialId.HasValue)
                .Select(async sd => new MaterialExportReponse(
                    MaterialId: sd.MaterialId.Value,
                    MaterialName: sd.Material.Name,
                    MaterialUnit: sd.Material.Unit,
                    MaterialImage: await _cloudStorage.GetSignedUrlAsync(sd.Material.Image),
                    Quantity: sd.Quantity,
                    Price: sd.MaterialPrice
                ));

            materialResponses = (await Task.WhenAll(materialResponseTasks)).ToList();
            totalMaterial = materialResponses.Sum(me => me.Quantity);
        }

        var totalSalaryMaterial = materialResponses.Sum(me => Decimal.Multiply((decimal)me.Quantity, me.Price));
        var totalSalaryBroken = productBrokenResponses.Sum(pb => Decimal.Multiply((decimal)pb.Quantity, pb.Price));
        var totalSalaryTotal = totalSalaryProduct - totalSalaryMaterial - totalSalaryBroken;

        double totalProductPre = 0;
        double totalBrokenPre = 0;


        var productExportResponsesPre = await GetProductExportResponses(receivedShipmentsPre, productPhaseSalaries, ProductPhaseType.NO_PROBLEM,phaseId2);
        if (productExportResponsesPre.Count > 0)
        {
            totalProductPre = productExportResponsesPre.Sum(pe => pe.Quantity);
        }

        var productBrokenResponsesPre = await GetProductBrokenResponses(sendShipmentsPre, productPhaseSalaries, ProductPhaseType.THIRD_PARTY_NO_FIX_ERROR,phaseId1);
        if (productBrokenResponsesPre.Count > 0)
        {
            totalBrokenPre = productBrokenResponsesPre.Sum(pb => pb.Quantity);
        }

        double rateProduct = -999999;
        double rateBroken = -999999;

        if (totalProductPre > 0)
        {
            rateProduct = (totalProduct - totalProductPre) / totalProductPre;
        }

        if (totalBrokenPre > 0)
        {
            rateBroken = (totalBroken - totalBrokenPre) / totalBrokenPre;
        }

        var response = new MonthlyCompanySalaryDetailResponse(
                CompanyId: request.CompanyId,
                Month: request.Month,
                Year: request.Year,
                Salary: monthlyCompanySalary.Salary, // Assign 0 or appropriate logic if required
                Status: monthlyCompanySalary.Status, // Assuming status is pending, update as necessary
                Note: monthlyCompanySalary.Note,
                TotalSalaryProduct: totalSalaryProduct,
                TotalSalaryMaterial: totalSalaryMaterial,
                TotalSalaryBroken: totalSalaryBroken,
                TotalSalaryTotal: totalSalaryTotal,
                TotalProduct: totalProduct,
                TotalMaterial: totalMaterial,
                TotalBroken: totalBroken,
                RateProduct: rateProduct,
                RateBroken: rateBroken,
                MaterialResponses: materialResponses,
                ProductExportResponses: productExportResponses,
                ProductBrokenResponses: productBrokenResponses
            );

        return Result.Success<MonthlyCompanySalaryDetailResponse>.Get(response);

    }

    private async Task<List<ProductExportResponse>> GetProductExportResponses(List<Shipment> shipments, List<ProductPhaseSalary> productPhaseSalaries, ProductPhaseType phaseType, Guid phaseId)
    {
        var shipmentDetails = shipments
            .SelectMany(sh => sh.ShipmentDetails)
            .Where(sd => sd.ProductPhaseType == phaseType && sd.PhaseId == phaseId)
            .Join(productPhaseSalaries,
                sd => (sd.ProductId.Value, sd.PhaseId.Value),
                pps => (pps.ProductId, pps.PhaseId),
                (sd, pps) => new
                {
                    ShipmentDetail = sd,
                    ProductPhaseSalary = pps
                })
            .ToList();

        var productExportResponses = new List<ProductExportResponse>();

        foreach (var item in shipmentDetails)
        {
            var sd = item.ShipmentDetail;
            var pps = item.ProductPhaseSalary;

            var productImage = await _cloudStorage
                .GetSignedUrlAsync(sd.Product.Images
                                    .FirstOrDefault(img => img.IsMainImage)
                                    ?.ImageUrl ?? "ImageNotFound");

            productExportResponses.Add(new ProductExportResponse(
                ProductId: sd.ProductId.Value,
                ProductCode: sd.Product.Code,
                ProductName: sd.Product.Name,
                ProductImage: productImage,
                PhaseId: sd.PhaseId.Value,
                PhaseName: sd.Phase.Name,
                Quantity: sd.Quantity,
                Price: pps.SalaryPerProduct
            ));
        }

        return productExportResponses;
    }

    private async Task<List<ProductExportResponse>> GetProductBrokenResponses(List<Shipment> shipments, List<ProductPhaseSalary> productPhaseSalaries, ProductPhaseType phaseType, Guid phaseIdForPricing)
    {

        // Lấy thông tin các chi tiết giao hàng kèm giá
        var shipmentDetails = shipments
            .SelectMany(sh => sh.ShipmentDetails)
            .Where(sd => sd.ProductPhaseType == phaseType)
            .Join(productPhaseSalaries,
                sd => (sd.ProductId.Value, sd.PhaseId.Value),
                pps => (pps.ProductId, pps.PhaseId),
                (sd, pps) => new
                {
                    ShipmentDetail = sd,
                    ProductPhaseSalary = pps
                })
            .ToList();

        var priceForPhaseId = productPhaseSalaries
            .Where(pps => pps.PhaseId == phaseIdForPricing)
            .ToDictionary(pps => pps.ProductId, pps => pps.SalaryPerProduct);

        var productExportResponses = new List<ProductExportResponse>();

        foreach (var item in shipmentDetails)
        {
            var sd = item.ShipmentDetail;
            var pps = item.ProductPhaseSalary;

            decimal price = priceForPhaseId.ContainsKey(sd.ProductId.Value)
                ? priceForPhaseId[sd.ProductId.Value]
                : pps.SalaryPerProduct;

            var productImage = await _cloudStorage
                .GetSignedUrlAsync(sd.Product.Images
                                    .FirstOrDefault(img => img.IsMainImage)
                                    ?.ImageUrl ?? "ImageNotFound");

            productExportResponses.Add(new ProductExportResponse(
                ProductId: sd.ProductId.Value,
                ProductCode: sd.Product.Code,
                ProductName: sd.Product.Name,
                ProductImage: productImage,
                PhaseId: sd.PhaseId.Value, // Giữ nguyên PhaseId
                PhaseName: sd.Phase.Name, // Giữ nguyên PhaseName
                Quantity: sd.Quantity,
                Price: price // Sử dụng giá từ PhaseId đặc biệt hoặc giá gốc nếu không có
            ));
        }

        return productExportResponses;
    }

}
