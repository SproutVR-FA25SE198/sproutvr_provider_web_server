using Common.Application.Contracts.Maps;
using MassTransit;

namespace Services.Catalogs.Presentation.Consumers;

internal sealed class MapCreatedFaultMessageConsumer : IConsumer<Fault<MapCreatedFaultMessage>>
{
    public async Task Consume(ConsumeContext<Fault<MapCreatedFaultMessage>> context)
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
