using Application.Abstractions.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;

namespace Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                option => option.MaxBatchSize(100)
            );
        });

        services.AddScoped<IUnitOfWork>(option => option.GetRequiredService<AppDbContext>());
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<ISlotRepository, SlotRepository>();
        services.AddScoped<IAttendanceRepository, AttendanceRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductImageRepository, ProductImageRepository>();
        services.AddScoped<IMaterialRepository, MaterialRepository>();
        services.AddScoped<IMaterialHistoryRepository, MaterialHistoryRepository>();
        services.AddScoped<IEmployeeProductRepository, EmployeeProductRepository>();
        services.AddScoped<IPhaseRepository, PhaseRepository>();
        services.AddScoped<ISetProductRepository, SetProductRepository>();
        services.AddScoped<ISetRepository, SetRepository>();
        services.AddScoped<IShipmentRepository, ShipmentRepository>();
        services.AddScoped<IShipmentDetailRepository, ShipmentDetailRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IProductPhaseRepository, ProductPhaseRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IShipOrderRepository, ShipOrderRepository>();
        services.AddScoped<IShipOrderDetailRepository, ShipOrderDetailRepository>();
        services.AddScoped<ISalaryHistoryRepository, SalaryHistoryRepository>();
        services.AddScoped<IProductPhaseSalaryRepository, ProductPhaseSalaryRepository>();
        services.AddScoped<IMonthlyEmployeeSalaryRepository, MonthlyEmployeeSalaryRepository>();
        services.AddScoped<IPaidSalaryRepository, PaidSalaryRepository>();
        services.AddScoped<IMonthlyCompanySalaryRepository, MonthlyCompanySalaryRepository>();
        return services;
    }
}
