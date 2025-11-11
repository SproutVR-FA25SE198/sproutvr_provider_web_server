using Common.Application.Abstractions.Data;
using Common.Domain.Exceptions;
using MediatR;
using Services.Orders.Application.BusinessLogics.Orders.Specifications;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Application.BusinessLogics.ActivationKeys.Features.ValidateActivationKey;
public class ValidateActivationKeyCommandHandler(
    IUnitOfWork uow) : IRequestHandler<ValidateActivationKeyCommand, bool>
{
    public async Task<bool> Handle(ValidateActivationKeyCommand request, CancellationToken cancellationToken)
    {
        // Validate input
        ActivationRequestDto activationRequest = request.ActivationRequest
            ?? throw new OperationFailedException("Thiếu dữ liệu kích hoạt");

        // Check if key is null or empty
        if (string.IsNullOrEmpty(activationRequest.ActivationKey))
        {
            throw new OperationFailedException("Mã kích hoạt đang để trống");
        }

        // Get Order by activation key
        var spec = new OrderSpecification(request.ActivationRequest.ActivationKey);
        Order order = await uow.Repository<Order>().GetEntityWithSpec(spec);

        // Check if key is valid
        if (order == null)
        {
            throw new OperationFailedException("Mã kích hoạt không hợp lệ");
        }

        // SECURITY CHECK 1: Does the Organization ID that OWNS this key
        // in our database match the Organization ID the user CLAIMS to be?
        if (order.OrganizationId != activationRequest.OrganizationId)
        {
            throw new UnauthorizedAccessException("Bạn không có quyền kích hoạt mã này");
        }

        // SECURITY CHECK 2: Has this key already been activated
        if (order.IsKeyActivated)
        {
            throw new OperationFailedException("Mã này đã được kích hoạt");
        }

        // If all check passed claim the key
        order.IsKeyActivated = true;
        bool result = await uow.SaveChangesAsync(cancellationToken);

        // Return result
        return result;
    }
}
