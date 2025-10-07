using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;
using Services.Payments.Application.Abstractions;
using Services.Payments.Application.BusinessLogics.CreatePayment;

namespace Services.Payments.Infrastructure.Services;
public class PayosPaymentService : IPayosPaymentService
{
    private readonly PayOS _payOS;
    private readonly IConfiguration _config;

    private readonly string _clientBaseUrl;
    private readonly string _clientCancelPath;
    private readonly string _clientReturnPath;

    public PayosPaymentService(PayOS payOS,
        IConfiguration config)
    {
        _payOS = payOS;
        _config = config;
        
        #pragma warning disable CS8601 // Possible null reference assignment.
        _clientBaseUrl = _config["ClientApp:BaseUrl"];
        _clientCancelPath = _config["ClientApp:CancelPath"];
        _clientReturnPath = _config["ClientApp:ReturnPath"];
        #pragma warning restore CS8601 // Possible null reference assignment.

    }
    public async Task<CreatePaymentResult> CreatePayment(CreatePaymentDto dto)
    {
        int orderCode = dto.OrderCode;
        int amount = dto.TotalMoneyAmount;
        string description = "Don hang " + orderCode;
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

    public Task<bool> VerifyPayment(int orderCode)
    {
        throw new NotImplementedException();
    }

    public async Task<PaymentLinkInformation> CancelPayment(int orderCode)
    {
        PaymentLinkInformation cancelledPaymentLinkInfo = await _payOS.cancelPaymentLink(orderCode);
        return cancelledPaymentLinkInfo;
    }


    private string GenerateSignature(int amount, string cancelUrl, string description, long expiredAt, int orderCode, string returnUrl)
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
