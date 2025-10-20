namespace Services.Orders.Domain.Entities.Orders;
public enum OrderStatus
{
    Pending_Payment,
    Payment_Failed,
    Pending_Bundle, // payment is successful, waiting for bundle preparation
    Assigned, // order is assigned to an admin
    Finished, // bundle uploaded
    Canceled // order is canceled by org
}
