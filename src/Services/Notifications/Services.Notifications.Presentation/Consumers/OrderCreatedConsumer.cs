using Common.Application.Contracts.Orders;
using MassTransit;

namespace Services.Notifications.Presentation.Consumers;

internal sealed class OrderCreatedConsumer : IConsumer<OrderCreatedMessage>
{
    public Task Consume(ConsumeContext<OrderCreatedMessage> context)
    {
        throw new NotImplementedException();
    }
}
