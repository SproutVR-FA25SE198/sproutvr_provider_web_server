using MediatR;

namespace Services.Orders.Application.BusinessLogics.ActivationKeys.MarkAsDownloaded;
public class MarkAsDownloadCommand(Guid orderItemId, MarkAsDownloadedRequestDto request) : IRequest<Unit>
{
    public Guid OrderItemId { get; set; } = orderItemId;
    public MarkAsDownloadedRequestDto MarkAsDownloadedRequestDto { get; set; } = request;
}
