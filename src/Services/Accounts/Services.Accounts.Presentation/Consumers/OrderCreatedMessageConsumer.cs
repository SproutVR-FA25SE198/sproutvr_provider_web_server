using Common.Application.Contracts.Orders;
using MassTransit;
using MediatR;
using Services.Accounts.Application.BusinessLogics.SystemAdmins.Features.UpdatePendingOrders;

namespace Services.Accounts.Presentation.Consumers;

#pragma warning disable CA1515 // Consider making public types internal
public class OrderCreatedMessageConsumer(IMediator mediator) : IConsumer<OrderCreatedMessage>
#pragma warning restore CA1515 // Consider making public types internal
{
    
    public async Task Consume(ConsumeContext<OrderCreatedMessage> context)
    {
        await mediator.Send(new UpdateSystemAdminPendingOrdersCommand
        {
            SystemAdminId = context.Message.AssignedSystemAdminId,
            IsIncrement = true
        }, context.CancellationToken);
    }
}
