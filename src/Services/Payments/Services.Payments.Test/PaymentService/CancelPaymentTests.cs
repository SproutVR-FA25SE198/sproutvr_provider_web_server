using Common.Application.Abstractions.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Net.payOS;
using Net.payOS.Errors;
using Net.payOS.Types;
using OrdersService;
using Services.Payments.Application.Abstractions.Grpc.Client;
using Services.Payments.Application.BusinessLogics;
using Services.Payments.Infrastructure.Services;

namespace Services.Payments.Test.PaymentService;
public class CancelPaymentTests
{
    // Mocks and Fields
    private readonly Mock<PayOS> _mockPayOS;
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly Mock<ILogger<PayosPaymentService>> _mockLogger;
    private readonly Mock<IGrpcOrderClient> _mockGrpcClient;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly PayosPaymentService _service;

    // Constructor / Setup
    public CancelPaymentTests()
    {
        // Initialize Mocks
        _mockConfig = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<PayosPaymentService>>();
        _mockGrpcClient = new Mock<IGrpcOrderClient>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        // Setup Default Valid Configuration
        // Client Apps
        _mockConfig.Setup(c => c["ClientApp:BaseUrl"]).Returns("https://client.com");
        _mockConfig.Setup(c => c["ClientApp:CancelPath"]).Returns("/cancel-default");
        _mockConfig.Setup(c => c["ClientApp:ReturnPath"]).Returns("/return-default");

        // PayOS Config
        _mockConfig.Setup(c => c["PayOs:ChecksumKey"]).Returns("test_checksum_key");
        _mockConfig.Setup(c => c["PayOs:ExpiredTime"]).Returns("300"); // 5 minutes default

        // Mock PayOS
        _mockPayOS = new Mock<PayOS>("test_client_id", "test_api_key", "test_checksum_key", (string?)null!);

        // Initialize Service with Mocks
        _service = new PayosPaymentService(
            _mockPayOS.Object,
            _mockConfig.Object,
            _mockLogger.Object,
            _mockGrpcClient.Object,
            _mockUnitOfWork.Object
        );
    }

    #region UTCID 01, 02, 03, 04, 05 - CancelPayment

    [Fact]
    public async Task CancelPayment_UTCID01_ShouldReturnInfo_WhenOrderUpdateSuccess_AndPayOsCancelSuccess()
    {
        // Condition: OrderCode = 1000, Order Update = Success, PayOS = Returns Object
        long orderCode = 1000;
        var grpcResponse = new UpdateOrderStatusResponse { IsSuccess = true };

        // Mock gRPC to return Success (1 argument matching Service usage)
        _mockGrpcClient.Setup(g => g.UpdateOrderStatusAsync(It.IsAny<UpdateOrderStatusRequest>()))
            .ReturnsAsync(grpcResponse);

        // Mock PayOS to return valid information (10 arguments matching Error Image)
        var expectedInfo = new PaymentLinkInformation(
            "id_123",           // id
            orderCode,          // orderCode
            5000,               // amount
            0,                  // amountPaid
            5000,               // amountRemaining
            "CANCELLED",        // status
            "2023-10-01",       // createdAt
            new List<Transaction>(), // transactions
            "2023-10-01",       // canceledAt
            "User Request"      // cancellationReason
        );

        _mockPayOS.Setup(p => p.cancelPaymentLink(orderCode, null))
            .ReturnsAsync(expectedInfo);

        // Act
        PaymentLinkInformation? result = await _service.CancelPayment(orderCode);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedInfo, result);

        // Verify gRPC was called with Payment_Failed status
        _mockGrpcClient.Verify(g => g.UpdateOrderStatusAsync(It.Is<UpdateOrderStatusRequest>(r =>
            r.OrderCode == orderCode &&
            r.Status == OrderStatus.Payment_Failed.ToString())), Times.Once);

        // Verify PayOS cancel was called
        _mockPayOS.Verify(p => p.cancelPaymentLink(orderCode, null), Times.Once);
    }

    [Fact]
    public async Task CancelPayment_UTCID02_ShouldReturnNull_WhenOrderUpdateFails()
    {
        // Condition: OrderCode = 1000, Order Update = Fail (IsSuccess: false)
        long orderCode = 1000;
        var grpcResponse = new UpdateOrderStatusResponse { IsSuccess = false };

        // Mock gRPC to return Failure
        _mockGrpcClient.Setup(g => g.UpdateOrderStatusAsync(It.IsAny<UpdateOrderStatusRequest>()))
            .ReturnsAsync(grpcResponse);

        // Act
        PaymentLinkInformation? result = await _service.CancelPayment(orderCode);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CancelPayment_UTCID03_ShouldThrowArgumentException_WhenGrpcClientThrowsArgumentException()
    {
        // Condition: OrderCode = 1000, gRPC Client throws ArgumentException (Simulating invalid request status)
        long orderCode = 1000;

        // Mock gRPC to Throw ArgumentException
        _mockGrpcClient.Setup(g => g.UpdateOrderStatusAsync(It.IsAny<UpdateOrderStatusRequest>()))
            .ThrowsAsync(new ArgumentException("Invalid Status"));

        // Act & Assert
        ArgumentException ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.CancelPayment(orderCode));
        Assert.Equal("Invalid Status", ex.Message);
    }

    [Fact]
    public async Task CancelPayment_UTCID04_ShouldThrowException_WhenPayOsThrowsException()
    {
        // Condition: OrderCode = 1000, Order Update = Success, PayOS = Throws Exception
        long orderCode = 1000;
        var grpcResponse = new UpdateOrderStatusResponse { IsSuccess = true };

        // Mock gRPC to return Success
        _mockGrpcClient.Setup(g => g.UpdateOrderStatusAsync(It.IsAny<UpdateOrderStatusRequest>()))
            .ReturnsAsync(grpcResponse);

        // Act & Assert
        PayOSError ex = await Assert.ThrowsAsync<PayOSError>(() => _service.CancelPayment(orderCode));
        Assert.Equal("Cổng thanh toán không tồn tại hoặc đã tạm dừng, vui lòng chọn cổng khác", ex.Message);
    }

    [Fact]
    public async Task CancelPayment_UTCID05_ShouldThrowException_WhenServerThrowsGenericException()
    {
        // Condition: OrderCode = 1000, Order Update = Throws Generic Exception
        long orderCode = 1000;

        // Mock gRPC to Throw Generic Exception
        _mockGrpcClient.Setup(g => g.UpdateOrderStatusAsync(It.IsAny<UpdateOrderStatusRequest>()))
            .ThrowsAsync(new Exception("Internal Server Error"));

        // Act & Assert
        Exception ex = await Assert.ThrowsAsync<Exception>(() => _service.CancelPayment(orderCode));
        Assert.Equal("Internal Server Error", ex.Message);
    }

    #endregion
}
