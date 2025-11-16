using Common.Application.Contracts.Bundles;
using MassTransit;
using MediatR;
using Services.Accounts.Application.BusinessLogics.SystemAdmins.Features.UpdateSystemAdminPendingOrders;

namespace Services.Accounts.Presentation.Consumers;

#pragma warning disable CA1515 // Consider making public types internal
public class BundleUploadedMessageConsumer(IMediator mediator) : IConsumer<BundleUploadedMessage>
#pragma warning restore CA1515 // Consider making public types internal
{
    public async Task Consume(ConsumeContext<BundleUploadedMessage> context)
    {
        await mediator.Send(new UpdateSystemAdminPendingOrdersCommand() 
        { 
            SystemAdminId = Guid.Parse(context.Message.AssignedSystemAdminId), 
            IsIncrement = false 
        });
    }
}
