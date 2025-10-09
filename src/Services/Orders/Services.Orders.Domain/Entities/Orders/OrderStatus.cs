namespace Services.Orders.Domain.Entities.Orders;
public enum OrderStatus
{
    Pending_Payment,
    Pending_Bundle, // payment is successful, waiting for bundle generation
    Payment_Failed,
    Finished,
    Canceled
}
