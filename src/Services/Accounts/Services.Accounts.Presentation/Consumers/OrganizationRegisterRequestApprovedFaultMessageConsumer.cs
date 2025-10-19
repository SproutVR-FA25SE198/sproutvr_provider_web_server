using Common.Application.Contracts.Accounts;
using MassTransit;

namespace Services.Accounts.Presentation.Consumers;

internal sealed class OrganizationRegisterRequestApprovedFaultMessageConsumer : IConsumer<Fault<OrganizationRegisterRequestApprovedFaultMessage>>
{
    public async Task Consume(ConsumeContext<Fault<OrganizationRegisterRequestApprovedFaultMessage>> context)
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
