using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Services.Notifications.Application.Abstractions.Services;

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
        await _emailService.SendEmailAsync("me", "me", "me");
    }
}
