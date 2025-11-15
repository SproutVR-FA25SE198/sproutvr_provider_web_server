using Common.Application.Contracts.Accounts;
using MassTransit;
using Services.Notifications.Application.Abstractions.Services;
using Services.Notifications.Application.BusinessLogics.Emails;

namespace Services.Notifications.Presentation.Consumers;

#pragma warning disable CA1515 // Consider making public types internal
public class OrganizationRequestCreatedMessageConsumer : IConsumer<OrganizationRequestCreatedMessage>
#pragma warning restore CA1515 // Consider making public types internal
{
    private readonly IEmailService _emailService;
    private readonly ILogger<OrganizationRequestCreatedMessageConsumer> _logger;
    private readonly IConfiguration _configuration;

    public OrganizationRequestCreatedMessageConsumer(
        IEmailService emailService,
        ILogger<OrganizationRequestCreatedMessageConsumer> logger,
        IConfiguration configuration)
    {
        _emailService = emailService;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task Consume(ConsumeContext<OrganizationRequestCreatedMessage> context)
    {
        try
        {
            OrganizationRequestCreatedMessage message = context.Message;
            
            _logger.LogInformation(
                "Processing email verification for {OrganizationName} ({Email})", 
                message.OrganizationName, 
                message.Email);

            // Get client base URL from configuration
            string clientBaseUrl = _configuration["ClientBaseUrl"] ?? "http://localhost:5173";

            string emailContent = EmailContentHelper.GetEmailVerificationHtml(message, clientBaseUrl);

            var emailRequest = new SendEmailRequest
            {
                ToEmail = message.Email,
                Subject = "[SproutVR] Xác Nhận Địa Chỉ Email Của Bạn",
                Body = emailContent
            };

            await _emailService.SendEmailAsync(emailRequest);

            _logger.LogInformation(
                "Successfully sent email verification to {Email}", 
                message.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "Failed to send email verification to {Email}", 
                context.Message.Email);
        }
    }
}

