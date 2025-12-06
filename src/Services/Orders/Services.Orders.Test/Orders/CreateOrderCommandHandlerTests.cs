using Common.Application.Abstractions.Data;
using Common.Domain.Exceptions;
using Moq;
using PaymentsService;
using Services.Orders.Application.Abstractions.Grpc.Clients;
using Services.Orders.Application.BusinessLogics.Basket.DTOs;
using Services.Orders.Application.BusinessLogics.Orders.Features.CreateOrder;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Test.Orders;

#pragma warning disable CA1515 // Consider making public types internal
public class CreateOrderCommandHandlerTests
#pragma warning restore CA1515 // Consider making public types internal
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IGenericRepository<Order>> _mockOrderRepo;
    private readonly Mock<IGrpcMapClient> _mockMapClient;
    private readonly Mock<IGrpcPaymentClient> _mockPaymentClient;
    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        // 1. Arrange Common Mocks
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockOrderRepo = new Mock<IGenericRepository<Order>>();
        _mockMapClient = new Mock<IGrpcMapClient>();
        _mockPaymentClient = new Mock<IGrpcPaymentClient>();

        // Setup UoW to return our mocked Order Repository
        _mockUnitOfWork.Setup(u => u.Repository<Order>())
                       .Returns(_mockOrderRepo.Object);

        // Initialize Handler
        _handler = new CreateOrderCommandHandler(
            _mockUnitOfWork.Object,
            _mockMapClient.Object,
            _mockPaymentClient.Object
        );
    }

    #region 1. Input Validation & Logic Scenarios

    // UTCID 01 - Happy Path
    [Fact]
    public async Task Handle_ShouldCreateOrderSuccessfully_WhenDataIsValid()
    {
        // Arrange
        var mapGuid = Guid.NewGuid();
        string mapIdString = mapGuid.ToString();

        var basketItems = new List<BasketItemDto>
        {
            new BasketItemDto { MapId = mapIdString }
        };
        CreateOrderCommand command = CreateCommandWithBasket(basketItems);

        // Mock Map Client to return valid maps
        // MapDto has Guid MapId, but BasketItemDto has String MapId
        var mockMaps = new List<MapDto>
        {
            new MapDto { MapId = mapGuid, Price = 1000, MapName = "Test Map" }
        };

        // Since BasketItemDto.MapId is string, the handler passes List<string> to the client
        _mockMapClient.Setup(x => x.GetMapsByIdsAsync(It.IsAny<List<string>>()))
                      .ReturnsAsync(mockMaps);

        // Mock Payment Client to return success
        var paymentResponse = new OrderResponseDto { PaymentUrl = "https://example.com" };
        _mockPaymentClient.Setup(x => x.CreatePaymentAsync(It.IsAny<CreatePaymentRequest>()))
                          .ReturnsAsync(paymentResponse);

        // Simulate DB generating an ID when Add is called
        _mockOrderRepo.Setup(r => r.Add(It.IsAny<Order>()))
                      .Callback<Order>(o => o.Id = Guid.NewGuid());

        // Mock Save Success
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(true);

        // Act
        OrderResponseDto result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("https://example.com", result.PaymentUrl);
        Assert.NotEqual(Guid.Empty, result.OrderId);

        // Verify Save was called
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    // UTCID 02 - Basket is Null
    [Fact]
    public async Task Handle_ShouldThrowNullReferenceException_WhenBasketIsNull()
    {
        // Arrange
        var command = new CreateOrderCommand(new CreateOrderDto
        {
            OrganizationId = Guid.NewGuid(),
            PaymentMethod = "Banking",
            Basket = null! // Force null
        });

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    // UTCID 03 - Empty Basket
    [Fact]
    public async Task Handle_ShouldCreateOrder_WhenBasketIsEmpty()
    {
        // Arrange
        CreateOrderCommand command = CreateCommandWithBasket(new List<BasketItemDto>()); // Empty list

        _mockMapClient.Setup(x => x.GetMapsByIdsAsync(It.IsAny<List<string>>()))
                      .ReturnsAsync(new List<MapDto>());

        var paymentResponse = new OrderResponseDto { PaymentUrl = "https://example.com" };
        _mockPaymentClient.Setup(x => x.CreatePaymentAsync(It.IsAny<CreatePaymentRequest>()))
                          .ReturnsAsync(paymentResponse);

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(true);

        // Act
        OrderResponseDto result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);

        // Verify TotalAmount is 0
        _mockOrderRepo.Verify(r => r.Add(It.Is<Order>(o => o.TotalMoneyAmount == 0)), Times.Once);
    }

    #endregion

    #region 2. Persistence & Exception Scenarios

    // UTCID 04 - Save Failed
    [Fact]
    public async Task Handle_ShouldThrowOperationFailedException_WhenSaveFails()
    {
        // Arrange
        CreateOrderCommand command = CreateCommandWithBasket(new List<BasketItemDto>());

        // Setup mocks to pass logic steps
        _mockMapClient.Setup(x => x.GetMapsByIdsAsync(It.IsAny<List<string>>()))
                      .ReturnsAsync(new List<MapDto>());
        _mockPaymentClient.Setup(x => x.CreatePaymentAsync(It.IsAny<CreatePaymentRequest>()))
                      .ReturnsAsync(new OrderResponseDto());

        // Mock Save Failure
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(false);

        // Act & Assert
        OperationFailedException ex = await Assert.ThrowsAsync<OperationFailedException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Failed to create order", ex.Message);
    }

    // UTCID 05 - Unexpected Exception
    [Fact]
    public async Task Handle_ShouldThrowException_WhenDependencyFails()
    {
        // Arrange
        CreateOrderCommand command = CreateCommandWithBasket(new List<BasketItemDto> { new BasketItemDto() });

        // Mock Dependency Failure
        _mockMapClient.Setup(x => x.GetMapsByIdsAsync(It.IsAny<List<string>>()))
                      .ThrowsAsync(new Exception());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    #endregion

    // Helper
    private CreateOrderCommand CreateCommandWithBasket(List<BasketItemDto> items)
    {
        return new CreateOrderCommand(new CreateOrderDto
        {
            OrganizationId = Guid.NewGuid(),
            PaymentMethod = "Banking",
            Basket = new BasketDto
            {
                BasketItems = items
            }
        });
    }
}
