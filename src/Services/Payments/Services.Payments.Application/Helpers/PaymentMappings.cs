using Services.Payments.Application.BusinessLogics.GetPaymentsList;
using Services.Payments.Domain.Entities.Payments;

namespace Services.Payments.Application.Helpers;
public static class PaymentMappings
{
    public static PaymentDto ToDto(this PaymentTransaction payment)
    {
        return new PaymentDto
        {
            Amount = payment.Amount,
            OrderId = payment.OrderId,
            PaymentMethod = payment.PaymentMethod?.ToString(),
            TransactionCode = payment.TransactionCode,
            BankCode = payment.BankCode,
            BankName = payment.BankName,
            PaymentType = payment.PaymentType.ToString(),
            Description = payment.Description,
            Status = payment.Status.ToString(),
            CreatedAtUtc = payment.CreatedAtUtc,
            Currency = payment.Currency
        };
    }
}
