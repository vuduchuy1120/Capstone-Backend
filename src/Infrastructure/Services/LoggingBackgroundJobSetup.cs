using Microsoft.Extensions.Options;
using Quartz;

namespace Infrastructure.Services;

public class LoggingBackgroundJobSetup : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions options)
    {
        var jobKey = JobKey.Create(nameof(LoggingBackgroundJob));

        options
            .AddJob<LoggingBackgroundJob>(jobBuilder => jobBuilder.WithIdentity(jobKey))
            .AddTrigger(trigger => trigger
                                    .ForJob(jobKey)
                                    .WithIdentity("LoggingBackgroundJob-trigger")
                                    .WithCronSchedule("0 2 5 * * ?"));
    }
}
