using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Services.MonthEmployeeSalary.Creates;
using Contract.Services.Slot.Create;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Persistence;
using Quartz;

namespace Infrastructure.Services;

[DisallowConcurrentExecution]
public class LoggingBackgroundJob : IJob
{
    private readonly ILogger<LoggingBackgroundJob> _logger;
    private readonly AppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommandHandler<CreateMonthEmployeeSalaryCommand> _createMonthEmployeeSalaryCommandHandler;

    public LoggingBackgroundJob(
        ILogger<LoggingBackgroundJob> logger,
        AppDbContext context,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        ICommandHandler<CreateMonthEmployeeSalaryCommand> createMonthEmployeeSalaryCommandHandler)

    {
        _logger = logger;
        _context = context;
        _unitOfWork = unitOfWork;
        _createMonthEmployeeSalaryCommandHandler = createMonthEmployeeSalaryCommandHandler;
    }
    public async Task Execute(IJobExecutionContext jobContext)
    {
        int monthNow = DateTime.Now.Month;
        int monthCalculate = monthNow - 1;
        int yearNow = DateTime.Now.Year;

        if (monthNow == 1)
        {
            monthCalculate = 12;
            yearNow = yearNow - 1;
        }

        var command = new CreateMonthEmployeeSalaryCommand(monthCalculate, yearNow);

        var result = await _createMonthEmployeeSalaryCommandHandler.Handle(command, CancellationToken.None);

        if (result.isSuccess)
        {
            _logger.LogInformation("Monthly employee salaries created successfully for {Month}/{Year}", monthCalculate, yearNow);
        }
        else
        {
            _logger.LogError("Failed to create monthly employee salaries for {Month}/{Year}", monthCalculate, yearNow);
        }
    }


}
