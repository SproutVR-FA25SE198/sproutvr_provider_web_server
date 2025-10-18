using Quartz;
using Services.Notifications.Application.Abstractions.Services;
using Services.Notifications.Application.BusinessLogics.Emails;

namespace Services.Notifications.Infrastructure.Jobs;
public class EmailJob : IJob
{
    private readonly IEmailService _emailService;

    public EmailJob(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _emailService.SendEmailAsync(new SendEmailRequest("me", "test", "test"));
    }
}
