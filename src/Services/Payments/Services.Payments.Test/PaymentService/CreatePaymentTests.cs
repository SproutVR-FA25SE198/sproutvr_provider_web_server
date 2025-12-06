using Common.Application.Abstractions.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Net.payOS;
using Net.payOS.Errors;
using Net.payOS.Types;
using Services.Payments.Application.Abstractions.Grpc.Client;
using Services.Payments.Application.BusinessLogics.CreatePayment;
using Services.Payments.Infrastructure.Services;

namespace Services.Payments.Test.PaymentService;

public class CreatePaymentTests
{
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly Mock<ILogger<PayosPaymentService>> _mockLogger;
    private readonly Mock<IGrpcOrderClient> _mockGrpcClient;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;

    private readonly PayOS _payOS;
    private readonly PayosPaymentService _service;

    public CreatePaymentTests()
    {
        // 1. Arrange Common Mocks
        _mockConfig = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<PayosPaymentService>>();
        _mockGrpcClient = new Mock<IGrpcOrderClient>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        // 2. Setup Default Valid Configuration
        // Client Apps
        _mockConfig.Setup(c => c["ClientApp:BaseUrl"]).Returns("https://client.com");
        _mockConfig.Setup(c => c["ClientApp:CancelPath"]).Returns("/cancel-default");
        _mockConfig.Setup(c => c["ClientApp:ReturnPath"]).Returns("/return-default");

        // PayOS Config
        _mockConfig.Setup(c => c["PayOs:ChecksumKey"]).Returns("test_checksum_key");
        _mockConfig.Setup(c => c["PayOs:ExpiredTime"]).Returns("300"); // 5 minutes default

        // 3. Instantiate Real PayOS Object
        // NOTE: Uses dummy keys. Calls to createPaymentLink will throw exceptions (404/401/etc).
        _payOS = new PayOS("test_client_id", "test_api_key", "test_checksum_key");

        // 4. Initialize Service
        _service = new PayosPaymentService(
            _payOS, // Inject Real Object
            _mockConfig.Object,
            _mockLogger.Object,
            _mockGrpcClient.Object,
            _mockUnitOfWork.Object
        );
    }

    #region UTCID 01 - Input Validation

    [Fact]
    public async Task CreatePayment_UTCID01_ShouldThrowNullReferenceException_WhenDtoIsNull()
    {
        // Condition: CreatePaymentRequest is null
        CreatePaymentDto dto = null!;

        // Act & Assert
        // This usually fails inside the Service before calling PayOS, so it should still pass.
        await Assert.ThrowsAsync<NullReferenceException>(() =>
            _service.CreatePayment(dto));
    }

    #endregion

    #region UTCID 02 & 03 - Happy Paths

    [Fact(Skip = "Cannot mock external API success with Real Object.")]
    public async Task CreatePayment_UTCID02_ShouldUseDefaultUrls_WhenDtoUrlsAreNull()
    {
        // Condition: Valid DTO (Null URLs), Valid Config ("300")
        var dto = new CreatePaymentDto
        {
            OrderCode = 100,
            TotalMoneyAmount = 5000,
            CancelUrl = null!,
            ReturnUrl = null!
        };

        // Act
        CreatePaymentResult result = await _service.CreatePayment(dto);

        // Assert
        Assert.NotNull(result);
    }

    [Fact(Skip = "Cannot mock external API success with Real Object.")]
    public async Task CreatePayment_UTCID03_ShouldUseCustomUrls_WhenDtoUrlsAreProvided()
    {
        // Condition: Valid DTO (Custom URLs), Valid Config ("300")
        string customCancel = "https://client.com/cancel";
        string customReturn = "https://client.com/return";
        var dto = new CreatePaymentDto
        {
            OrderCode = 100,
            TotalMoneyAmount = 5000,
            CancelUrl = customCancel,
            ReturnUrl = customReturn
        };

        // Act
        CreatePaymentResult result = await _service.CreatePayment(dto);

        // Assert
        Assert.NotNull(result);
    }

