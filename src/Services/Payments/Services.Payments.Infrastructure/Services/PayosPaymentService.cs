using System.Security.Cryptography;
using System.Text;
using Common.Application.Abstractions.Data;
using Common.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Net.payOS;
using Net.payOS.Types;
using OrdersService;
using Services.Payments.Application.Abstractions;
using Services.Payments.Application.Abstractions.Grpc.Client;
using Services.Payments.Application.BusinessLogics;
using Services.Payments.Application.BusinessLogics.CreatePayment;
using Services.Payments.Domain.Entities.Payments;

namespace Services.Payments.Infrastructure.Services;
public class PayosPaymentService : IPayosPaymentService
{
    private readonly PayOS _payOS;
    private readonly IConfiguration _config;
    private readonly ILogger<PayosPaymentService> _logger;
    private readonly IGrpcOrderClient _grpcOrderClient;
    private readonly IUnitOfWork _unitOfWork;

    private readonly string _clientBaseUrl;
    private readonly string _clientCancelPath;
    private readonly string _clientReturnPath;

    public PayosPaymentService(
        PayOS payOS,
        IConfiguration config,
        ILogger<PayosPaymentService> logger,
        IGrpcOrderClient grpcOrderClient,
        IUnitOfWork unitOfWork
        )
    {
        _payOS = payOS;
        _config = config;
        _logger = logger;
        _grpcOrderClient = grpcOrderClient;
        _unitOfWork = unitOfWork;

#pragma warning disable CS8601 // Possible null reference assignment.
        _clientBaseUrl = _config["ClientApp:BaseUrl"];
        _clientCancelPath = _config["ClientApp:CancelPath"];
        _clientReturnPath = _config["ClientApp:ReturnPath"];
#pragma warning restore CS8601 // Possible null reference assignment.

    }
    public async Task<CreatePaymentResult> CreatePayment(CreatePaymentDto dto)
    {
        long orderCode = dto.OrderCode;
        int amount = dto.TotalMoneyAmount;
        string description = "Don hang ORD" + orderCode;
        string cancelUrl = dto.CancelUrl ?? $"{_clientBaseUrl}{_clientCancelPath}";
        string returnUrl = dto.ReturnUrl ?? $"{_clientBaseUrl}{_clientReturnPath}";

#pragma warning disable CA1305 // Specify IFormatProvider
        long expiredAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + Convert.ToInt32(_config["PayOs:ExpiredTime"]);
#pragma warning restore CA1305 // Specify IFormatProvider

        string signature = GenerateSignature(amount, cancelUrl, description, expiredAt, orderCode, returnUrl);

        // payment data
        var paymentData = new PaymentData(
            orderCode,
            amount,
            description,
            [],
            cancelUrl,
            returnUrl,
            signature,
            expiredAt: expiredAt
        );

        // Create payment link and return
        CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);

        return createPayment;
    }
    public async Task<PaymentLinkInformation> CancelPayment(long orderCode)
    {
        OrdersService.UpdateOrderStatusResponse result = await _grpcOrderClient.UpdateOrderStatusAsync(new OrdersService.UpdateOrderStatusRequest
        {
            OrderCode = orderCode,
            Status = OrderStatus.Payment_Failed.ToString()
        });

        PaymentLinkInformation cancelledPaymentLinkInfo = null;
        if (result.IsSuccess)
        { 
            cancelledPaymentLinkInfo = await _payOS.cancelPaymentLink(orderCode); 
        }

        return cancelledPaymentLinkInfo;
    }

    public async Task<bool> ProcessPayment(WebhookType body)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(body);
            WebhookData data = _payOS.verifyPaymentWebhookData(body);

            // For setting up webhook only
            if (data.description == "Ma giao dich thu nghiem" || data.description == "VQRIO123")
            {
                return true;
            }

            // Check the status code
            OrderStatus orderStatus = data.code == "00" ? OrderStatus.Bundle_Pending : OrderStatus.Payment_Failed;

            // grpc to update order status in Orders service
            OrdersService.UpdateOrderStatusResponse updateOrderResult = await _grpcOrderClient.UpdateOrderStatusAsync(new OrdersService.UpdateOrderStatusRequest
            {
                OrderCode = data.orderCode,
                Status = orderStatus.ToString()
            });

            // store new payment transaction record in database
            var paymentTransaction = new PaymentTransaction
            {
                Amount = data.amount,
                OrderId = Guid.Parse(updateOrderResult.OrderId),
                PaymentMethod = PaymentMethod.PAYOS,
                TransactionCode = data.reference,
                BankCode = data.counterAccountBankId,
                BankName = data.counterAccountBankName,
                PaymentType = PaymentType.MapOrder,
                TransactionDateTime = data.transactionDateTime,
                Status = data.code == "00" ? PaymentStatus.Succeeded : PaymentStatus.Failed,
                Description = data.description,
                Currency = data.currency,
            };
            _unitOfWork.Repository<PaymentTransaction>().Add(paymentTransaction);
            bool result = await _unitOfWork.SaveChangesAsync();

            return updateOrderResult.IsSuccess && result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while processing payment webhook");
            throw new OperationFailedException("Error while processing payment");
        }
    }

    // helper
    private string GenerateSignature(long amount, string cancelUrl, string description, long expiredAt, long orderCode, string returnUrl)
    {
        // Create a dictionary of parameters to ensure alphabetical sorting.
        // PayOS requires the signature data string to be built from parameters sorted alphabetically by key.
        var parameters = new SortedDictionary<string, object>
            {
                { "amount", amount },
                { "cancelUrl", cancelUrl },
                { "description", description },
                { "expiredAt", expiredAt },
                { "orderCode", orderCode },
                { "returnUrl", returnUrl }
            };

        // Build the data string from the sorted parameters alphabetically.
        string hashSource = string.Join("&", parameters.Select(kvp => $"{kvp.Key}={kvp.Value}"));

        // Get the Checksum Key from configuration.
        string checksumKey = _config["PayOs:ChecksumKey"]
            ?? throw new InvalidOperationException("PayOs:ChecksumKey is not configured.");

        // Create the HMACSHA256 hash using the static method for efficiency.
        byte[] hashBytes = HMACSHA256.HashData(
            Encoding.UTF8.GetBytes(checksumKey),
            Encoding.UTF8.GetBytes(hashSource)
        );

        // Convert the byte array to a lowercase hexadecimal string.
        // Format required by the PayOS documentation.
        return Convert.ToHexStringLower(hashBytes);
    }

}
