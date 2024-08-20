using Infrastructure.Services;
using Microsoft.Extensions.Options;
using Quartz;

namespace Infrastructure.BackgtoundServiceOptions;

public class MonthlyCompanySalaryBackgroundJobSetup : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions options)
    {
        var jobKey = JobKey.Create(nameof(MonthlyCompanySalaryBackgroundJob));

        options
            .AddJob<MonthlyCompanySalaryBackgroundJob>(jobBuilder => jobBuilder.WithIdentity(jobKey))
            .AddTrigger(trigger => trigger
                                    .ForJob(jobKey)
                                    .WithIdentity("MonthlyCompanySalaryBackgroundJob-trigger")
                                    .WithCronSchedule("0 0 2 5 * ?"));
    }
}