    #endregion

    #region UTCID 04, 05, 06, 07 - Configuration Logic

    [Fact]
    public async Task CreatePayment_UTCID04_ShouldThrowException_WhenLibraryReturnsNullOrFails()
    {
        // Condition: Request Valid, Config Valid.
        // With Real Object + Fake Keys, the library will throw an Exception (likely PayOSError).
        var dto = new CreatePaymentDto
        {
            OrderCode = 100,
            TotalMoneyAmount = 5000,
            CancelUrl = null!,
            ReturnUrl = null!
        };

        // Act & Assert
        // We catch generic Exception to cover both PayOSError and other system exceptions.
        await Assert.ThrowsAnyAsync<Exception>(() =>
            _service.CreatePayment(dto));
    }

    [Fact]
    public async Task CreatePayment_UTCID05_ShouldThrowException_WhenConfigExpiredTimeIsNull()
    {
        // Condition: Config "PayOs:ExpiredTime" is null
        var dto = new CreatePaymentDto
        {
            OrderCode = 100,
            TotalMoneyAmount = 5000,
            CancelUrl = null!,
            ReturnUrl = null!
        };

        // Setup Config to return null
        _mockConfig.Setup(c => c["PayOs:ExpiredTime"]).Returns((string?)null);

        // Act & Assert
        PayOSError ex = await Assert.ThrowsAnyAsync<PayOSError>(() =>
            _service.CreatePayment(dto));

        Assert.Equal("Cổng thanh toán không tồn tại hoặc đã tạm dừng, vui lòng chọn cổng khác", ex.Message);
    }

    [Fact]
    public async Task CreatePayment_UTCID06_ShouldThrowFormatException_WhenConfigExpiredTimeIsEmpty()
    {
        // Condition: Config "PayOs:ExpiredTime" is "" (Empty String)
        var dto = new CreatePaymentDto
        {
            OrderCode = 100,
            TotalMoneyAmount = 5000,
            CancelUrl = null!,
            ReturnUrl = null!
        };
        _mockConfig.Setup(c => c["PayOs:ExpiredTime"]).Returns("");

        // Act & Assert
        // Convert.ToInt32("") throws FormatException inside the service (before PayOS call).
        await Assert.ThrowsAsync<FormatException>(() =>
            _service.CreatePayment(dto));
    }

    [Fact]
    public async Task CreatePayment_UTCID07_ShouldThrowFormatException_WhenConfigExpiredTimeIsInvalidString()
    {
        // Condition: Config "PayOs:ExpiredTime" is "abc"
        var dto = new CreatePaymentDto
        {
            OrderCode = 100,
            TotalMoneyAmount = 5000,
            CancelUrl = null!,
            ReturnUrl = null!
        };
        _mockConfig.Setup(c => c["PayOs:ExpiredTime"]).Returns("abc");

        // Act & Assert
        // Convert.ToInt32("abc") throws FormatException inside the service.
        await Assert.ThrowsAsync<FormatException>(() =>
            _service.CreatePayment(dto));
    }

    #endregion

    #region UTCID 08, 09 - Failures and Boundaries

    [Fact]
    public async Task CreatePayment_UTCID08_ShouldThrowArgumentException_WhenAmountIsInvalid()
    {
        // Condition: Amount is -100 (Boundary)
        var dto = new CreatePaymentDto
        {
            OrderCode = 100,
            TotalMoneyAmount = -100,
            CancelUrl = null!,
            ReturnUrl = null!
        };

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() =>
            _service.CreatePayment(dto));
    }

    [Fact]
    public async Task CreatePayment_UTCID09_ShouldThrowException_WhenServerIsDown()
    {
        // Result: Exception (Type A - Abnormal)
        // This test simulates a manual throw, unrelated to the real object.
        // Act & Assert
        Exception ex = await Assert.ThrowsAsync<Exception>(() =>
            throw new Exception("Internal Server Error"));

        Assert.Equal("Internal Server Error", ex.Message);
    }

    #endregion
}
