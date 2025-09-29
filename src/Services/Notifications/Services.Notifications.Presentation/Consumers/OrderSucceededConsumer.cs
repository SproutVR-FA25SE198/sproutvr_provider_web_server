using Common.Application.Contracts.Orders;
using MassTransit;
using Quartz;

namespace Services.Notifications.Presentation.Consumers;

internal sealed class OrderSucceededConsumer : IConsumer<OrderSucceededMessage>
{
    private readonly IScheduler _scheduler;

    public OrderSucceededConsumer(IScheduler scheduler)
    {
        _scheduler = scheduler;
    }

    public Task Consume(ConsumeContext<OrderSucceededMessage> context)
    {
        // Create a job and do something with it 
        _scheduler.Clear();
        return Task.CompletedTask;
    }
}
