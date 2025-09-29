using Common.Application.Contracts.Orders;
using MassTransit;

namespace Services.Orders.Presentation.Consumers;

internal sealed class OrderCreatedFaultMessageConsumer : IConsumer<Fault<OrderCreatedFaultMessage>>
{
    public async Task Consume(ConsumeContext<Fault<OrderCreatedFaultMessage>> context)
    {
        ExceptionInfo exception = context.Message.Exceptions[0];

        if (exception.ExceptionType == "System.ArgumentException")
        {
            await context.Publish(context.Message.Message);
        }
        else
        {
            // will implement the rule later
        }
    }
}
