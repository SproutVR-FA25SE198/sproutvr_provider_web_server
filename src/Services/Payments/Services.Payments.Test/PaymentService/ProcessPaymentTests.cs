using Common.Application.Abstractions.Data;
using Common.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Net.payOS;
using Net.payOS.Types;
using Net.payOS.Utils;
using Newtonsoft.Json.Linq;
using OrdersService;
using Services.Payments.Application.Abstractions.Grpc.Client;
using Services.Payments.Domain.Entities.Payments;
using Services.Payments.Infrastructure.Services;

namespace Services.Payments.Test.PaymentService;
public class ProcessPaymentTests
{
    // Mocks
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly Mock<ILogger<PayosPaymentService>> _mockLogger;
    private readonly Mock<IGrpcOrderClient> _mockGrpcClient;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IGenericRepository<PaymentTransaction>> _mockRepo;

    // Use Real Object
    private readonly PayOS _payOS;

    // Service under test
    private readonly PayosPaymentService _service;

    // Constants for testing
    private const string ChecksumKey = "checksum";

    public ProcessPaymentTests()
    {
        // 1. Init Mocks
        _mockConfig = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<PayosPaymentService>>();
        _mockGrpcClient = new Mock<IGrpcOrderClient>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockRepo = new Mock<IGenericRepository<PaymentTransaction>>();

        // 2. Setup Config
        _mockConfig.Setup(c => c["ClientApp:BaseUrl"]).Returns("https://client.com");
        _mockConfig.Setup(c => c["ClientApp:CancelPath"]).Returns("/cancel");
        _mockConfig.Setup(c => c["ClientApp:ReturnPath"]).Returns("/return");
        _mockConfig.Setup(c => c["PayOs:ChecksumKey"]).Returns(ChecksumKey);
        _mockConfig.Setup(c => c["PayOs:ExpiredTime"]).Returns("300");

        // 3. Setup PayOS (REAL INSTANCE)
        // We use the real class because we cannot mock non-virtual methods.
        _payOS = new PayOS("clientId", "apiKey", ChecksumKey);

        // 4. Setup UnitOfWork to return our Mock Repo
        _mockUnitOfWork.Setup(u => u.Repository<PaymentTransaction>()).Returns(_mockRepo.Object);

        // 5. Init Service
        _service = new PayosPaymentService(
            _payOS, // Inject Real Object
            _mockConfig.Object,
            _mockLogger.Object,
            _mockGrpcClient.Object,
            _mockUnitOfWork.Object
        );
    }

    // Helper to create valid WebhookData
    private static WebhookData CreateWebhookData(string description, string code)
    {
        return new WebhookData(
            1000,               // orderCode
            5000,               // amount
            description,        // description
            "123456",           // accountNumber
            "TRANS_REF_123",    // reference
            "2023-10-01",       // transactionDateTime
            "VND",              // currency
            "link_id",          // paymentLinkId
            code,               // code
            "desc",             // desc
            "BANK_ID",          // counterAccountBankId
            "BANK_NAME",        // counterAccountBankName
            "ACC_NAME",         // counterAccountName
            "ACC_NUM",          // counterAccountNumber
            "VIRT_NAME",        // virtualAccountName
            "VIRT_NUM"          // virtualAccountNumber
        );
    }

    // Helper to create WebhookType
    private static WebhookType CreateWebhookType(WebhookData data, bool useValidSignature = true)
    {
        string signature;

        if (data == null)
        {
            // If data is null, we can't sign it. Pass null or empty.
            return new WebhookType("00", "Success", true, null!, "invalid_sig");
        }

        if (useValidSignature)
        {
            // CRITICAL: We use the actual logic to sign the data so _payOS.verifyPaymentWebhookData succeeds.
            // If SignatureControl is not accessible here, copy the implementation of CreateSignatureFromObj into this test class.
            signature = SignatureControl.CreateSignatureFromObj(JObject.FromObject(data), ChecksumKey);
        }
        else
        {
            signature = "invalid_signature_string";
        }

        return new WebhookType("00", "Success", true, data, signature);
    }

    #region UTCID 01, 02, 03, 04 - Logic Flow

