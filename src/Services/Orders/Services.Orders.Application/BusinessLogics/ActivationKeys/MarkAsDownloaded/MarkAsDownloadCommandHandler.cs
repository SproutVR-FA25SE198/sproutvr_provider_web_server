using Common.Application.Abstractions.Data;
using Common.Domain.Exceptions;
using MediatR;
using Services.Orders.Application.BusinessLogics.OrderItems.Specifications;
using Services.Orders.Domain.Entities.OrderItems;

namespace Services.Orders.Application.BusinessLogics.ActivationKeys.MarkAsDownloaded;
public class MarkAsDownloadCommandHandler(
    IUnitOfWork uow) : IRequestHandler<MarkAsDownloadCommand, Unit>
{
    public async Task<Unit> Handle(MarkAsDownloadCommand request, CancellationToken cancellationToken)
    {
        // Check if request is null
        if (request.MarkAsDownloadedRequestDto == null)
        {
            throw new OperationFailedException("Request payload is null");
        }

        // Check empty
        Guid orgId = request.MarkAsDownloadedRequestDto.OrganizationId;
        if (orgId == Guid.Empty)
        {
            throw new OperationFailedException("Tổ chức không tồn tại");
        }

        // Find the OrderItem, but also include its parent Order
        // to check if the user is allowed to modify it.
        var spec = new OrderItemSpecification(request.OrderItemId, byOrderItemId: true);
        OrderItem orderItem = await uow.Repository<OrderItem>().GetEntityWithSpec(spec);

        if (orderItem == null)
        {
            throw new NotFoundException("Học liệu không được tìm thấy.");
        }

        // SECURITY CHECK:
        // Has the bundle been activated
        if (!orderItem.Order.IsKeyActivated)
        {
            throw new OperationFailedException("Bạn chưa kích hoạt gói học liệu này");
        }

        // Does the organization that owns this item match the user's organization?
        if (orderItem.Order.OrganizationId != orgId)
        {
            throw new OperationFailedException("Bạn không có quyền cập nhật học liệu này.");
        }

        // Mark as downloaded
        // If it's already true, we don't need to do anything.
        if (orderItem.IsDownloaded)
        {
            return Unit.Value;
        }

        orderItem.IsDownloaded = true;
        uow.Repository<OrderItem>().Update(orderItem);
        await uow.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
