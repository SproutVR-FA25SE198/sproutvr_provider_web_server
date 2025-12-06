namespace Services.Payments.Application.BusinessLogics;
public enum OrderStatus
{
    Payment_Pending,
    Bundle_Pending, // payment is successful, waiting for bundle generation
    Payment_Failed,
    Finished,
    Canceled,
    Refunded
}
