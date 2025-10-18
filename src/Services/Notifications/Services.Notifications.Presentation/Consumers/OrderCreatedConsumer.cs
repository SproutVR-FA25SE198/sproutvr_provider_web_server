using Common.Application.Contracts.Orders;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OrganizationAccountsService;
using Services.Notifications.Application.Abstractions.Grpc;
using Services.Notifications.Application.Abstractions.Services;
using Services.Notifications.Application.BusinessLogics.Emails;
using Services.Notifications.Presentation.Hubs;

namespace Services.Notifications.Presentation.Consumers;

#pragma warning disable CA1515 // Consider making public types internal
public sealed class OrderCreatedConsumer : IConsumer<OrderCreatedMessage>
#pragma warning restore CA1515 // Consider making public types internal
{
    private readonly IGrpcOrganizationClient _grpcOrganizationClient;
    private readonly IEmailService _emailService;
    private readonly IHubContext<NotificationHub> _hubContext;
    public OrderCreatedConsumer(IGrpcOrganizationClient grpcOrganizationClient, IEmailService emailService, IHubContext<NotificationHub> hubContext)
    {
        _grpcOrganizationClient = grpcOrganizationClient;    
        _emailService = emailService;
        _hubContext = hubContext;
    }
    public async Task Consume(ConsumeContext<OrderCreatedMessage> context)
    {
        OrderCreatedMessage message = context.Message;
        // call account service via grpc to get org email
        GetOrganizationByIdResponse organizationResponse = await _grpcOrganizationClient.GetOrganizationByIdAsync(message.OrganizationId);

        // send email invoice for organization customer
        string emailContent = EmailContentHelper.GetOrderCreatedEmailHtml(message);

        var emailRequest = new SendEmailRequest
        {
            ToEmail = organizationResponse.OrganizationEmail,
            Subject = $"[SproutVR] Order Invoice #ORD{message.OrderCode} - Order Confirmed",
            Body = emailContent
        };

        await _emailService.SendEmailAsync(emailRequest);

        // prepare notification content for system admins
        string notificationContent = NotificationContentHelper.OrderCreatedNotification(message);

        // send real-time notification to all connected system admins via SignalR
        await _hubContext.Clients.All.SendAsync("OrderCreated", notificationContent);

    }
}
