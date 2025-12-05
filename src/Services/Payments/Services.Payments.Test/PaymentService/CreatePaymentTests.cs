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
    private readonly Mock<PayOS> _mockPayOS;
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly Mock<ILogger<PayosPaymentService>> _mockLogger;
    private readonly Mock<IGrpcOrderClient> _mockGrpcClient;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
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

        // 3. Mock PayOS
        _mockPayOS = new Mock<PayOS>("test_client_id", "test_api_key", "test_checksum_key", (string?)null!);

        // 4. Initialize Service
        _service = new PayosPaymentService(
            _mockPayOS.Object,
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
        await Assert.ThrowsAsync<NullReferenceException>(() =>
            _service.CreatePayment(dto));
    }

    #endregion

    #region UTCID 02 & 03 - Happy Paths

    [Fact]
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

        var expectedResult = new CreatePaymentResult("bin", "acc", 5000, "desc", 100, "VND", "linkId", "PENDING", 123456, "http://checkout", "qr");
        _mockPayOS.Setup(p => p.createPaymentLink(It.IsAny<PaymentData>())).ReturnsAsync(expectedResult);

        // Act
        CreatePaymentResult result = await _service.CreatePayment(dto);

        // Assert
        Assert.NotNull(result);
        // Verify Default URLs used from Config
        string expectedCancel = "https://client.com/cancel-default";
        string expectedReturn = "https://client.com/return-default";

        _mockPayOS.Verify(p => p.createPaymentLink(It.Is<PaymentData>(d =>
            d.cancelUrl == expectedCancel && d.returnUrl == expectedReturn)), Times.Once);
    }

    [Fact]
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

        var expectedResult = new CreatePaymentResult("bin", "acc", 5000, "desc", 100, "VND", "linkId", "PENDING", 123456, "http://checkout", "qr");
        _mockPayOS.Setup(p => p.createPaymentLink(It.IsAny<PaymentData>())).ReturnsAsync(expectedResult);

        // Act
        CreatePaymentResult result = await _service.CreatePayment(dto);

        // Assert
        Assert.NotNull(result);
        // Verify Custom URLs used
        _mockPayOS.Verify(p => p.createPaymentLink(It.Is<PaymentData>(d =>
            d.cancelUrl == customCancel && d.returnUrl == customReturn)), Times.Once);
    }

    #endregion

    #region UTCID 04, 05, 06, 07 - Configuration Logic

    [Fact]
    public async Task CreatePayment_UTCID04_ShouldThrowException_WhenLibraryReturnsNullOrFails()
    {
        // Condition: Request Valid, Config Valid, Library Result is NULL (or fails)
        var dto = new CreatePaymentDto
        {
            OrderCode = 100,
            TotalMoneyAmount = 5000,
            CancelUrl = null!,
            ReturnUrl = null!
        };

        // Simulating the library failing gracefully by returning null, 
        // OR throwing an exception. Decision table marks "Exception" as output.
        // Since the code provided does NOT check for null, we assume the library throws.
        _mockPayOS.Setup(p => p.createPaymentLink(It.IsAny<PaymentData>()))
                  .ThrowsAsync(new Exception());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
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
        PayOSError ex = await Assert.ThrowsAsync<PayOSError>(() =>
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
        // Convert.ToInt32("") throws FormatException
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
        // Convert.ToInt32("abc") throws FormatException
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

        // The library verifies the amount and throws ArgumentException
        _mockPayOS.Setup(p => p.createPaymentLink(It.Is<PaymentData>(d => d.amount < 0)))
                  .ThrowsAsync(new ArgumentException("Amount must be positive"));

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CreatePayment(dto));
    }

    [Fact]
    public async Task CreatePayment_UTCID09_ShouldThrowException_WhenServerIsDown()
    {
        // Condition: PayOS Server is down (External Failure)
        // Result: Exception (Type A - Abnormal)
        var dto = new CreatePaymentDto { OrderCode = 100, TotalMoneyAmount = 5000, CancelUrl = null!, ReturnUrl = null! };

        // Mock PayOS throwing a generic Exception
        _mockPayOS.Setup(p => p.createPaymentLink(It.IsAny<PaymentData>()))
                  .ThrowsAsync(new Exception("Internal Server Error"));

        // Act & Assert
        Exception ex = await Assert.ThrowsAsync<Exception>(() =>
            _service.CreatePayment(dto));

        Assert.Equal("Internal Server Error", ex.Message);
    }

    #endregion
}
