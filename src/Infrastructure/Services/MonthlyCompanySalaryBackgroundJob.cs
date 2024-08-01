using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Services.MonthlyCompanySalary.Creates;
using Microsoft.Extensions.Logging;
using Persistence;
using Quartz;

namespace Infrastructure.Services;

[DisallowConcurrentExecution]
public class MonthlyCompanySalaryBackgroundJob : IJob
{
    private readonly ILogger<MonthlyCompanySalaryBackgroundJob> _logger;
    private readonly AppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommandHandler<CreateMonthlyCompanySalaryCommand> _createMonthlyCompanySalaryCommandHandler;



    public MonthlyCompanySalaryBackgroundJob(
        ILogger<MonthlyCompanySalaryBackgroundJob> logger,
        AppDbContext context,
        IUnitOfWork unitOfWork,
        ICommandHandler<CreateMonthlyCompanySalaryCommand> createMonthlyCompanySalaryCommandHandler)

    {
        _logger = logger;
        _context = context;
        _unitOfWork = unitOfWork;
        _createMonthlyCompanySalaryCommandHandler = createMonthlyCompanySalaryCommandHandler;
    }
    public async Task Execute(IJobExecutionContext context)
    {

        int monthNow = DateTime.Now.Month;
        int monthCalculate = monthNow - 1;
        int yearNow = DateTime.Now.Year;

        if (monthNow == 1)
        {
            monthCalculate = 12;
            yearNow = yearNow - 1;
        }

        var command = new CreateMonthlyCompanySalaryCommand(monthCalculate, yearNow);

        var result = await _createMonthlyCompanySalaryCommandHandler.Handle(command, CancellationToken.None);

        if (result.isSuccess)
        {
            _logger.LogInformation("Monthly Company salaries created successfully for {Month}/{Year}", monthCalculate, yearNow);
        }
        else
        {
            _logger.LogError("Failed to create monthly Company salaries for {Month}/{Year}", monthCalculate, yearNow);
        }
    }
}
