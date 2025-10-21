namespace Services.Orders.Domain.Entities.Orders;
public enum OrderStatus
{
    Payment_Pending,
    Payment_Failed,
    Bundle_Pending, // payment is successful, waiting for bundle preparation
    Assigned, // order is assigned to an admin
    Finished, // bundle uploaded
    Canceled, // order is canceled by org
    Refunded
}
