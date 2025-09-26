using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Notifications.Application.Abstractions.Services;

namespace Services.Notifications.Infrastructure.Services;

internal sealed class EmailService : IEmailService
{
    public Task SendEmailAsync(string to, string subject, string body)
    {
        throw new NotImplementedException();
    }
}
