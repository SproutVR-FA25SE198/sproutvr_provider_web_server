using Common.Application.Contracts.Bundles;
using MassTransit;

namespace Services.Bundles.Presentation.Consumers;

#pragma warning disable CA1515 // Consider making public types internal
internal sealed class BundleUploadedFaultMessageConsumer : IConsumer<Fault<BundleUploadedFaultMessage>>
{
    public async Task Consume(ConsumeContext<Fault<BundleUploadedFaultMessage>> context)
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

#pragma warning restore CA1515 // Consider making public types internal
