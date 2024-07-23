using Application.Abstractions.Data;
using Microsoft.Extensions.Logging;
using Persistence;
using Quartz;

namespace Infrastructure.Services;

public class MonthlyCompanySalaryBackgroundJob : IJob
{
    private readonly ILogger<MonthlyCompanySalaryBackgroundJob> _logger;
    private readonly AppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;


    public MonthlyCompanySalaryBackgroundJob(
        ILogger<MonthlyCompanySalaryBackgroundJob> logger,
        AppDbContext context,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository)

    {
        _logger = logger;
        _context = context;
        _unitOfWork = unitOfWork;
    }
    public Task Execute(IJobExecutionContext context)
    {
        throw new NotImplementedException();
    }
}
