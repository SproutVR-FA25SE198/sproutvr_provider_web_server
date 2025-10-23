using MediatR;

namespace Services.Accounts.Application.BusinessLogics.SystemAdmins.Features.UpdatePendingOrders;
public class UpdateSystemAdminPendingOrdersCommand : IRequest<bool>
{
    public Guid SystemAdminId { get; set; }
    public bool IsIncrement { get; set; } // true: increment, false: decrement
}