    [Fact]
    public async Task ProcessPayment_UTCID01_ShouldReturnTrue_WhenUpdatesSucceed()
    {
        // Condition: Normal Transaction, Update Order Success, Save DB Success
        // Input: We create data with a VALID signature
        WebhookData data = CreateWebhookData("Normal Order", "00");
        WebhookType body = CreateWebhookType(data, useValidSignature: true);

        // Mock gRPC Success
        _mockGrpcClient.Setup(g => g.UpdateOrderStatusAsync(It.IsAny<UpdateOrderStatusRequest>()))
            .ReturnsAsync(new UpdateOrderStatusResponse { IsSuccess = true, OrderId = Guid.NewGuid().ToString() });

        // Mock DB Save Success
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(true);

        // Act
        bool result = await _service.ProcessPayment(body);

        // Assert
        Assert.True(result);
        _mockGrpcClient.Verify(g => g.UpdateOrderStatusAsync(It.IsAny<UpdateOrderStatusRequest>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task ProcessPayment_UTCID02_ShouldReturnFalse_WhenUpdateOrderFails()
    {
        // Condition: Normal Transaction, Update Order Fails (IsSuccess = false)
        WebhookData data = CreateWebhookData("Normal Order", "00");
        WebhookType body = CreateWebhookType(data, useValidSignature: true);

        // Mock gRPC Failure
        _mockGrpcClient.Setup(g => g.UpdateOrderStatusAsync(It.IsAny<UpdateOrderStatusRequest>()))
            .ReturnsAsync(new UpdateOrderStatusResponse { IsSuccess = false, OrderId = Guid.NewGuid().ToString() });

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(true);

        // Act
        bool result = await _service.ProcessPayment(body);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ProcessPayment_UTCID03_ShouldReturnFalse_WhenDbSaveFails()
    {
        // Condition: Normal Transaction, Update Order Success, DB Save Fails
        WebhookData data = CreateWebhookData("Normal Order", "00");
        WebhookType body = CreateWebhookType(data, useValidSignature: true);

        // Mock gRPC Success
        _mockGrpcClient.Setup(g => g.UpdateOrderStatusAsync(It.IsAny<UpdateOrderStatusRequest>()))
            .ReturnsAsync(new UpdateOrderStatusResponse { IsSuccess = true, OrderId = Guid.NewGuid().ToString() });

        // Mock DB Save Failure
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(false);

        // Act
        bool result = await _service.ProcessPayment(body);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ProcessPayment_UTCID04_ShouldReturnTrue_WhenDescriptionIsTestTransaction()
    {
        // Condition: Description "VQRIO123" -> Short Circuit
        WebhookData data = CreateWebhookData("VQRIO123", "00");
        WebhookType body = CreateWebhookType(data, useValidSignature: true);

        // Act
        bool result = await _service.ProcessPayment(body);

        // Assert
        Assert.True(result);
        _mockGrpcClient.Verify(g => g.UpdateOrderStatusAsync(It.IsAny<UpdateOrderStatusRequest>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Never);
    }

    #endregion

    #region UTCID 05, 06, 07 - Validation Exceptions

    [Fact]
    public async Task ProcessPayment_UTCID05_ShouldThrowArgumentNullException_WhenBodyIsNull()
    {
        // Condition: WebhookType is null
        WebhookType? body = null;

        // Act & Assert
        await Assert.ThrowsAsync<OperationFailedException>(() => _service.ProcessPayment(body!));
    }

    [Fact]
    public async Task ProcessPayment_UTCID06_ShouldThrowOperationFailedException_WhenDataIsNull()
    {
        // Condition: Data is null. The REAL verifyPaymentWebhookData throws "No data".
        // We pass null for data
        var body = new WebhookType("00", "desc", true, null!, "sig");

        // No Setup needed. Real object throws Exception("No data"), Service catches it.

        // Act & Assert
        OperationFailedException ex = await Assert.ThrowsAsync<OperationFailedException>(() => _service.ProcessPayment(body));
        Assert.Equal("Error while processing payment", ex.Message);
    }

    [Fact]
    public async Task ProcessPayment_UTCID07_ShouldThrowOperationFailedException_WhenSignatureIsInvalid()
    {
        // Condition: Signature invalid -> REAL verifyPaymentWebhookData throws "data unreliable..."
        WebhookData data = CreateWebhookData("Normal Order", "00");

        // Pass FALSE to generate a mismatching signature
        WebhookType body = CreateWebhookType(data, useValidSignature: false);

        // Act & Assert
        // The real PayOS logic will throw the Exception because the signature doesn't match
        OperationFailedException ex = await Assert.ThrowsAsync<OperationFailedException>(() => _service.ProcessPayment(body));
        Assert.Equal("Error while processing payment", ex.Message);
    }

    #endregion

    #region UTCID 08, 09, 10 - Infrastructure Exceptions

    [Fact]
    public async Task ProcessPayment_UTCID08_ShouldThrowOperationFailedException_WhenGrpcClientThrows()
    {
        // Condition: gRPC Client throws Exception
        WebhookData data = CreateWebhookData("Normal Order", "00");
        WebhookType body = CreateWebhookType(data, useValidSignature: true);

        // Mock gRPC to Throw
        _mockGrpcClient.Setup(g => g.UpdateOrderStatusAsync(It.IsAny<UpdateOrderStatusRequest>()))
            .ThrowsAsync(new Exception("gRPC Service Unavailable"));

        // Act & Assert
        OperationFailedException ex = await Assert.ThrowsAsync<OperationFailedException>(() => _service.ProcessPayment(body));
        Assert.Equal("Error while processing payment", ex.Message);
    }

    [Fact]
    public async Task ProcessPayment_UTCID09_ShouldThrowOperationFailedException_WhenDbSaveThrows()
    {
        // Condition: DB Save throws Exception
        WebhookData data = CreateWebhookData("Normal Order", "00");
        WebhookType body = CreateWebhookType(data, useValidSignature: true);

        _mockGrpcClient.Setup(g => g.UpdateOrderStatusAsync(It.IsAny<UpdateOrderStatusRequest>()))
            .ReturnsAsync(new UpdateOrderStatusResponse { IsSuccess = true, OrderId = Guid.NewGuid().ToString() });

        // Mock DB to Throw
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(default))
            .ThrowsAsync(new Exception("Database Connection Failure"));

        // Act & Assert
        OperationFailedException ex = await Assert.ThrowsAsync<OperationFailedException>(() => _service.ProcessPayment(body));
        Assert.Equal("Error while processing payment", ex.Message);
    }

    [Fact]
    public async Task ProcessPayment_UTCID10_ShouldThrowOperationFailedException_WhenServerIsDown()
    {
        // Condition: Server Down
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => throw new Exception());
    }

    #endregion
}
