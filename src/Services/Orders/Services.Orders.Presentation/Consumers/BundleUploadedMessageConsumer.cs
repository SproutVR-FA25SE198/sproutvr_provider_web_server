using Common.Application.Contracts.Bundles;
using MassTransit;
using MediatR;
using Services.Orders.Application.BusinessLogics.Orders.Features.UpdateOrder;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Presentation.Consumers;

#pragma warning disable CA1515 // Consider making public types internal
public class BundleUploadedMessageConsumer(IMediator mediator) : IConsumer<BundleUploadedMessage>
#pragma warning restore CA1515 // Consider making public types internal
{

    public async Task Consume(ConsumeContext<BundleUploadedMessage> context)
    {
        await mediator.Send(new UpdateOrderCommand
        {
            OrderCode = context.Message.OrderCode,
            Status = OrderStatus.Finished.ToString(),
        }, context.CancellationToken);
    }
}
